using System;
using UnityEngine;

public class SoundEventsReceiver : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        //try
        //{
        //    Sounds sound = (Sounds)(int.Parse(parameters.Split("_")[0]));
        //    Debug.Log("CallSound " + sound);

        //    SFXManager.Instance.PlaySFXClip(sound);

        //}
        //catch (Exception e)
        //{
        //    Debug.LogException(e);
        //}

        try
        {
            SFXManager.Instance.PlaySFXClip(soundName);
        }
        catch (Exception e) { Debug.LogException(e); }
    }
}
