using UnityEngine;

namespace General
{
    [RequireComponent(typeof(RectTransform))]
    public class PanelAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Slide Settings")]
        [Tooltip("Direction the panel slides from when appearing")]
        [SerializeField] private SlideDirection slideDirection = SlideDirection.Bottom;

        [Header("Content References")]
        [Tooltip("The content that will slide (leave empty to slide entire panel)")]
        [SerializeField] private RectTransform slidingContent;

        [Tooltip("Background element that will fade instead of slide")]
        [SerializeField] private CanvasGroup backgroundCanvasGroup;

        private RectTransform rectTransform;
        private Vector2 visiblePosition;
        private Vector2 hiddenPosition;
        private bool isAnimating = false;
        private float animationProgress = 0f;
        private bool targetVisible = false;

        public enum SlideDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            // If no sliding content specified, use the panel itself
            if (slidingContent == null)
                slidingContent = rectTransform;

            visiblePosition = slidingContent.anchoredPosition;
            CalculateHiddenPosition();
        }

        private void OnEnable()
        {
            if (slidingContent != null)
            {
                slidingContent.anchoredPosition = hiddenPosition;
                if (backgroundCanvasGroup != null)
                    backgroundCanvasGroup.alpha = 0f;
                AnimateToVisible(true);
            }
        }

        private void Update()
        {
            if (isAnimating)
            {
                animationProgress += Time.unscaledDeltaTime / animationDuration;

                if (animationProgress >= 1f)
                {
                    animationProgress = 1f;
                    isAnimating = false;

                    if (!targetVisible)
                    {
                        gameObject.SetActive(false);
                    }
                }

                float t = animationCurve.Evaluate(animationProgress);

                // Animate the sliding content
                if (targetVisible)
                {
                    slidingContent.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, t);
                }
                else
                {
                    slidingContent.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, t);
                }

                // Fade the background
                if (backgroundCanvasGroup != null)
                {
                    backgroundCanvasGroup.alpha = targetVisible ? t : 1f - t;
                }
            }
        }

        public void AnimateToVisible(bool visible)
        {
            targetVisible = visible;
            animationProgress = 0f;
            isAnimating = true;

            if (visible && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            AnimateToVisible(false);
        }

        public void Show()
        {
            AnimateToVisible(true);
        }

        private void CalculateHiddenPosition()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Rect rect = slidingContent.rect;

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
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (slidingContent == null)
                slidingContent = rectTransform;

            visiblePosition = slidingContent.anchoredPosition;
            CalculateHiddenPosition();
        }
    }
}
