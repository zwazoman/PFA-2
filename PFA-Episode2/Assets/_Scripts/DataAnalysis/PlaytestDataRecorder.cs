using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
//#if DEVELOPMENT_BUILD || UNITY_EDITOR
public class PlaytestDataRecorder : MonoBehaviour
{
    //game events
    public void OnRunStarted()
    {
        AddToInt("TotalRunStartedCount",1);
    }
        
    public void OnBossReached()
    {
        AddToInt("TotalBossReachedCount",1);
    }

    public void OnBossBeaten()
    {
        AddToInt("TotalBossBeatenCount",1);
    }

    public void OnGameOver()
    {
        AddToInt("TotalGameOverCount",1);
    }

    
    //scene events
    public void OnSceneOpened()
    {
        
    }

    public void OnSceneExited()
    {
        
    }

    
    
    //helper functions
    
    private async void AddToFloat(string valueName, float offset)
    {
        string name = Tools.FormatPlaytestValueNameString(valueName) ;
        
            float? currentValue = await GlobalPlayerPrefs.GetFloat(valueName);
            if (currentValue == null)
            {
                await GlobalPlayerPrefs.SetValue(valueName,offset);
            }
            else
            {
                currentValue += offset;
                await GlobalPlayerPrefs.SetValue(valueName,currentValue.Value);
            }
    }
    
    private async void AddToInt(string valueName, int offset)
    {
        string name = Tools.FormatPlaytestValueNameString(valueName) ;
        int? currentValue = await GlobalPlayerPrefs.GetInt(valueName);
        if (currentValue == null)
        {
            await GlobalPlayerPrefs.SetValue(valueName,offset);
        }
        else
        {
            currentValue += offset;
            await GlobalPlayerPrefs.SetValue(valueName,currentValue);
        }
    }
    
    private async UniTask UpdateAverage(string valueName,float valueToAdd)
    {
        try
        {
            //set up names
            string averageName = Tools.FormatPlaytestValueNameString(valueName) + "_average";
            string countName = Tools.FormatPlaytestValueNameString(valueName) + "_count";

            //fetch current global values
            float? currentAverage = await GlobalPlayerPrefs.GetFloat(averageName);
            int? currentCount = await GlobalPlayerPrefs.GetInt(countName);

            if ((currentAverage == null) != (currentCount == null)) throw (new Exception("y'a un big probleme là"));
            
            //update averages
            
            //si la valeur n'existe pas déjà
            if (currentAverage == null || currentCount == null)
            {
                currentAverage = valueToAdd;
                currentCount = 1;
            }
            else //sinon, si elle existait déjà : 
            {
                Tools.AccumulateAverage(ref currentAverage,ref currentCount,valueToAdd);
            }
            
            //update global values
            await GlobalPlayerPrefs.SetValue(averageName, currentAverage.Value);
            await GlobalPlayerPrefs.SetValue(countName, currentCount.Value);

        }catch (Exception e) {Debug.LogException(e);}

    }
}
//#endif