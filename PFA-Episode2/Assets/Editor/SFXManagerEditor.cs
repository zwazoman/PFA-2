using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SFXManager))]
public class SFXManagerEditor : Editor
{
    SerializedProperty _poolSizeProp;
    SerializedProperty _SFXObjectProp;
    SerializedProperty _soundsDictProp;

    private void OnEnable()
    {
        _poolSizeProp = serializedObject.FindProperty("_poolSize");
        _SFXObjectProp = serializedObject.FindProperty("_SFXObject");
        _soundsDictProp = serializedObject.FindProperty("_soundsDict");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SFXManager audioManager = (SFXManager)target;
        //DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(_poolSizeProp);

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(_SFXObjectProp);

        GUILayout.Space(10);

        DrawLine(Color.grey);

        GUILayout.Space(10);

        DrawTitle("SOUNDS");

        EditorGUILayout.PropertyField(_soundsDictProp);
        
        GUILayout.Space(30);

        if (GUILayout.Button("Générer l'enum")) audioManager.GenerateSoundEnum();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawTitle(string title)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 15;
        EditorGUILayout.LabelField(title, style);
        GUILayout.Space(4);
    }

    void DrawLine(Color color,float thickness = 2f, float padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(false, thickness + padding);
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

}
