using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSounds;
    [SerializeField] private Slider sliderMaster;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SOUND_PARAM = "SoundFXVolume";
    private const string MASTER_PARAM = "MasterVolume";

    private bool isInitialized = false;

    void Start()
    {
        InitializeSliders();
        SubscribeToSettingsManager();
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            SubscribeToSettingsManager();
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromSettingsManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromSettingsManager();
    }

    private void InitializeSliders()
    {
        // Load values from SettingsManager
        if (SettingsManager.Instance != null)
        {
            if (sliderMaster != null)
            {
                sliderMaster.value = SettingsManager.Instance.MasterVolume;
                sliderMaster.onValueChanged.AddListener(OnMasterSliderChanged);
            }

            if (sliderMusic != null)
            {
                sliderMusic.value = SettingsManager.Instance.MusicVolume;
                sliderMusic.onValueChanged.AddListener(OnMusicSliderChanged);
            }

            if (sliderSounds != null)
            {
                sliderSounds.value = SettingsManager.Instance.SFXVolume;
                sliderSounds.onValueChanged.AddListener(OnSoundSliderChanged);
            }

            // Apply initial values to mixer
            ApplyMasterVolume(SettingsManager.Instance.MasterVolume);
            ApplyMusicVolume(SettingsManager.Instance.MusicVolume);
            ApplySoundVolume(SettingsManager.Instance.SFXVolume);
        }
        else
        {
            // Fallback if SettingsManager doesn't exist
            if (sliderMaster != null)
            {
                sliderMaster.value = 1f;
                sliderMaster.onValueChanged.AddListener(OnMasterSliderChanged);
                ApplyMasterVolume(1f);
            }

            if (sliderMusic != null)
            {
                sliderMusic.value = 1f;
                sliderMusic.onValueChanged.AddListener(OnMusicSliderChanged);
                ApplyMusicVolume(1f);
            }

            if (sliderSounds != null)
            {
                sliderSounds.value = 1f;
                sliderSounds.onValueChanged.AddListener(OnSoundSliderChanged);
                ApplySoundVolume(1f);
            }
        }
    }

    private void SubscribeToSettingsManager()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnMasterVolumeChanged += HandleMasterVolumeChanged;
            SettingsManager.Instance.OnMusicVolumeChanged += HandleMusicVolumeChanged;
            SettingsManager.Instance.OnSFXVolumeChanged += HandleSFXVolumeChanged;
        }
    }

    private void UnsubscribeFromSettingsManager()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnMasterVolumeChanged -= HandleMasterVolumeChanged;
            SettingsManager.Instance.OnMusicVolumeChanged -= HandleMusicVolumeChanged;
            SettingsManager.Instance.OnSFXVolumeChanged -= HandleSFXVolumeChanged;
        }
    }

    private void OnMasterSliderChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.MasterVolume = value;
        }
        else
        {
            ApplyMasterVolume(value);
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.MusicVolume = value;
        }
        else
        {
            ApplyMusicVolume(value);
        }
    }

    private void OnSoundSliderChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SFXVolume = value;
        }
        else
        {
            ApplySoundVolume(value);
        }
    }

    private void HandleMasterVolumeChanged(float value)
    {
        if (sliderMaster != null && !Mathf.Approximately(sliderMaster.value, value))
        {
            sliderMaster.SetValueWithoutNotify(value);
        }
        ApplyMasterVolume(value);
    }

    private void HandleMusicVolumeChanged(float value)
    {
        if (sliderMusic != null && !Mathf.Approximately(sliderMusic.value, value))
        {
            sliderMusic.SetValueWithoutNotify(value);
        }
        ApplyMusicVolume(value);
    }

    private void HandleSFXVolumeChanged(float value)
    {
        if (sliderSounds != null && !Mathf.Approximately(sliderSounds.value, value))
        {
            sliderSounds.SetValueWithoutNotify(value);
        }
        ApplySoundVolume(value);
    }

    private void ApplySoundVolume(float level)
    {
        if (audioMixer == null) return;

        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;

        try
        {
            audioMixer.SetFloat(SOUND_PARAM, dbValue);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[MixerManager] Failed to set {SOUND_PARAM}. Make sure the parameter is exposed in the AudioMixer. Error: {e.Message}");
        }
    }

    private void ApplyMusicVolume(float level)
    {
        if (audioMixer == null) return;

        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;

        try
        {
            audioMixer.SetFloat(MUSIC_PARAM, dbValue);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[MixerManager] Failed to set {MUSIC_PARAM}. Make sure the parameter is exposed in the AudioMixer. Error: {e.Message}");
        }
    }

    private void ApplyMasterVolume(float level)
    {
        if (audioMixer == null) return;

        float dbValue = (level <= 0.0001f) ? -80f : Mathf.Log10(level) * 20f;

        try
        {
            audioMixer.SetFloat(MASTER_PARAM, dbValue);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[MixerManager] Failed to set {MASTER_PARAM}. Make sure the parameter is exposed in the AudioMixer. Error: {e.Message}");
        }
    }

    // Legacy public methods for backwards compatibility (if needed)
    [System.Obsolete("Use SettingsManager.Instance.SFXVolume instead")]
    public void SoundVolume(float level)
    {
        ApplySoundVolume(level);
    }

    [System.Obsolete("Use SettingsManager.Instance.MusicVolume instead")]
    public void MusicVolume(float level)
    {
        ApplyMusicVolume(level);
    }

    [System.Obsolete("Use SettingsManager.Instance.MasterVolume instead")]
    public void MasterVolume(float level)
    {
        ApplyMasterVolume(level);
    }
}
