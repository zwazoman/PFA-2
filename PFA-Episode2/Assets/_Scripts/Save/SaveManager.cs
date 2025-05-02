using System.IO;
using UnityEngine;

public static class SaveManager
{
    // Sauvegarde d'un objet ISavable
    public static void Save<T>(T savableObject, byte saveFileID) where T : ISavable<T>
    {
        string path = Application.persistentDataPath + $"/save_{saveFileID}.json";

        string json = JsonUtility.ToJson(savableObject);
        File.WriteAllText(path, json);

        Debug.Log($"{typeof(T).Name} sauvegardé à {path}");
    }

    // Chargement d'un objet ISavable
    public static T Load<T>(byte saveFileID) where T : ISavable<T>, new()
    {
        string path = Application.persistentDataPath + $"/save_{saveFileID}.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            T loadedObject = JsonUtility.FromJson<T>(json);
            Debug.Log($"{typeof(T).Name} chargé depuis {path}");
            return loadedObject;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée !");
            return new T(); // Retourne une nouvelle instance de l'objet si aucune sauvegarde n'est trouvée
        }
    }
}
