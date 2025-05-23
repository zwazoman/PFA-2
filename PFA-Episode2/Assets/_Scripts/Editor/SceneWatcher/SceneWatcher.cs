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
        EditorSceneManager.sceneClosed += OnSceneClosed;
    }

    static void OnSceneEdited(Scene scene)
    {
        //Debug.Log("Edited scene : " + scene.name);
    }

    static void OnSceneClosed(Scene scene)
    {
        //Debug.Log("closed scene : " + scene.name);
    }
}