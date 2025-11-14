using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Centralized settings manager that persists across scenes.
/// Use this singleton to manage all game settings (brightness, volume, fullscreen, etc.)
/// Also controls the AudioMixer directly to ensure volume works across all scenes.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    private static SettingsManager instance;
    public static SettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SettingsManager");
                instance = go.AddComponent<SettingsManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("AudioMixer")]
    [Tooltip("Optional: Assign the GlobalMixer here. If not assigned, it will try to load from Resources/Sounds/GlobalMixer")]
    [SerializeField] private AudioMixer audioMixer;

    // PlayerPrefs keys
    private const string BRIGHTNESS_KEY = "BrightnessValue";
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";

    // AudioMixer parameter names
    private const string MASTER_PARAM = "MasterVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SoundFXVolume";

    // Current values
    private float brightness = 1f;
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private bool isFullscreen = true;

    // Events that UI can subscribe to
    public System.Action<float> OnBrightnessChanged;
    public System.Action<float> OnMasterVolumeChanged;
    public System.Action<float> OnMusicVolumeChanged;
    public System.Action<float> OnSFXVolumeChanged;
    public System.Action<bool> OnFullscreenChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Try to load AudioMixer from Resources if not assigned
            if (audioMixer == null)
            {
                audioMixer = Resources.Load<AudioMixer>("Sounds/GlobalMixer");
                if (audioMixer == null)
                {
                    Debug.LogWarning("[SettingsManager] AudioMixer not assigned and could not be loaded from Resources/Sounds/GlobalMixer. Volume controls will not work!");
                }
                else
                {
                    Debug.Log("[SettingsManager] AudioMixer loaded from Resources/Sounds/GlobalMixer");
                }
            }

            LoadSettings();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        brightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        // Apply fullscreen setting
        Screen.fullScreen = isFullscreen;

        // Apply audio settings to mixer
        ApplyMasterVolumeToMixer(masterVolume);
        ApplyMusicVolumeToMixer(musicVolume);
        ApplySFXVolumeToMixer(sfxVolume);

        // Trigger brightness event so UI can update
        OnBrightnessChanged?.Invoke(brightness);
    }

    // Brightness
    public float Brightness
    {
        get => brightness;
        set
        {
            brightness = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(BRIGHTNESS_KEY, brightness);
            PlayerPrefs.Save();
            OnBrightnessChanged?.Invoke(brightness);
        }
    }

    // Master Volume
    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
            PlayerPrefs.Save();
            ApplyMasterVolumeToMixer(masterVolume);
            OnMasterVolumeChanged?.Invoke(masterVolume);
        }
    }

    // Music Volume
    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.Save();
            ApplyMusicVolumeToMixer(musicVolume);
            OnMusicVolumeChanged?.Invoke(musicVolume);
        }
    }

    // SFX Volume
    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
            ApplySFXVolumeToMixer(sfxVolume);
            OnSFXVolumeChanged?.Invoke(sfxVolume);
        }
    }

    // Fullscreen
    public bool IsFullscreen
    {
        get => isFullscreen;
        set
        {
            isFullscreen = value;
            PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
            Screen.fullScreen = isFullscreen;
            OnFullscreenChanged?.Invoke(isFullscreen);
        }
    }

    // AudioMixer control methods
    private void ApplyMasterVolumeToMixer(float level)
    {
        if (audioMixer == null) return;
        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;
        audioMixer.SetFloat(MASTER_PARAM, dbValue);
    }

    private void ApplyMusicVolumeToMixer(float level)
    {
        if (audioMixer == null) return;
        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;
        audioMixer.SetFloat(MUSIC_PARAM, dbValue);
    }

    private void ApplySFXVolumeToMixer(float level)
    {
        if (audioMixer == null) return;
        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;
        audioMixer.SetFloat(SFX_PARAM, dbValue);
    }
}
