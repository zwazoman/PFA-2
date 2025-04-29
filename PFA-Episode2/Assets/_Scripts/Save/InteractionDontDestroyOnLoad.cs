using UnityEngine;

/// <summary>
/// Script qui permet d'activer des fonction des script en DontDestroyOnLoadvia des boutons, etc...
/// </summary>
public class InteractDontDestroyOnLoad : MonoBehaviour
{
    // Fonction changer le numéro de la sauvegarde
    public void ChangeSaveID(int id)
    {
        SaveManager.Instance.SaveID = (byte)id;
    }

    // Fonction pour lancer les fonction du SaveManager
    public void SaveFunction(int function)
    {
        switch (function)
        {
            case 0:
                SaveManager.Instance.SaveMap();
                break;
            case 1:
                SaveManager.Instance.LoadMap();
                break;
            case 2:
                SaveManager.Instance.DeleteMap();
                break;
        }
    }

    // Fonction pour appeler un changement de scene
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene.Instance.SceneLoadingOn(sceneName));
    }
}
