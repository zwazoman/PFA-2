using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicParameters
{
    [SerializeField] public List<AudioClip> musics;
    [SerializeField, Range(0, 1)] public float targetVolume;
}
