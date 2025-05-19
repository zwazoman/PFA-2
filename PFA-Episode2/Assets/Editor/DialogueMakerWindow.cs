using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueMakerWindow : EditorWindow
{
    public TextAsset TextData;

    public List<SceneText> Dialogues;

    [MenuItem("Window/Dialogue Maker")]
    static void ShowWindow()
    {
        GetWindow<DialogueMakerWindow>("DialogueMakerWindow");
    }

    void OnGUI()
    {
        GUILayout.Label("Transform excel file into dialogue for game", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        TextData = EditorGUILayout.ObjectField(TextData, typeof(TextAsset), false) as TextAsset;
        EditorGUILayout.Space();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("Dialogues");

        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
        EditorGUILayout.Space();

        if (GUILayout.Button("Read File"))
        {
            ReadDialogue();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Export Dialogue"))
        {
            ExportDialogue();
        }
    }

    void ReadDialogue()
    {
        string[] lines = TextData.text.Split("\n");

        foreach (string line in lines)
        {
            Sentence sentence = new Sentence();

            string[] elements = line.Split(";");

            if (elements.Length >= 7)
            {
                sentence.CharacterName = elements[1];
                sentence.ShakeText = TextEffect(elements[3]);
                sentence.LetterByLetter = LetterByLetter(elements[3]);
                sentence.ShakeTextBox = TextBoxEffect(elements[3]);

            }
        }
    }

    bool TextEffect(string effect)
    {
        return effect.ToUpper().Contains("T");
    }

    bool LetterByLetter(string effect)
    {
        return effect.Contains("*");
    }

    bool TextBoxEffect(string effect)
    {
        return effect.Contains("!");
    }

    void ExportDialogue()
    {

    }
}
