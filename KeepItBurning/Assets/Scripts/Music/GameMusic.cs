using UnityEngine;

public class GameMusic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Play(SoundAction.GameMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
