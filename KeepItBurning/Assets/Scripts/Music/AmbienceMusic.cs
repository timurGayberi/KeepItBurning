using UnityEngine;

public class AmbienceMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Play(SoundAction.AmbienceMusic);
    }

}
