using UnityEngine;

/// <summary>
/// Script qui permet d'activer des fonction des script en DontDestroyOnLoadvia des boutons, etc...
/// </summary>
public class InteractDontDestroyOnLoad : MonoBehaviour
{
    public Ingredient ingredient;
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

    public void SaveInventoryFunction(string function)
    {
        switch (function)
        {
            case "Save":
                GameManager.Instance.playerInventory.Save(GameManager.Instance.playerInventory.NameSave);
                break;
            case "Delete":
                GameManager.Instance.DeleteSave();
                break;
        }
    }

    // Fonction pour appeler un changement de scene
    public async void ChangeScene(string sceneName)
    {
        await SceneTransitionManager.Instance.GoToScene(sceneName);
    }

    public void Yadesobjets()
    {
        GameManager.Instance.playerInventory.Ingredients.Add(ingredient);
    }
}
