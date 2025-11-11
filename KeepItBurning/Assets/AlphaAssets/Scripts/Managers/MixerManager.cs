using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSounds;
    [SerializeField] private Slider sliderMaster;

    private const string MUSIC_PARAM = "MusicVolume";
    private const string SOUND_PARAM = "SoundFXVolume";
    private const string MASTER_PARAM = "MasterVolume";

    void Start()
    {
        if (audioMixer.GetFloat(MUSIC_PARAM, out float dbMusic))
            sliderMusic.value = Mathf.Pow(10f, dbMusic / 20f);
        sliderMusic.onValueChanged.AddListener(MusicVolume);
        MusicVolume(sliderMusic.value);

        if (audioMixer.GetFloat(SOUND_PARAM, out float dbSound))
            sliderSounds.value = Mathf.Pow(10f, dbSound / 20f);
        sliderSounds.onValueChanged.AddListener(SoundVolume);
        SoundVolume(sliderSounds.value);

        if (audioMixer.GetFloat(MASTER_PARAM, out float dbMaster))
            sliderMaster.value = Mathf.Pow(10f, dbMaster / 20f);
        sliderMaster.onValueChanged.AddListener(MasterVolume);
        MasterVolume(sliderMaster.value);
    }

    public void SoundVolume(float level)
    {
        if (level <= 0.0001f)
            audioMixer.SetFloat(SOUND_PARAM, -80f);
        else
            audioMixer.SetFloat(SOUND_PARAM, Mathf.Log10(level) * 20f);
    }

    public void MusicVolume(float level)
    {
        if (level <= 0.0001f)
            audioMixer.SetFloat(MUSIC_PARAM, -80f);
        else
            audioMixer.SetFloat(MUSIC_PARAM, Mathf.Log10(level) * 20f);
    }

    public void MasterVolume(float level)
    {
        if (level <= 0.0001f)
            audioMixer.SetFloat(MASTER_PARAM, -80f);
        else
            audioMixer.SetFloat(MASTER_PARAM, Mathf.Log10(level) * 20f);
    }
}
