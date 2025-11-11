using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuManagerTest: MonoBehaviour
{
    [SerializeField] private CanvasGroup playScreenCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;
    //Cameras
    [SerializeField] private GameObject BoardCamera;
    [SerializeField] private GameObject LeaderboardCamera;
    [SerializeField] private GameObject SettingsCamera;
    [SerializeField] private GameObject ControlsCamera;
    [SerializeField] private GameObject HowToPlayCamera;
    //Buttons
    [SerializeField] private GameObject PlayButton;
    [SerializeField] private GameObject LeaderboardButton;
    [SerializeField] private GameObject SettingsButton;
    [SerializeField] private GameObject ControlsButton;
    [SerializeField] private GameObject HowToPlayButton;
    //Light
    [SerializeField] private Light BoardLight;
    [SerializeField] private float lightFadeDuration = 1f;
    [SerializeField] private float maxLightIntensity = 1f;

    private Coroutine lightFadeCoroutine;

    public void ClickAnywhereToStart()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            playScreenCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        playScreenCanvasGroup.alpha = 0f;
        playScreenCanvasGroup.gameObject.SetActive(false);
    }
    private void DisableAllCameras()
    {   
        BoardCamera.SetActive(false);
        LeaderboardCamera.SetActive(false);
        SettingsCamera.SetActive(false);
        ControlsCamera.SetActive(false);
        HowToPlayCamera.SetActive(false);
    }
    private void DisableAllButtons()
    {   
        FadeLight(false);
        PlayButton.SetActive(false);
        LeaderboardButton.SetActive(false);
        SettingsButton.SetActive(false);
        ControlsButton.SetActive(false);
        HowToPlayButton.SetActive(false);
    }
    private void EnableAllButtons()
    {   
        FadeLight(true);
        PlayButton.SetActive(true);
        LeaderboardButton.SetActive(true);
        SettingsButton.SetActive(true);
        ControlsButton.SetActive(true);
        HowToPlayButton.SetActive(true);
    }
    public void GoToLeaderboard()
    {
        DisableAllCameras();
        DisableAllButtons();
        LeaderboardCamera.SetActive(true);
    }

    public void GoToSettings()
    {
        DisableAllCameras();
        DisableAllButtons();
        SettingsCamera.SetActive(true);
    }
    public void GoToControls()
    {
        DisableAllCameras();
        DisableAllButtons();
        ControlsCamera.SetActive(true);
    }
    public void GoToHowToPlay()
    {
        DisableAllCameras();
        DisableAllButtons();
        HowToPlayCamera.SetActive(true);
    }

    public void GoToMainMenu()
    {
        DisableAllCameras();
        EnableAllButtons();
        BoardCamera.SetActive(true);
    }

    public void Play()
    {
        SceneManager.LoadScene("AlphaScene");
    }
    private void FadeLight(bool turnOn)
    {
        if (BoardLight == null) return;

        if (lightFadeCoroutine != null)
        StopCoroutine(lightFadeCoroutine);

        if (turnOn && !BoardLight.enabled)
        {
            BoardLight.enabled = true;
            BoardLight.intensity = 0f;
        }

        lightFadeCoroutine = StartCoroutine(FadeLightRoutine(turnOn));
    }

    private IEnumerator FadeLightRoutine(bool turnOn)
    {
        if (BoardLight == null)
            yield break;

        if (turnOn)
        {
            if (!BoardLight.enabled)
            {
                BoardLight.enabled = true;
                BoardLight.intensity = 0f; 
                yield return null; 
            }
        }
    
        float startIntensity = BoardLight.intensity;
        float targetIntensity = turnOn ? maxLightIntensity : 0f;
        float elapsed = 0f;

        while (elapsed < lightFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lightFadeDuration);
            BoardLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        BoardLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, 1f);

        if (!turnOn)
            BoardLight.enabled = false;
    }
}