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

    [Header("Dynamic Speed Settings")]
    [SerializeField] private float minPitch = 1.0f;  // Normal pitch at 100% fuel
    [SerializeField] private float maxPitch = 2.0f;  // 2x speed at 0% fuel
    [SerializeField] private float speedUpThreshold = 0.3f; // Start speeding up when fuel below 30%

    private SoundAction? currentMusicTrack = null;
    private AudioSource currentMusicSource = null;
    private GamePlay.Interactables.FireplaceInteraction fireplace = null;

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
        Debug.Log("[MusicManager] Starting...");
        // Subscribe to game state changes
        if (GameStateManager.instance != null)
        {
            Debug.Log($"[MusicManager] Found GameStateManager, subscribing. Current state: {GameStateManager.instance.currentState}");
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
            // Play music based on initial state
            OnGameStateChanged(GameStateManager.instance.currentState);
        }
        else
        {
            Debug.LogWarning("[MusicManager] GameStateManager.instance is null! Music will not play.");
        }

        // Find and subscribe to campfire
        SubscribeToFireplace();
    }

    private void SubscribeToFireplace()
    {
        // Find the fireplace in the scene
        fireplace = FindObjectOfType<GamePlay.Interactables.FireplaceInteraction>();

        if (fireplace != null)
        {
            fireplace.OnFuelChanged += UpdateMusicSpeed;
            Debug.Log("[MusicManager] Subscribed to fireplace fuel changes.");
        }
        else
        {
            Debug.LogWarning("[MusicManager] Fireplace not found. Music speed won't change dynamically.");
        }
    }

    private void Update()
    {
        // Keep music speed updated dynamically during gameplay
        if (currentMusicTrack == SoundAction.GameMusic && currentMusicSource != null)
        {
            // This ensures the pitch stays correct even if fuel changes rapidly
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.instance != null)
        {
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }

        if (fireplace != null)
        {
            fireplace.OnFuelChanged -= UpdateMusicSpeed;
        }
    }

    /// <summary>
    /// Updates music pitch based on campfire fuel level
    /// </summary>
    private void UpdateMusicSpeed(float currentFuel, float maxFuel)
    {
        // Only affect game music during gameplay
        if (currentMusicTrack != SoundAction.GameMusic || currentMusicSource == null)
            return;

        float fuelPercentage = currentFuel / maxFuel;

        // Only speed up when fuel is below threshold
        if (fuelPercentage <= speedUpThreshold)
        {
            // Map fuel percentage (0-threshold) to pitch (maxPitch-minPitch)
            // When fuel = threshold, pitch = minPitch (1.0)
            // When fuel = 0, pitch = maxPitch (1.5)
            float normalizedFuel = fuelPercentage / speedUpThreshold; // 0 to 1
            float pitch = Mathf.Lerp(maxPitch, minPitch, normalizedFuel);
            currentMusicSource.pitch = pitch;
        }
        else
        {
            // Above threshold, keep normal speed
            currentMusicSource.pitch = minPitch;
        }
    }

    private void OnGameStateChanged(GameStateManager.GameState newState)
    {
        Debug.Log($"[MusicManager] Game state changed to: {newState}");

        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                Debug.Log("[MusicManager] Playing MainMenuMusic");
                PlayMusic(SoundAction.MainMenuMusic);
                break;

            case GameStateManager.GameState.GamePlay:
                Debug.Log("[MusicManager] Playing GameMusic");
                PlayMusic(SoundAction.GameMusic);
                break;

            case GameStateManager.GameState.GameOver:
                Debug.Log("[MusicManager] Playing GameOverMusic");
                PlayMusic(SoundAction.GameOverMusic);
                break;

            case GameStateManager.GameState.Paused:
                Debug.Log("[MusicManager] Paused - keeping current music");
                // Don't change music when paused, keep current track playing
                break;
        }
    }

    /// <summary>
    /// Play a music track. Stops the current track if different.
    /// </summary>
    public void PlayMusic(SoundAction musicAction)
    {
        Debug.Log($"[MusicManager] PlayMusic called with: {musicAction}");

        // If already playing this track, don't restart
        if (currentMusicTrack == musicAction && currentMusicSource != null && currentMusicSource.isPlaying)
        {
            Debug.Log($"[MusicManager] Already playing {musicAction}, skipping");
            return;
        }

        Debug.Log($"[MusicManager] Stopping current music: {currentMusicTrack}");
        // Stop current music
        StopCurrentMusic();

        Debug.Log($"[MusicManager] Starting new music loop: {musicAction}");
        // Play new music as a loop
        currentMusicSource = SoundManager.PlayLoop(musicAction);
        currentMusicTrack = musicAction;

        if (currentMusicSource != null)
        {
            Debug.Log($"[MusicManager] Successfully started {musicAction}. AudioSource playing: {currentMusicSource.isPlaying}");
        }
        else
        {
            Debug.LogError($"[MusicManager] Failed to start {musicAction} - SoundManager.PlayLoop returned null!");
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
