using UnityEngine;
using System.Collections;

public class ClipboardMenuController : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private CanvasGroup clipboardMainMenu;
    [SerializeField] private CanvasGroup clipboardSettings;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;
    private bool isOnSettingsMenu = false;

    public static ClipboardMenuController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Initialize: Show main menu, hide settings
        if (clipboardMainMenu != null)
        {
            clipboardMainMenu.alpha = 1f;
            clipboardMainMenu.gameObject.SetActive(true);
            clipboardMainMenu.interactable = true;
            clipboardMainMenu.blocksRaycasts = true;
        }

        if (clipboardSettings != null)
        {
            clipboardSettings.alpha = 0f;
            clipboardSettings.gameObject.SetActive(false);
            clipboardSettings.interactable = false;
            clipboardSettings.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Call this method from the settings button's onClick event
    /// </summary>
    public void ShowSettingsMenu()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(TransitionToSettings());
    }

    /// <summary>
    /// Call this method to return to the main menu.
    /// Connect this to the BackButton's onClick event in ClipboardSettings.
    /// </summary>
    public void ShowMainMenu()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(TransitionToMainMenu());
    }

    /// <summary>
    /// Alternative name for ShowMainMenu() - for clarity when using with back button.
    /// Connect this to the BackButton's onClick event in ClipboardSettings.
    /// </summary>
    public void OnBackButtonClicked()
    {
        ShowMainMenu();
    }

    private IEnumerator TransitionToSettings()
    {
        // Instantly hide main menu and show settings
        HideMenuInstant(clipboardMainMenu);
        ShowMenuInstant(clipboardSettings);
        isOnSettingsMenu = true;
        yield break;
    }

    private IEnumerator TransitionToMainMenu()
    {
        // Instantly hide settings and show main menu
        HideMenuInstant(clipboardSettings);
        ShowMenuInstant(clipboardMainMenu);
        isOnSettingsMenu = false;
        yield break;
    }

    /// <summary>
    /// Check if currently on the settings menu
    /// </summary>
    public bool IsOnSettingsMenu()
    {
        return isOnSettingsMenu;
    }

    private void HideMenuInstant(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.gameObject.SetActive(false);
    }

    private void ShowMenuInstant(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;

        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
