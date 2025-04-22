using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAmbienceManager : MonoBehaviour
{
    #region Singleton
    private static DynamicAmbienceManager instance;

    public static DynamicAmbienceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Ambience Manager");
                instance = go.AddComponent<DynamicAmbienceManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion

    DynamicAmbienceData _currentAmb;

    public void StartAmbience(DynamicAmbienceData newAmb )
    {
        StopCoroutine(AmbiencePlayer(_currentAmb));
        StartCoroutine(AmbiencePlayer(newAmb));
    }

    private IEnumerator AmbiencePlayer(DynamicAmbienceData amb)
    {
        _currentAmb = amb;

        List<AudioClip> sounds = new List<AudioClip>();

        foreach(AmbientSound ambientSound in amb.AmbientSounds)
        {
            sounds.Add(ambientSound.Sound);
        }

        while (true)
        {
            Vector3 soundPlayPos = ChooseRandomSpotAround();

            SFXManager.Instance.PlaySFXClipAtPosition(sounds[Random.Range(0, sounds.Count)], ChooseRandomSpotAround());

            yield return new WaitForSeconds(Random.Range(amb.MinDelay, amb.MaxDelay));
        }
    }

    private Vector3 ChooseRandomSpotAround()
    {
        Vector3 randomPos = Random.insideUnitSphere;

        if (randomPos.y < 0) randomPos.y *= -1;

        return randomPos;
    }
}
