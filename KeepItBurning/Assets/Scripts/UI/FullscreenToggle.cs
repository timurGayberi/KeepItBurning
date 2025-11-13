using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenCheckbox;

    private void Start()
    {
        // Get the toggle component if not assigned
        if (fullscreenCheckbox == null)
            fullscreenCheckbox = GetComponent<Toggle>();

        if (fullscreenCheckbox != null)
        {
            // Set initial state to match current fullscreen mode
            fullscreenCheckbox.isOn = Screen.fullScreen;

            // Add listener for when checkbox value changes
            fullscreenCheckbox.onValueChanged.AddListener(OnFullscreenToggled);
        }
    }

    private void OnDestroy()
    {
        // Clean up listener when object is destroyed
        if (fullscreenCheckbox != null)
        {
            fullscreenCheckbox.onValueChanged.RemoveListener(OnFullscreenToggled);
        }
    }

    /// <summary>
    /// Called when the checkbox value changes
    /// </summary>
    /// <param name="isFullscreen">True if checkbox is checked, false if unchecked</param>
    private void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    /// <summary>
    /// Public method to manually set fullscreen mode (can be called from UI buttons)
    /// </summary>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Update checkbox to match if it exists
        if (fullscreenCheckbox != null)
        {
            fullscreenCheckbox.isOn = isFullscreen;
        }
    }
}
