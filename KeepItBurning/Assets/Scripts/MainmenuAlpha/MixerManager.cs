using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Connects menu scene sliders to SettingsManager.
/// NOTE: This is a simplified version. SettingsManager now handles AudioMixer control directly.
/// Use SettingsUI instead for a more feature-complete solution that works in all scenes.
/// </summary>
public class MixerManager : MonoBehaviour
{
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSounds;
    [SerializeField] private Slider sliderMaster;

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
        if (SettingsManager.Instance == null)
        {
            Debug.LogError("[MixerManager] SettingsManager.Instance is null!");
            return;
        }

        Debug.Log($"[MixerManager] Initializing sliders with values - Master: {SettingsManager.Instance.MasterVolume}, Music: {SettingsManager.Instance.MusicVolume}, SFX: {SettingsManager.Instance.SFXVolume}");

        // Load current values from SettingsManager
        if (sliderMaster != null)
        {
            sliderMaster.value = SettingsManager.Instance.MasterVolume;
            sliderMaster.onValueChanged.AddListener(OnMasterSliderChanged);
            Debug.Log($"[MixerManager] Master slider initialized to {sliderMaster.value}");
        }
        else
        {
            Debug.LogWarning("[MixerManager] Master slider not assigned!");
        }

        if (sliderMusic != null)
        {
            sliderMusic.value = SettingsManager.Instance.MusicVolume;
            sliderMusic.onValueChanged.AddListener(OnMusicSliderChanged);
            Debug.Log($"[MixerManager] Music slider initialized to {sliderMusic.value}");
        }
        else
        {
            Debug.LogWarning("[MixerManager] Music slider not assigned!");
        }

        if (sliderSounds != null)
        {
            sliderSounds.value = SettingsManager.Instance.SFXVolume;
            sliderSounds.onValueChanged.AddListener(OnSoundSliderChanged);
            Debug.Log($"[MixerManager] SFX slider initialized to {sliderSounds.value}");
        }
        else
        {
            Debug.LogWarning("[MixerManager] SFX slider not assigned!");
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

    // Slider callbacks - forward to SettingsManager
    private void OnMasterSliderChanged(float value)
    {
        Debug.Log($"[MixerManager] Master slider changed to {value}");
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.MasterVolume = value;
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        Debug.Log($"[MixerManager] Music slider changed to {value}");
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.MusicVolume = value;
        }
    }

    private void OnSoundSliderChanged(float value)
    {
        Debug.Log($"[MixerManager] SFX slider changed to {value}");
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SFXVolume = value;
        }
    }

    // SettingsManager callbacks - update sliders if changed externally
    private void HandleMasterVolumeChanged(float value)
    {
        if (sliderMaster != null && !Mathf.Approximately(sliderMaster.value, value))
        {
            sliderMaster.SetValueWithoutNotify(value);
        }
    }

    private void HandleMusicVolumeChanged(float value)
    {
        if (sliderMusic != null && !Mathf.Approximately(sliderMusic.value, value))
        {
            sliderMusic.SetValueWithoutNotify(value);
        }
    }

    private void HandleSFXVolumeChanged(float value)
    {
        if (sliderSounds != null && !Mathf.Approximately(sliderSounds.value, value))
        {
            sliderSounds.SetValueWithoutNotify(value);
        }
    }
}
