using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Clip
{
    [SerializeField]
    public List<AudioClip> ClipList = new List<AudioClip>();

    [SerializeField, Range(0, 1)]
    public float Volume = 1;

    [SerializeField, Range(0,1)]
    public float Pitch = 1;

    [SerializeField]
    public RandomType RandomType = RandomType.NoDuplicateRandom;

    [SerializeField]
    public AudioMixerGroup MixerGroup;

    [HideInInspector]
    public List<AudioClip> tempClipList = new List<AudioClip>();

    [HideInInspector]
    public AudioClip tempClip;
}

