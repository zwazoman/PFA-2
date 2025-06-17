using System.IO;
using UnityEditor;
using UnityEngine;

public static class DeleteSaveUtility
{
    [MenuItem("Save/Delete Save Files")]
    public static void DeleteSave()
    {
        string saveDirectory = Application.persistentDataPath;

        if (Directory.Exists(saveDirectory))
        {
            string[] files = Directory.GetFiles(saveDirectory);

            foreach (string filePath in files)
            {
                File.Delete(filePath);
            }

            EditorUtility.DisplayDialog("Succ�s", "Tous les fichiers de sauvegarde ont �t� supprim�s", "OK");
        }
    }
}

public static class OpenSaveFolder
{
    [MenuItem("Save/Open Save Folder")]
    public static void OpenFolder()
    {
        string Path = Application.persistentDataPath + "/";

        EditorUtility.RevealInFinder(Path);
    }
}
