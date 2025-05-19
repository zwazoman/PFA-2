using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string GetPath(string saveKey)
    {
        return Path.Combine(Application.persistentDataPath, $"{saveKey}.json");
    }

    public static void Save<T>(string saveKey, T data, bool encrypt = false)
    {
        string json = JsonUtility.ToJson(data, true);

        if (encrypt) json = EncryptDecrypt(json);

        File.WriteAllText(GetPath(saveKey), json);
        Debug.Log($"Données sauvegardées, clé : '{saveKey}'");
    }

    public static T Load<T>(string saveKey, bool encrypt = false) where T : new()
    {
        string path = GetPath(saveKey);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Aucun fichier de sauvegarde trouvé à '{path}'");
            return new T();
        }

        string json = File.ReadAllText(path);

        if (encrypt) json = EncryptDecrypt(json);

        T data = JsonUtility.FromJson<T>(json);
        Debug.Log($"Données chargées depuis '{saveKey}'");
        return data;
    }

    // Cryptage XOR
    private static string EncryptDecrypt(string data)
    {
        char key = 'K';
        char[] output = new char[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            output[i] = (char)(data[i] ^ key);
        }

        return new string(output);
    }

    public static void Delete(string saveKey)
    {
        string path = GetPath(saveKey);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Fichier '{saveKey}' supprimé");
        }
    }

    public static void DeleteAll()
    {
        string saveDirectory = Application.persistentDataPath;

        if (Directory.Exists(saveDirectory))
        {
            string[] files = Directory.GetFiles(saveDirectory);

            foreach (string filePath in files)
            {
                File.Delete(filePath);
                Debug.Log($"Deleted: {filePath}");
            }
        }
    }
}

