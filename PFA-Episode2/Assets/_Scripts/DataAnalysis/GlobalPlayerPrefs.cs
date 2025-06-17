using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//#if DEVELOPMENT_BUILD || UNITY_EDITOR

public static class GlobalPlayerPrefs
{
    private static HashSet<string> _allFloatNames = new();
    private static HashSet<string> _allIntNames = new();
    
    public async static Task SetString(string valueName,string value,[CanBeNull] System.Net.Http.HttpClient client = null,bool showDebugLogs = false)
    {
        // bool newClient = client == null;
        // if (newClient) client = new();
        //
        // //cleanup value name
        // string cleanName = "";
        // foreach (char c in valueName)
        // {
        //     if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) cleanName += c;
        // }
        // string request = $"https://trmpnt.okman65.xyz/api/setValue/{cleanName}/{value}\n";
        //
        // //request
        // try
        // {
        //     using (HttpResponseMessage response = await client.GetAsync(request))
        //     {
        //         //if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
        //         response.EnsureSuccessStatusCode();
        //         
        //         string responseBody = await response.Content.ReadAsStringAsync();
        //         // Debug.Log(response.ReasonPhrase);
        //         // Debug.Log(response.StatusCode);
        //         // Debug.Log(response.RequestMessage.Headers);
        //         //if(showDebugLogs) Debug.Log("answer : " + responseBody);
        //     }
        // }catch(Exception e) {Debug.LogException(e);}
        //     
        //
        // if (newClient) client.Dispose();
    }
    
    public async static Task<string?> GetString(string ValueName,[CanBeNull] System.Net.Http.HttpClient client = null,bool  showDebugLogs = false)
    {
        // string? output = null;
        //
        // bool newClient = client == null;
        // if (newClient) client = new();
        //
        // //cleanup value name
        // string cleanName = "";
        // foreach (char c in ValueName)
        // {
        //     if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) cleanName += c;
        // }
        // string request = $"https://trmpnt.okman65.xyz/api/getValue/{cleanName}";
        //
        //
        // //request
        // try
        // {
        //     using (HttpResponseMessage response = await client.GetAsync(request))
        //     {
        //         //if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
        //         response.EnsureSuccessStatusCode();
        //
        //         string responseBody = await response.Content.ReadAsStringAsync();
        //
        //         //Debug.Log(response.ReasonPhrase);
        //         //Debug.Log(response.StatusCode);
        //         //Debug.Log(response.RequestMessage.Headers);
        //         //if(showDebugLogs) Debug.Log("answer : " + responseBody);
        //
        //         output = (responseBody);
        //     }
        // }catch(Exception e) {Debug.LogException(e);}
        //
        // if(newClient)client.Dispose();
        //     
        //
        // return output;
        return "";
    }
    
    public async static Task<int?> GetInt(string ValueName,[CanBeNull] HttpClient client = null)
    {
        // string answer = await GetString(ValueName,client);
        // int? output = null;
        // //Debug.Log(answer);
        // try
        // {
        //     if (int.TryParse(answer,out int result)) return result;
        //     else return null;
        // }
        // catch(Exception e)
        // {
        //     Debug.LogException(new Exception("c'est pas un int connard", e));
        //     return null;
        // }
        //
        // return output;
        return 0;
    }
    
    public async static Task<float?> GetFloat(string ValueName,[CanBeNull] HttpClient client = null)
    {
        // string answer = await GetString(ValueName,client);
        // float? output = null;
        //
        // try
        // {
        //     if (float.TryParse(answer,out float result)) return result;
        //     else return null;
        // }
        // catch(Exception e)
        // {
        //     Debug.LogException(new Exception("c'est pas un float connard", e));
        //     return null;
        // }
        //
        // return output;
        return 0;
    }

    /// <summary>
    /// supports ints, floats and strings.
    /// </summary>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <param name="client"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="Exception"></exception>
    public async static Task SetValue<T>(string valueName, T value,[CanBeNull] HttpClient client = null)
    {
        // switch (value)
        // {
        //     case int :
        //         await SetString(valueName, value.ToString(),client);
        //         _allIntNames.Add(valueName);
        //         break;
        //     case float:
        //         await SetString(valueName, value.ToString(),client);
        //         _allFloatNames.Add(valueName);
        //         break;
        //     case string : 
        //         await SetString(valueName, value.ToString(),client);
        //         break;
        //     default:
        //         throw new Exception( "Value type isn't supported.");
        // }
    }
    
    //menu items
    #if UNITY_EDITOR

    [MenuItem("Data / Attention / c'est pas pour toi / Tes vraiment sur ? / pitié / Delete All Global PlayerKeys / vraiment ? / pour de vrai ? / on pourra pas les récupérer après / je vais pleurer / Amen")]
    public static void DeleteAllGlobalPlayerKeys()
    {
        // ClearAllValues(null, true);
    }

    [MenuItem("Playtests Analysis /Editor Data/print all editor data")]
    public static async void PrintAllDevelopmentPlayerData()
    {
        // var dico = await GetGlobalPlaytestStats(false);
        // foreach (var pair in dico)
        // {
        //     //Debug.Log(pair.Key + " : " + pair.Value);
        // }
    }
    
