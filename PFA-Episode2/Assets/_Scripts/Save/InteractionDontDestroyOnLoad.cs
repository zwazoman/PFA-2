using UnityEngine;

/// <summary>
/// Script qui permet d'activer des fonction des script en DontDestroyOnLoadvia des boutons, etc...
/// </summary>
public class InteractDontDestroyOnLoad : MonoBehaviour
{
    // Fonction changer le numéro de la sauvegarde
    public void ChangeSaveID(int id)
    {
        SaveMapGeneration.Instance.SaveID = (byte)id;
    }

    // Fonction pour lancer les fonction du SaveManager
    public void SaveMapFunction(int function)
    {
        switch (function)
        {
            case 0:
                SaveMapGeneration.Instance.SaveMap();
                break;
            case 1:
                SaveMapGeneration.Instance.LoadMap();
                break;
            case 2:
                SaveMapGeneration.Instance.DeleteMap();
                break;
        }
    }

    public void SaveInventoryFunction(int function)
    {
        switch (function)
        {
            case 0:
                Inventory.Instance.Save(0);
                break;
            case 1:
                Inventory.Instance.Load(0);
                break;
        }
    }

    // Fonction pour appeler un changement de scene
    public async void ChangeScene(string sceneName)
    {
        await SceneTransitionManager.Instance.GoToScene(sceneName);
    }
}
