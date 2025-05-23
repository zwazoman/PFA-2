using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class IgnoreFolderSelection
{
    public static Object ActiveSelection;

    private static bool _active = false;

    public static bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = value;

            if (_active)
            {
                Selection.selectionChanged += OnSelectionChanged;
                Debug.Log("IgnoreFolderSelection activ� !");
            }
            else
            {
                Selection.selectionChanged -= OnSelectionChanged;
                ActiveEditorTracker.sharedTracker.isLocked = false;
                Debug.Log("IgnoreFolderSelection d�sactiv� !");
            }
        }
    }

    [MenuItem("Tools/Ignore Folders")]
    public static void ToggleActivation()
    {
        Active = !Active;
    }

    static IgnoreFolderSelection()
    {
        // Pour initialiser � false sans d�clencher l�abonnement inutilement
        _active = false;
    }

    private static void OnSelectionChanged()
    {
        Object active = Selection.activeObject;
        if (active == null) return;

        string path = AssetDatabase.GetAssetPath(active);

        if (Directory.Exists(path))
        {
            return;
        }

        ActiveEditorTracker.sharedTracker.isLocked = false;

        ActiveSelection = active;

        ActiveEditorTracker.sharedTracker.isLocked = true;
    }
}
