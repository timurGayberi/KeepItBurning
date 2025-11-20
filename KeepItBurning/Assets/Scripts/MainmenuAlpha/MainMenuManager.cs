using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Interfaces;   
using General;
using TMPro;

public class MainMenuManager : MonoBehaviour
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
    
    [Space]
    
    //  --- Added for players NickName Submit Function ---
    [Header("Nickname UI")]
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField nicknameInputField;
    
    [SerializeField] private Button nicknameSubmitButton;
    [SerializeField] private Button skipButton;
    
    [SerializeField] private TextMeshProUGUI nicknameStatusText;
    [SerializeField] private TextMeshProUGUI welcomeText;

    [SerializeField] private Image clipboardImage;

    public bool NickNameSubmitted {get; private set;}
    
    // --- Service Subscribe ---
    private IPlayFabService _playFabService;
    
    
    private void Start()
    {
        try
        {
            _playFabService = ServiceLocator.GetService<IPlayFabService>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PlayFab Service Not Found: {e.Message}");
        }
        
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(false);
        }
        
        if (nicknameSubmitButton != null)
        {
            nicknameSubmitButton.onClick.RemoveAllListeners();
            nicknameSubmitButton.onClick.AddListener(SubmitNicknameAndStartGame);
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(SkipNicknameAndStartGame);
        }
    }
    
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
        // --- Commented out due test ----
        
        /*
        DisableAllCameras();
        DisableAllButtons();
        LeaderboardCamera.SetActive(true);
        */
        
        // --- test
        
        
        DisableAllCameras();
        DisableAllButtons();
        
        if (_playFabService != null)
        {
            _playFabService.RetrieveLeaderboard(); 
            
            Debug.Log("Leaderboard data request sent. Waiting for PlayFab response...");
        }
        else
        {
            Debug.LogError("Cannot retrieve leaderboard: PlayFab Service not found.");
        }
        
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
        // --- This Function changed by Timur to make player write their nickname before game ---
        
        DisableAllCameras();
        DisableAllButtons();
        
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(true);
            clipboardImage.gameObject.SetActive(true);
            nicknameStatusText.text = "Sign your name";
        }
        
        // --- Previous version ---
        //SceneManager.LoadScene("GameScene");
    }
    
    // --- This methods added by Timur for players nickname submit functions 
    
    private void SubmitNicknameAndStartGame()
    {
        if (_playFabService == null)
        {
            nicknameStatusText.text = "Error: PlayFab Service not initialized!";
            return;
        }

        string nickname = nicknameInputField.text.Trim();

        if (string.IsNullOrEmpty(nickname) || nickname.Length < 3)
        {
            nicknameStatusText.text = "Nickname must be at least 3 characters.";
            return;
        }
        
        StartCoroutine(HandleNicknameSubmissionRoutine(nickname));
    }
    
    private void SkipNicknameAndStartGame()
    {
        Debug.Log("Skipping nickname submission. Starting game...");
        
        NickNameSubmitted = false;
        
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(false);
        }

        clipboardImage.gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }
    
    private IEnumerator HandleNicknameSubmissionRoutine(string nickname)
    {
        nicknameSubmitButton.interactable = false;
        nicknameStatusText.text = "Submitting...";
        
        _playFabService.SaveNickname(nickname); 
        
        yield return new WaitForSeconds(1.5f); 
        
        NickNameSubmitted = true;
        
        nicknameStatusText.text = "Submission successful! Starting game...";
        
        if (nicknamePanel != null)
        {
            nicknamePanel.SetActive(false);
        }

        clipboardImage.gameObject.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }
    
    // --- End of Changes ---
    
    
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