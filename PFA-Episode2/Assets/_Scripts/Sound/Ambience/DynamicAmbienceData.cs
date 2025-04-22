using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAmbience",menuName = "Ambience")]
public class DynamicAmbienceData : ScriptableObject
{
    [field : SerializeField]
    public float MinDelay { get; private set; }

    [field : SerializeField]
    public float MaxDelay { get; private set; }

    [field : SerializeField]
    public List<AmbientSound> AmbientSounds { get; private set; }
}
