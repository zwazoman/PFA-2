using Codice.Utils;
using System.IO;
using System.Linq;
using System.Security.Principal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


[InitializeOnLoad]
class SceneWatcher
{
    static SceneWatcher()
    {
        EditorSceneManager.sceneDirtied += OnSceneEdited;
        EditorSceneManager.sceneSaved += OnSceneEdited;
        EditorSceneManager.sceneClosed += OnSceneClosed;
    }

    static async void OnSceneEdited(Scene scene)
    {
        
        using (System.Net.Http.HttpClient client = new())
        {
            string Name = HttpUtility.UrlPathEncode(WindowsIdentity.GetCurrent().Name);
            string CleanName = "";
            foreach (char c in Name)
            {
                if (!Path.GetInvalidPathChars().Contains(c) && c!= '\\' ) CleanName += c;
            }
            string request = $"https://trmpnt.okman65.xyz/api/user/{CleanName}/map/edit/{scene.name}";
            Debug.Log(request);
            await client.GetAsync(request);
        };

    }

    static async void OnSceneClosed(Scene scene)
    {
        using (System.Net.Http.HttpClient client = new())
        {
            string Name = HttpUtility.UrlPathEncode(WindowsIdentity.GetCurrent().Name);
            string CleanName = "";
            foreach (char c in Name)
            {
                if (!Path.GetInvalidPathChars().Contains(c) && c != '\\') CleanName += c;
            }

            string request = $"https://trmpnt.okman65.xyz/api/user/{CleanName}/map/leave";
            Debug.Log(request);
            await client.GetAsync(request);
        };

    }
}