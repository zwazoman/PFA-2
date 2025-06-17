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
            }
            else
            {
                Selection.selectionChanged -= OnSelectionChanged;
                ActiveEditorTracker.sharedTracker.isLocked = false;
            }
        }
    }

    [MenuItem("Tools/Ignore Folders", false)]
    public static void ToggleActivation()
    {
        Active = !Active;
    }

    [MenuItem("Tools/Ignore Folders", true)]
    public static bool ToggleActivationValidate()
    {
        Menu.SetChecked("Tools/Ignore Folders", Active);
        return true;
    }

    static IgnoreFolderSelection()
    {
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
