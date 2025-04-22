using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    #region Singleton
    private static AudioMixerManager instance = null;
    public static AudioMixerManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioMixer Manager");
                instance = go.AddComponent<AudioMixerManager>();
            }
            return instance;
        }
    }
    #endregion

    AudioMixer _mixer;

    public const string Mixer_MasterVolume = "MasterVolume";
    public const string Mixer_MusicVolume = "MusicVolume";
    public const string Mixer_SFXVolume = "SFXVolume";
    public const string Mixer_AmbienceVolume = "AmbienceVolume";
    public const string Mixer_VoicelinesVolume = "VoicelinesVolume";

    

    public void SetVolume(string group, float value)
    {
        _mixer.SetFloat(Mixer_SFXVolume, Mathf.Log10(value) * 20);
    }
}
