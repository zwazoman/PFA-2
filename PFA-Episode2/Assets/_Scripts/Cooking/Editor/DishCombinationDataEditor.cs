using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DishCombinationData))]
public class DishCombinationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(20);
        if (GUILayout.Button("RebuildTable"))
        {
            ((DishCombinationData)target).RebuildTable();
        }
    }
}
