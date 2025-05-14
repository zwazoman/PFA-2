using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameStaticData))]
public class DishCombinationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20);
        if (GUILayout.Button("RebuildTable"))
        {
            ((GameStaticData)target).RebuildTable();
        }
    }
}
