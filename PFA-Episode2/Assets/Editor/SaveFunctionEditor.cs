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
                Debug.Log($"Deleted: {filePath}");
            }

            EditorUtility.DisplayDialog("Succès", "Tous les fichiers de sauvegarde ont été supprimés", "OK");
        }
    }
}

public static class OpenSaveFolder
{
    [MenuItem("Save/Open Save Folder")]
    public static void OuvrirDossier()
    {
        string path = Application.persistentDataPath;

        EditorUtility.RevealInFinder(path + $"/PFA-Episode2");
    }
}
