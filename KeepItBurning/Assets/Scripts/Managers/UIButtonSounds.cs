using UnityEngine;

/// <summary>
/// Simple script to play UI sounds for button hover and click events.
/// Attach this to UI buttons and hook up methods to EventTrigger or Button onClick.
/// </summary>
public class UIButtonSounds : MonoBehaviour
{
    /// <summary>
    /// Call this from EventTrigger: Pointer Enter
    /// </summary>
    public void PlayHoverSound()
    {
        SoundManager.Play(SoundAction.UiHover);
    }

    /// <summary>
    /// Call this from Button onClick or EventTrigger: Pointer Click
    /// </summary>
    public void PlayClickSound()
    {
        SoundManager.Play(SoundAction.UiClick);
    }

    /// <summary>
    /// Call this for back buttons
    /// </summary>
    public void PlayBackSound()
    {
        SoundManager.Play(SoundAction.UiClickBack);
    }
}
