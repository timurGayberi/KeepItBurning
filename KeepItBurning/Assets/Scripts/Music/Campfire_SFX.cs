using UnityEngine;

public class Campfire_SFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.PlayAtPosition(SoundAction.CampfireSFX, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
