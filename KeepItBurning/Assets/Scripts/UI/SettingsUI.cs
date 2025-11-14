using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Connects UI elements (sliders, toggles) to the SettingsManager.
/// Attach this to settings panel in both Main Menu and Game Scene. //Ramona
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Brightness")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private CanvasGroup brightnessOverlay; 

    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Fullscreen")]
    [SerializeField] private Toggle fullscreenToggle;

    private bool isInitializing = false;

    private void Start()
    {
        InitializeUI();
        RegisterListeners();
    }

    private void OnDestroy()
    {
        UnregisterListeners();
    }

    private void InitializeUI()
    {
        isInitializing = true;

        // Set UI to match current settings
        if (brightnessSlider != null)
        {
            brightnessSlider.value = SettingsManager.Instance.Brightness;

            // Only update local overlay if it exists (optional, global overlay handles it)
            if (brightnessOverlay != null)
            {
                UpdateBrightnessOverlay(SettingsManager.Instance.Brightness);
            }
        }

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = SettingsManager.Instance.MasterVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = SettingsManager.Instance.MusicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = SettingsManager.Instance.SFXVolume;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = SettingsManager.Instance.IsFullscreen;

        isInitializing = false;
    }

    private void RegisterListeners()
    {
        // UI -> SettingsManager
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeSliderChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        // SettingsManager -> UI (in case settings change from another UI instance)
        SettingsManager.Instance.OnBrightnessChanged += OnBrightnessChangedFromManager;
        SettingsManager.Instance.OnMasterVolumeChanged += OnMasterVolumeChangedFromManager;
        SettingsManager.Instance.OnMusicVolumeChanged += OnMusicVolumeChangedFromManager;
        SettingsManager.Instance.OnSFXVolumeChanged += OnSFXVolumeChangedFromManager;
        SettingsManager.Instance.OnFullscreenChanged += OnFullscreenChangedFromManager;
    }

    private void UnregisterListeners()
    {
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.RemoveListener(OnBrightnessSliderChanged);

        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeSliderChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeSliderChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeSliderChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggleChanged);

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnBrightnessChanged -= OnBrightnessChangedFromManager;
            SettingsManager.Instance.OnMasterVolumeChanged -= OnMasterVolumeChangedFromManager;
            SettingsManager.Instance.OnMusicVolumeChanged -= OnMusicVolumeChangedFromManager;
            SettingsManager.Instance.OnSFXVolumeChanged -= OnSFXVolumeChangedFromManager;
            SettingsManager.Instance.OnFullscreenChanged -= OnFullscreenChangedFromManager;
        }
    }

    // UI changed by user
    private void OnBrightnessSliderChanged(float value)
    {
        if (isInitializing) return;
        SettingsManager.Instance.Brightness = value;

        // Update local overlay if it exists (global overlay updates automatically via event)
        if (brightnessOverlay != null)
        {
            UpdateBrightnessOverlay(value);
        }
    }

    private void OnMasterVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        SettingsManager.Instance.MasterVolume = value;
    }

    private void OnMusicVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        SettingsManager.Instance.MusicVolume = value;
    }

    private void OnSFXVolumeSliderChanged(float value)
    {
        if (isInitializing) return;
        SettingsManager.Instance.SFXVolume = value;
    }

    private void OnFullscreenToggleChanged(bool value)
    {
        if (isInitializing) return;
        SettingsManager.Instance.IsFullscreen = value;
    }

    // Settings changed from another UI instance - sync this UI
    private void OnBrightnessChangedFromManager(float value)
    {
        if (brightnessSlider != null && Mathf.Abs(brightnessSlider.value - value) > 0.01f)
        {
            brightnessSlider.value = value;

            // Update local overlay if it exists
            if (brightnessOverlay != null)
            {
                UpdateBrightnessOverlay(value);
            }
        }
    }

    private void OnMasterVolumeChangedFromManager(float value)
    {
        if (masterVolumeSlider != null && Mathf.Abs(masterVolumeSlider.value - value) > 0.01f)
            masterVolumeSlider.value = value;
    }

    private void OnMusicVolumeChangedFromManager(float value)
    {
        if (musicVolumeSlider != null && Mathf.Abs(musicVolumeSlider.value - value) > 0.01f)
            musicVolumeSlider.value = value;
    }

    private void OnSFXVolumeChangedFromManager(float value)
    {
        if (sfxVolumeSlider != null && Mathf.Abs(sfxVolumeSlider.value - value) > 0.01f)
            sfxVolumeSlider.value = value;
    }

    private void OnFullscreenChangedFromManager(bool value)
    {
        if (fullscreenToggle != null && fullscreenToggle.isOn != value)
            fullscreenToggle.isOn = value;
    }

    private void UpdateBrightnessOverlay(float brightness)
    {
        // Use the global overlay if available, otherwise use local one
        var overlayToUpdate = brightnessOverlay;

        if (overlayToUpdate == null && UI.GlobalBrightnessOverlay.Instance != null)
        {
            overlayToUpdate = UI.GlobalBrightnessOverlay.Instance.OverlayPanel;
        }

        if (overlayToUpdate != null)
        {
            overlayToUpdate.alpha = 1f - brightness;
        }
    }
}
