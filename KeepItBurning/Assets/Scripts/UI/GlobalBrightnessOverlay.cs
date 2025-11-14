using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Global brightness overlay that persists across scenes and applies the brightness setting.
    /// This script creates its own Canvas and overlay automatically.
    /// Add this to the Managers prefab or any DontDestroyOnLoad object.
    /// </summary>
    public class GlobalBrightnessOverlay : MonoBehaviour
    {
        public static GlobalBrightnessOverlay Instance { get; private set; }
        private Canvas overlayCanvas;
        private CanvasGroup overlayPanel;

        public CanvasGroup OverlayPanel => overlayPanel;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateOverlay();
        }

        private void CreateOverlay()
        {
            // Create a new GameObject for the canvas
            GameObject canvasObj = new GameObject("GlobalBrightnessCanvas");
            canvasObj.transform.SetParent(transform);
            DontDestroyOnLoad(canvasObj);

            // Add Canvas component
            overlayCanvas = canvasObj.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 9999; // Very high to be on top

            // Add CanvasScaler for proper scaling
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add GraphicRaycaster (required for Canvas)
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create the overlay panel
            GameObject panelObj = new GameObject("BrightnessOverlay");
            panelObj.transform.SetParent(canvasObj.transform, false);

            // Make it full screen
            RectTransform rectTransform = panelObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            // Add Image component (black overlay)
            Image image = panelObj.AddComponent<Image>();
            image.color = Color.black;

            // Add CanvasGroup for alpha control
            overlayPanel = panelObj.AddComponent<CanvasGroup>();
            overlayPanel.interactable = false;
            overlayPanel.blocksRaycasts = false;
        }

        private void Start()
        {
            // Apply initial brightness from settings
            if (SettingsManager.Instance != null)
            {
                UpdateBrightness(SettingsManager.Instance.Brightness);
            }
        }

        private void OnEnable()
        {
            // Subscribe to brightness changes
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.OnBrightnessChanged += UpdateBrightness;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.OnBrightnessChanged -= UpdateBrightness;
            }
        }

        private void UpdateBrightness(float brightness)
        {
            if (overlayPanel != null)
            {
                // Higher brightness = less dark overlay
                // brightness 1.0 = alpha 0 (no overlay, full brightness)
                // brightness 0.0 = alpha 1 (full dark overlay, minimum brightness)
                overlayPanel.alpha = 1f - brightness;
            }
        }
    }
}
