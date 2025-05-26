using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class EmptyGameObjectCleaner
{
    [MenuItem("Tools/Remove Empty GameObjects")]
    [System.Obsolete]
    public static void RemoveEmptyGameObjects()
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        List<GameObject> Removable = new();

        foreach (GameObject go in allObjects)
        {
            if (IsEmpty(go) && !go.name.StartsWith("---") && go.transform.parent == null)
                Removable.Add(go);
        }

        int removed = 0;
        foreach (GameObject go in Removable)
        {
            if (EditorUtility.DisplayDialog("Supprimer cet objet vide ?",
                $"Nom : {go.name}\nChemin : {GetFullPath(go.transform)}", "Supprimer", "Garder"))
            {
                Object.DestroyImmediate(go);
                removed++;
            }
        }

        EditorUtility.DisplayDialog("Nettoyage terminé", $"{removed} objets supprimés.", "OK");
    }

    private static bool IsEmpty(GameObject go)
    {
        if (go.transform.childCount > 0)
            return false;

        Component[] components = go.GetComponents<Component>();
        foreach (Component c in components)
        {
            if (!(c is Transform))
                return false;
        }

        return true;
    }

    private static string GetFullPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
