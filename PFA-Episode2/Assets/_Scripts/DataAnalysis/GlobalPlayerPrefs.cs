using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

#if DEVELOPMENT_BUILD || UNITY_EDITOR

public static class GlobalPlayerPrefs
{
    
    public async static Task SetString(string ValueName,string value)
    {
        using (System.Net.Http.HttpClient client = new())
        {
            //cleanup value name
            string cleanName = "";
            foreach (char c in ValueName)
            {
                if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) cleanName += c;
            }
            string request = $"https://trmpnt.okman65.xyz/api/setValue/{cleanName}/{value}\n";
            
            //request
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(request))
                {
                    Debug.Log(" == sent request : " + request + " ==");
                    response.EnsureSuccessStatusCode();
                    
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Debug.Log(response.ReasonPhrase);
                    // Debug.Log(response.StatusCode);
                    // Debug.Log(response.RequestMessage.Headers);
                    Debug.Log("answer : " + responseBody);
                }
            }catch(Exception e) {Debug.LogException(e);}
            
        };
    }
    
    public async static Task<string?> GetString(string ValueName)
    {
        string? output = null;
        
        using (System.Net.Http.HttpClient client = new())
        {
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
                    Debug.Log(" == sent request : " + request + " ==");
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    //Debug.Log(response.ReasonPhrase);
                    //Debug.Log(response.StatusCode);
                    //Debug.Log(response.RequestMessage.Headers);
                    Debug.Log("answer : " + responseBody);

                    output = (responseBody);
                }
            }catch(Exception e) {Debug.LogException(e);}
        };

        return output;
    }
    public async static Task<int?> GetInt(string ValueName)
    {
        string answer = await GetString(ValueName);
        int? output = null;
        
        try
        {
            output = int.Parse(answer);
        }
        catch(Exception e) when (e is FormatException or ArgumentNullException)
        {
            Debug.LogException(new Exception("c'est pas un int connard", e));
            return null;
        }
        
        return output;
    }
    
    public async static Task<float?> GetFloat(string ValueName)
    {
        string answer = await GetString(ValueName);
        float? output = null;
        
        try
        {
            output = float.Parse(answer);
        }
        catch(Exception e) when (e is FormatException or ArgumentNullException)
        {
            Debug.LogException(new Exception("c'est pas un float connard", e));
            return null;
        }
        
        return output;
    }
    
    /// <summary>
    /// supports ints, floats and strings.
    /// </summary>
    /// <param name="ValueName"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="Exception"></exception>
    public async static Task SetValue<T>(string ValueName, T value)
    {
        switch (value)
        {
            case int :
                await SetString(ValueName, value.ToString());
                break;
            case float:
                await SetString(ValueName, value.ToString());
                break;
            case string : 
                await SetString(ValueName, value.ToString());
                break;
            default:
                throw new Exception( "Value type isn't supported.");
        }
    }
    
    /*[MenuItem("Data/test set int")]
    public static void testSetInt()
    {
        SetValue("test1212",69.5f);
    }*/
    
    /*[MenuItem("Data/show player data")]
    public static void PrintAllData()
    {
        GetString("test1212");
        GetFloat("test1212");
        GetInt("test1212");
    }*/
}

#endif