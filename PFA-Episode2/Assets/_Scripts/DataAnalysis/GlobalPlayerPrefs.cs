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
        bool newClient = client == null;
        if (newClient) client = new();
        
        //cleanup value name
        string cleanName = "";
        foreach (char c in valueName)
        {
            if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) cleanName += c;
        }
        string request = $"https://trmpnt.okman65.xyz/api/setValue/{cleanName}/{value}\n";
        
        //request
        try
        {
            using (HttpResponseMessage response = await client.GetAsync(request))
            {
                if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                // Debug.Log(response.ReasonPhrase);
                // Debug.Log(response.StatusCode);
                // Debug.Log(response.RequestMessage.Headers);
                if(showDebugLogs) Debug.Log("answer : " + responseBody);
            }
        }catch(Exception e) {Debug.LogException(e);}
            
        
        if (newClient) client.Dispose();
    }
    
    public async static Task<string?> GetString(string ValueName,[CanBeNull] System.Net.Http.HttpClient client = null,bool  showDebugLogs = false)
    {
        string? output = null;
        
        bool newClient = client == null;
        if (newClient) client = new();
        
        //cleanup value name
        string cleanName = "";
        foreach (char c in ValueName)
        {
            if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) cleanName += c;
        }
        string request = $"https://trmpnt.okman65.xyz/api/getValue/{cleanName}";
        
        
        //request
        try
        {
            using (HttpResponseMessage response = await client.GetAsync(request))
            {
                if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                //Debug.Log(response.ReasonPhrase);
                //Debug.Log(response.StatusCode);
                //Debug.Log(response.RequestMessage.Headers);
                if(showDebugLogs) Debug.Log("answer : " + responseBody);

                output = (responseBody);
            }
        }catch(Exception e) {Debug.LogException(e);}
    
        if(newClient)client.Dispose();
            

        return output;
    }
    
    public async static Task<int?> GetInt(string ValueName,[CanBeNull] HttpClient client = null)
    {
        string answer = await GetString(ValueName,client);
        int? output = null;
        
        try
        {
            if (int.TryParse(answer,out int result)) return result;
            else return null;
        }
        catch(Exception e)
        {
            Debug.LogException(new Exception("c'est pas un int connard", e));
            return null;
        }
        
        return output;
    }
    
    public async static Task<float?> GetFloat(string ValueName,[CanBeNull] HttpClient client = null)
    {
        string answer = await GetString(ValueName,client);
        float? output = null;
        
        try
        {
            if (float.TryParse(answer,out float result)) return result;
            else return null;
        }
        catch(Exception e)
        {
            Debug.LogException(new Exception("c'est pas un float connard", e));
            return null;
        }
        
        return output;
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
        switch (value)
        {
            case int :
                await SetString(valueName, value.ToString(),client);
                _allIntNames.Add(valueName);
                break;
            case float:
                await SetString(valueName, value.ToString(),client);
                _allFloatNames.Add(valueName);
                break;
            case string : 
                await SetString(valueName, value.ToString(),client);
                break;
            default:
                throw new Exception( "Value type isn't supported.");
        }
    }
    
    #if UNITY_EDITOR

    [MenuItem("Playtests Analysis /Delete All Global PlayerKeys")]
    public static void DeleteAllGlobalPlayerKeys()
    {
        ClearAllValues(null, true);
    }

    [MenuItem("Playtests Analysis /Print All Development Player Data")]
    public static void PrintAllDevelopmentPlayerData() => PrintAllGlobalPlayerKeys(false);
    
    [MenuItem("Playtests Analysis /Print All Build Player Data")]
    public static void PrintAllBuildPlayerData() => PrintAllGlobalPlayerKeys(true);
    
    #endif
    
    public static async Task PrintAllGlobalPlayerKeys(bool showBuildData = false)
    {
        using (HttpClient client = new())
        {
            List<string> keys = (await FetchAllKeys(client, true)).ToList();
            keys.Sort();
            foreach (string key in keys)
            {
                if(key.StartsWith("dev_") == showBuildData) continue;
                
                if (key.EndsWith("hidden")) continue;
                
                string value = await GetString(key, client);
                
                string cleanKey = key;
                if (cleanKey.StartsWith("dev_")) cleanKey = cleanKey.Substring(4);
                if (cleanKey.StartsWith(Application.version)) cleanKey = cleanKey.Substring(Application.version.Length+1);
                cleanKey = cleanKey.RemoveConsecutiveCharacters('_');
                //cleanKey = cleanKey.Replace('_',' ');
                
                Debug.Log(cleanKey + " : " + value);
            }
        };

    }
    
    
    public static async Task<string[]> FetchAllKeys([CanBeNull] HttpClient client,bool showDebugLogs = false)
    {
        bool newClient = client == null;
        if (newClient) client = new();
        
        string request = $"http://trmpnt.okman65.xyz/api/getAllKeys";
        
        //request
        try
        {
            using (HttpResponseMessage response = await client.GetAsync(request))
            {
                if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();

                if(showDebugLogs) Debug.Log("answer : " + responseBody);
                
                string[] requests = responseBody.Substring(2,responseBody.Length - 4).Split("\",\"");
                return requests;
            }
        }catch(Exception e) {Debug.LogException(e);}
            
        
        if (newClient) client.Dispose();
        
        return Array.Empty<string>();
    }
    
    public static async Task ClearAllValues([CanBeNull] HttpClient client, bool showDebugLogs = true)
    {
        
        bool newClient = client == null;
        if (newClient) client = new();
        
        string request = $"http://trmpnt.okman65.xyz/api/clearAllEntries";
        
        //request
        try
        {
            using (HttpResponseMessage response = await client.GetAsync(request))
            {
                if(showDebugLogs) Debug.Log(" == sent request : " + request + " ==");
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                
                if(showDebugLogs) Debug.Log("answer : " + responseBody);
            }
        }catch(Exception e) {Debug.LogException(e);}
    
        if(newClient)client.Dispose();
    }
    

}

//#endif