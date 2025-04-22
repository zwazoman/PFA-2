using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[Serializable]
public class AmbientSound
{
    [field : SerializeField] 
    public AudioClip Sound { get; private set; }

    [ field : SerializeField, Range(0, 1)]
    public float Volume { get; private set; }

    [field : SerializeField, Range(0,1)]
    public float Pitch { get; private set; }

    //[field: SerializeField, Range(0, 1)]
    //public float Rate { get; private set; }

}
