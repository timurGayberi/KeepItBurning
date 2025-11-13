using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GameControlsPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private RectTransform controlsPanel;
    [SerializeField] private CanvasGroup backgroundOverlay; // Optional: semi-transparent background

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Position Settings")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Bottom;

    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;
    private Coroutine slideCoroutine;
    private bool isVisible = false;
    private bool isAnimating = false;

    public enum SlideDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private void Awake()
    {
        if (controlsPanel == null)
            controlsPanel = GetComponent<RectTransform>();

        // Calculate positions
        visiblePosition = controlsPanel.anchoredPosition;
        CalculateHiddenPosition();

        // Start hidden
        controlsPanel.anchoredPosition = hiddenPosition;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Call this from the GameControlsButton's onClick event
    /// </summary>
    public void ShowPanel()
    {
        if (isVisible || isAnimating) return;

        gameObject.SetActive(true);

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideIn());
    }

    /// <summary>
    /// Hide the panel with slide-out animation
    /// </summary>
    public void HidePanel()
    {
        if (!isVisible || isAnimating) return;

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideOut());
    }

    /// <summary>
    /// Toggle panel visibility
    /// </summary>
    public void TogglePanel()
    {
        if (isVisible)
            HidePanel();
        else
            ShowPanel();
    }

    private void Update()
    {
        // Check for click anywhere to close (only when visible and not animating)
        if (isVisible && !isAnimating)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                // Check if we clicked outside the panel
                if (!IsPointerOverPanel())
                {
                    HidePanel();
                }
            }
        }
    }

    private bool IsPointerOverPanel()
    {
        // Check if mouse/touch is over the controls panel
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            // Check if we hit the controls panel or any of its children
            if (result.gameObject.transform.IsChildOf(controlsPanel.transform) ||
                result.gameObject.transform == controlsPanel.transform)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator SlideIn()
    {
        isAnimating = true;
        isVisible = false;

        // Fade in background if exists
        if (backgroundOverlay != null)
        {
            backgroundOverlay.alpha = 0f;
            backgroundOverlay.gameObject.SetActive(true);
        }

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);

            controlsPanel.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, t);

            if (backgroundOverlay != null)
                backgroundOverlay.alpha = t * 0.5f; // 50% opacity background

            yield return null;
        }

        controlsPanel.anchoredPosition = visiblePosition;
        if (backgroundOverlay != null)
            backgroundOverlay.alpha = 0.5f;

        isVisible = true;
        isAnimating = false;
    }

    private IEnumerator SlideOut()
    {
        isAnimating = true;

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);

            controlsPanel.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, t);

            if (backgroundOverlay != null)
                backgroundOverlay.alpha = (1f - t) * 0.5f;

            yield return null;
        }

        controlsPanel.anchoredPosition = hiddenPosition;

        if (backgroundOverlay != null)
        {
            backgroundOverlay.alpha = 0f;
            backgroundOverlay.gameObject.SetActive(false);
        }

        isVisible = false;
        isAnimating = false;
        gameObject.SetActive(false);
    }

    private void CalculateHiddenPosition()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Rect rect = controlsPanel.rect;

        switch (slideDirection)
        {
            case SlideDirection.Bottom:
                hiddenPosition = new Vector2(visiblePosition.x, -canvasRect.rect.height / 2 - rect.height);
                break;
            case SlideDirection.Top:
                hiddenPosition = new Vector2(visiblePosition.x, canvasRect.rect.height / 2 + rect.height);
                break;
            case SlideDirection.Left:
                hiddenPosition = new Vector2(-canvasRect.rect.width / 2 - rect.width, visiblePosition.y);
                break;
            case SlideDirection.Right:
                hiddenPosition = new Vector2(canvasRect.rect.width / 2 + rect.width, visiblePosition.y);
                break;
        }
    }

    private void OnValidate()
    {
        if (controlsPanel == null)
            controlsPanel = GetComponent<RectTransform>();

        visiblePosition = controlsPanel.anchoredPosition;
        CalculateHiddenPosition();
    }
}
