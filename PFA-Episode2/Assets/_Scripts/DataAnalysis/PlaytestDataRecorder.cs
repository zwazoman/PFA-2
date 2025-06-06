using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

//#if DEVELOPMENT_BUILD || UNITY_EDITOR
public class PlaytestDataRecorder : MonoBehaviour
{
    private static Dictionary<string, float> _mapTotalLengths = new();
    private static Dictionary<string, int> _mapOccurencesPerRun = new();
    
    //singleton
    public static PlaytestDataRecorder Instance { get; private set; }

    private HttpClient _client;
    private HttpClient client
    {
        get { return _client ??= new HttpClient(); }
    }

    private void OnApplicationQuit()
    {
        if(_client!=null)_client.Dispose();
    }

    public void CallOnRunStarted()
    {
        OnRunStarted();
    }

    private void Awake()
    {
        Instance = this;
        if(Time.time<.2f)
        { 
            _ = OnGameLauched();
        }
    }
    
    //game events
    public async UniTask OnGameLauched()
    {
        _mapTotalLengths.Clear();
        _mapOccurencesPerRun.Clear();
        await AddToInt("Progression_GameLaunch_TotalCount",1);
    }

    public async UniTask OnRunStarted()
    {
        CombatManager.TotalEncounteredCombatsCountOverRun = 0;
        await AddToInt("Progression_RunStarted_TotalCount",1);
    }
        
    //todo : appeler ces méthodes
    public async UniTask OnBossReached()
    {
        await UpdateFullDataAtEvent("Progression_BossReached");
    }

    public async UniTask OnBossBeaten()
    {
        List<UniTask> tasks = new();
        tasks.Add( AddToInt("Progression_BossBeaten_TotalCount",1));
        tasks.Add( UpdateAverage("Progression_BossBeaten_Time",Time.time));
        await UniTask.WhenAll(tasks);
    }

    public async UniTask OnGameOver()
    {
        await UpdateFullDataAtEvent("GameOver");
    }
    
    //scene events
    public async UniTask OnSceneOpened()
    {
        _mapTotalLengths.TryAdd(SceneManager.GetActiveScene().name, 0);
        _mapOccurencesPerRun.TryAdd(SceneManager.GetActiveScene().name,1);

        //capture average fps
        await UniTask.WaitForSeconds(1);
        float fpsSum = 0;
        for(int i = 0;i<20;i++)
        {
            fpsSum += 1f / Time.deltaTime;
            await UniTask.WaitForSeconds(.2f);
        }
        float averageFps = fpsSum / 20f;
        await UpdateAverage("Map_"+SceneManager.GetActiveScene().name+"_FPS", averageFps);

    }
    public async UniTask OnSceneExited()
    {
        await UpdateAverage("Map_"+ SceneManager.GetActiveScene().name + "_Duration",Time.timeSinceLevelLoad);
        
        _mapTotalLengths[SceneManager.GetActiveScene().name] += Time.timeSinceLevelLoad;
        _mapOccurencesPerRun[SceneManager.GetActiveScene().name] ++;
    }

    //helper local functions
    private async UniTask UpdateFullDataAtEvent(string eventName)
    {
        Debug.Log(" ===== about to update data for event : " + eventName + " =====");

        List<UniTask> tasks = new();
        eventName = "Event_" + eventName;
        tasks.Add(  AddToInt(eventName + "_TotalOccurrences",1));
        tasks.Add(  UpdateAverage(eventName +"_ReachedAt", Time.time));
        
        tasks.Add(  UpdateAverage(eventName + "_Inventory_RemainingIngredientsCount", GameManager.Instance.playerInventory.Ingredients.Count));
        tasks.Add(  UpdateAverage(eventName + "_Inventory_SpellCount", GameManager.Instance.playerInventory.Spells.Count));
        
        tasks.Add(  UpdateAverage(eventName + "_CombatsOccurences", CombatManager.TotalEncounteredCombatsCountOverRun));

        foreach (string key in _mapTotalLengths.Keys)
        {
            tasks.Add(  UpdateAverage(eventName + "_Map_TotalSpentTimeOnMap_"+key, _mapTotalLengths[key]));
            tasks.Add(  UpdateAverage(eventName + "_Map_MapOccurrences_"+key, _mapOccurencesPerRun[key]));
        }

        await UniTask.WhenAll(tasks);
    }
    
    //helper network functions
    private async UniTask AddToFloat(string valueName, float offset)
    {
        string name = Tools.FormatPlaytestValueNameString(valueName) ;
        
            float? currentValue = await GlobalPlayerPrefs.GetFloat(valueName);
            if (currentValue == null)
            {
                await GlobalPlayerPrefs.SetValue(valueName,offset,client);
            }
            else
            {
                currentValue += offset;
                await GlobalPlayerPrefs.SetValue(valueName,currentValue.Value,client);
            }
    }
    
    private async UniTask AddToInt(string valueName, int offset)
    {
        string name = Tools.FormatPlaytestValueNameString(valueName) ;
        int? currentValue = await GlobalPlayerPrefs.GetInt(valueName);
        if (currentValue == null)
        {
            await GlobalPlayerPrefs.SetValue(name,offset,client);
        }
        else
        {
            currentValue += offset;
            await GlobalPlayerPrefs.SetValue(name,currentValue,client);
        }
        
    }
    
    private async UniTask UpdateAverage(string valueName,float valueToAdd)
    {
        try
        {
            //set up names
            string averageName = Tools.FormatPlaytestValueNameString(valueName) + "__Average";
            string countName = Tools.FormatPlaytestValueNameString(valueName) + "__Count__hidden";

            //fetch current global values
            float? currentAverage = await GlobalPlayerPrefs.GetFloat(averageName,client);
            int? currentCount = await GlobalPlayerPrefs.GetInt(countName,client);

            if ((currentAverage == null) != (currentCount == null)) Debug.LogException( (new Exception("y'a un big probleme là")));
            
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
            await GlobalPlayerPrefs.SetValue(averageName, currentAverage.Value,client);
            await GlobalPlayerPrefs.SetValue(countName, currentCount.Value,client);

        }catch (Exception e) {Debug.LogException(e);}

    }
}
//#endif