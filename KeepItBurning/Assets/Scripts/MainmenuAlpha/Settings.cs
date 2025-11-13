using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private CanvasGroup brightness;
    [SerializeField] private Slider brightnessSlider;

    private const string BRIGHTNESS_KEY = "BrightnessValue";

    void Start()
    {
        float savedBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);

        brightnessSlider.value = savedBrightness;
        brightness.alpha = 1f - savedBrightness;

        brightnessSlider.onValueChanged.AddListener(value =>
        {
            brightness.alpha = 1f - value;
            PlayerPrefs.SetFloat(BRIGHTNESS_KEY, value);
            PlayerPrefs.Save();
        });
    }
}
