using UnityEngine;

/// <summary>
/// Simple script to start main menu music.
/// Attach this to a GameObject in the Main Menu scene.
/// </summary>
public class MainMenuMusicStarter : MonoBehaviour
{
    private void Start()
    {
        // Play main menu music as a loop
        SoundManager.PlayLoop(SoundAction.MainMenuMusic);
        Debug.Log("[MainMenuMusicStarter] Playing MainMenuMusic");
    }
}
