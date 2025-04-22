using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEventsReceiver : MonoBehaviour
{
    public void CallSound(string parameters)
    {
        try
        {
            Sounds sound = (Sounds)(int.Parse(parameters.Split("_")[0]));
            Debug.Log("CallSound " + sound);

            SFXManager.Instance.PlaySFXClip(sound);

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
