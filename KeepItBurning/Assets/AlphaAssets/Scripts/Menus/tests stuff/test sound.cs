using UnityEngine;
using UnityEngine.Audio;

public class testsound : MonoBehaviour
{
    [SerializeField] private AudioClip test;
    [SerializeField] private AudioMixer audioMixer;
    private const string SOUND_PARAM = "SoundFXVolume";

    public void PlaySound()
    {
        float dB;
        if (audioMixer.GetFloat(SOUND_PARAM, out dB))
        {
            float volume = Mathf.Pow(10f, dB / 20f);

            SoundEffectsManager.instance.PlaySoundEffects(test, transform, volume);
        }
    }
}