    [MenuItem("Playtests Analysis /Build Data/print all build data")]
    public static async void PrintAllBuildPlayerData() 
    {
        // var dico = await GetGlobalPlaytestStats(true);
        // foreach (var pair in dico)
        // {
        //     //Debug.Log(pair.Key + " : " + pair.Value);
        // }
    }
    
    [MenuItem("Playtests Analysis /Editor Data/save all editor data to file")]
    public static void SaveAllDevelopmentPlayerDataToFile()
    {
        // throw new NotImplementedException();
        // GetGlobalPlaytestStats(false);
    }

    [MenuItem("Playtests Analysis /Build Data/save all build data to file")]
    public async static void SaveAllBuildPlayerDataToFile()
    {
//         //fetch data
//         SortedDictionary<string, string> entries = await GetGlobalPlaytestStats(true);
//         
//         //generate cells
//         SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, string>>> tableaux = new();
//         
//        /* foreach (var entry in entries)
//         {
//             string[] words = entry.Key.Split("_");
//             string tableau = words[0];
//             string ligne = words[1];
//             string colonne = entry.Key.Substring(tableau.Length+ligne.Length+2);
//
//             tableaux[tableau][ligne][colonne] = entry.Value;
//         }*/
//
//         
//         
//         // int nbTableaux = tableaux.Count;
//         // int nblignes = 0;
//         // foreach (var ligne in tableaux.Values)
//         // {
//         //     nblignes = Mathf.Max(nblignes,)
//         // }
//         string content = "sep=,\n";
//         string lastWord = "";
//         foreach (var entry in entries)
//         {
//             string[] words = entry.Key.Split("_");
//             string word = words[0];
//         
//             if (word != lastWord) content += ",\n" ;
//         
//             lastWord = word;
//             content += entry.Key + ',' + entry.Value.Replace(',','.') + '\n';
//         }
//         //
//         //write to file
//         string docPath = "Assets/_Data/Playtests/Build";
//         string docName = Tools.FormatPlaytestValueNameString("PlaytestData",false);
//         using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, docName+".csv")))
//         {
//             outputFile.Write(content);
//             //Debug.Log(content);
//             //Debug.Log(outputFile);
//         }
//         
    }
    
#endif
    
    public static async Task<SortedDictionary<string, string>> GetGlobalPlaytestStats(bool showBuildData = false)
    {
        // static async UniTask PullDataIntoDico(SortedDictionary<string, string> dico,string key,HttpClient client)
        // {
        //     string value = await GetString(key, client);
        //         
        //     string cleanKey = key;
        //     if (cleanKey.StartsWith("dev_")) cleanKey = cleanKey.Substring(4);
        //     if (cleanKey.StartsWith(Application.version)) cleanKey = cleanKey.Substring(Application.version.Length+1);
        //     cleanKey = cleanKey.RemoveConsecutiveCharacters('_');
        //
        //     dico[cleanKey] = value;
        // }
        // SortedDictionary<string, string> dico = new();
        //
        // using (HttpClient client = new())
        // {
        //     List<string> keys = (await FetchAllKeys(client, true)).ToList();
        //     keys.Sort();
        //
        //     List<UniTask> tasks = new();
        //     foreach (string key in keys)
        //     {
        //         if(key.StartsWith("dev_") == showBuildData) continue;
        //         if (key.EndsWith("hidden")) continue;
        //
        //         tasks.Add( PullDataIntoDico(dico,key,client));
        //
        //     }
        //
        //     await UniTask.WhenAll(tasks);
        //     
        // };
        // return dico;
        return new();
    }
    
    
    
    
    
    public static async Task<string[]> FetchAllKeys([CanBeNull] HttpClient client,bool showDebugLogs = false)
    {
        // bool newClient = client == null;
        // if (newClient) client = new();
        //
        // string request = $"http://trmpnt.okman65.xyz/api/getAllKeys";
        //
        // //request
        // try
        // {
        //     using (HttpResponseMessage response = await client.GetAsync(request))
        //     {
        //         //if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
        //         response.EnsureSuccessStatusCode();
        //         
        //         string responseBody = await response.Content.ReadAsStringAsync();
        //
        //         //if(showDebugLogs) Debug.Log("answer : " + responseBody);
        //         
        //         string[] requests = responseBody.Substring(2,responseBody.Length - 4).Split("\",\"");
        //         return requests;
        //     }
        // }catch(Exception e) {Debug.LogException(e);}
        //     
        //
        // if (newClient) client.Dispose();
        
        return Array.Empty<string>();
    }
    
    public static async Task ClearAllValues([CanBeNull] HttpClient client, bool showDebugLogs = true)
    {
        
        // bool newClient = client == null;
        // if (newClient) client = new();
        //
        // string request = $"http://trmpnt.okman65.xyz/api/clearAllEntries";
        //
        // //request
        // try
        // {
        //     using (HttpResponseMessage response = await client.GetAsync(request))
        //     {
        //         //if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
        //         response.EnsureSuccessStatusCode();
        //
        //         string responseBody = await response.Content.ReadAsStringAsync();
        //         
        //         //if(showDebugLogs) Debug.Log("answer : " + responseBody);
        //     }
        // }catch(Exception e) {Debug.LogException(e);}
        //
        // if(newClient)client.Dispose();
    }
    

}

//#endif