using UnityEngine;
using Managers.GeneralManagers;

/// <summary>
/// Manages background music based on game state and scene.
/// Automatically plays the appropriate music track and handles transitions.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private float fadeOutDuration = 1.5f;

    private SoundAction? currentMusicTrack = null;
    private AudioSource currentMusicSource = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Subscribe to game state changes
        if (GameStateManager.instance != null)
        {
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
            // Play music based on initial state
            OnGameStateChanged(GameStateManager.instance.currentState);
            Debug.Log($"[MusicManager] Started. Current game state: {GameStateManager.instance.currentState}");
        }
        else
        {
            Debug.LogWarning("[MusicManager] GameStateManager not found. Music won't change automatically based on game state.");
        }

        // Check if SoundManager is available
        if (SoundManager.Instance == null)
        {
            Debug.LogError("[MusicManager] SoundManager not found! Make sure SoundManager exists in the scene.");
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.instance != null)
        {
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                PlayMusic(SoundAction.MainMenuMusic);
                break;

            case GameStateManager.GameState.GamePlay:
                PlayMusic(SoundAction.GameMusic);
                break;

            case GameStateManager.GameState.GameOver:
                PlayMusic(SoundAction.GameOverMusic);
                break;

            case GameStateManager.GameState.Paused:
                // Don't change music when paused, keep current track playing
                break;
        }
    }

    /// <summary>
    /// Play a music track. Stops the current track if different.
    /// </summary>
    public void PlayMusic(SoundAction musicAction)
    {
        // If already playing this track, don't restart
        if (currentMusicTrack == musicAction && currentMusicSource != null && currentMusicSource.isPlaying)
        {
            return;
        }

        // Stop current music
        StopCurrentMusic();

        // Play new music as a loop
        currentMusicSource = SoundManager.PlayLoop(musicAction);
        currentMusicTrack = musicAction;

        if (currentMusicSource != null)
        {
            Debug.Log($"[MusicManager] Now playing: {musicAction}");
        }
        else
        {
            Debug.LogWarning($"[MusicManager] Failed to play music: {musicAction}. Make sure it's configured in SoundManager!");
        }
    }

    /// <summary>
    /// Stop the currently playing music with optional fade out
    /// </summary>
    public void StopCurrentMusic(bool fadeOut = true)
    {
        if (currentMusicTrack.HasValue)
        {
            SoundManager.StopLoop(currentMusicTrack.Value, fadeOut ? fadeOutDuration : 0f);
            currentMusicTrack = null;
            currentMusicSource = null;
        }
    }

    /// <summary>
    /// Manually play a specific music track (use for special events)
    /// </summary>
    public void PlayMusicOverride(SoundAction musicAction)
    {
        PlayMusic(musicAction);
    }
}
