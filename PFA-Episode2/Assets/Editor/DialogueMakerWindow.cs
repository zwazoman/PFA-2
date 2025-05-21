using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueMakerWindow : EditorWindow
{
    public TextAsset TextData;

    public List<SceneText> Dialogues = new();

    private List<string> _colors = new();
    private List<string[]> _intsColor = new();
    private List<string> _styles = new();
    private List<string[]> _intsStyle = new();

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
        if (TextData == null)
        {
            Debug.LogWarning("Aucun fichier texte sélectionné !");
            return;
        }

        string[] scenes = TextData.text.Split("|");

        for (int i = 1; i < scenes.Length; i++) // Commencer à 1 pour ignorer le premier
        {
            SceneText sceneText = new SceneText();
            string scene = scenes[i];
            string[] lines = scene.Split("\n");

            HashSet<int> alreadyParsed = new HashSet<int>();

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (alreadyParsed.Contains(lineIndex)) continue;

                string[] elements = lines[lineIndex].Split(";");

                if (elements.Length >= 7 && !string.IsNullOrWhiteSpace(elements[5]))
                {
                    DialogueData dialogueData = new DialogueData();
                    HashSet<int> visitedRefs = new HashSet<int>();

                    int currentIndex = lineIndex;

                    while (currentIndex >= 0 && currentIndex < lines.Length && !visitedRefs.Contains(currentIndex))
                    {
                        visitedRefs.Add(currentIndex);
                        alreadyParsed.Add(currentIndex);

                        string[] currentElements = lines[currentIndex].Split(";");

                        if (currentElements.Length >= 7 && !string.IsNullOrWhiteSpace(currentElements[5]))
                        {
                            Sentence sentence = new Sentence
                            {
                                CharacterName = currentElements[2],
                                Key = currentElements[1],
                                NextKey = currentElements[6],
                                ShakeText = TextEffect(currentElements[4]),
                                LetterByLetter = LetterByLetter(currentElements[4]),
                                ShakeTextBox = TextBoxEffect(currentElements[4])
                            };

                            // Gestion des couleurs
                            if (currentElements[7].StartsWith("#"))
                            {
                                string[] color = currentElements[7].Split("/");
                                foreach (string infocolor in color)
                                {
                                    if (infocolor.StartsWith("#"))
                                        _colors.Add(infocolor);
                                    else if (infocolor.StartsWith(":"))
                                        _intsColor.Add(infocolor.Substring(1).Split(":"));
                                }
                            }

                            // Gestion des styles
                            string[] style = currentElements[3].Split("/");
                            foreach (string infostyle in style)
                            {
                                if (infostyle.StartsWith(":"))
                                    _intsStyle.Add(infostyle.Substring(1).Split(":"));
                                else
                                    _styles.Add(infostyle.ToLower());
                            }

                            string dialogue = FontStyle(currentElements[5]);
                            dialogue = ColorWord(dialogue);
                            sentence.Text = dialogue;

                            dialogueData.Sentence.Add(sentence);
                        }

                        // Lire la clé référence du prochain text
                        string referenceKey = currentElements[6].Trim();
                        if (!string.IsNullOrEmpty(referenceKey) && int.TryParse(referenceKey, out int nextIndex)
                            && nextIndex >= 0 && nextIndex < lines.Length)
                        {
                            currentIndex = nextIndex;
                        }
                        else
                        {
                            break;
                        }
                    }

                    Sentence endSentence = new Sentence
                    {
                        CharacterName = "End",
                        Text = "-",
                        Key = "End",
                        NextKey = null,
                        ShakeText = false,
                        LetterByLetter = false,
                        ShakeTextBox = false
                    };
                    dialogueData.Sentence.Add(endSentence);
                    sceneText.DialogueDatas.Add(dialogueData);
                }
            }

            if (sceneText.DialogueDatas.Count > 0)
            {
                Dialogues.Add(sceneText);
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

    // Lis les différent caractère pour potentielement changer le style de la police
    private string FontStyle(string dialogue)
    {
        string[] words = dialogue.Split(' ');

        for (int i = 0; i < _intsStyle.Count; i++)
        {
            for (int j = 0; j < _intsStyle[i].Length; j++)
            {
                if (int.TryParse(_intsStyle[i][j], out int wordIndex))
                {
                    if (wordIndex >= 0 && wordIndex < words.Length)
                    {
                        words[wordIndex] = $"<{_styles[i]}>{words[wordIndex]}</{_styles[i]}>";
                    }
                }
            }
        }

        _styles.Clear();
        _intsStyle.Clear();
        return string.Join(" ", words);
    }

    // Change la couleur des mots désigné avec la/les couleurs mises 
    private string ColorWord(string dialogue)
    {
        string[] words = dialogue.Split(' ');

        for (int i = 0; i < _intsColor.Count; i++)
        {
            for (int j = 0; j < _intsColor[i].Length; j++)
            {
                if (int.TryParse(_intsColor[i][j], out int wordIndex))
                {
                    if (wordIndex >= 0 && wordIndex < words.Length)
                    {
                        words[wordIndex] = $"<color={_colors[i]}>{words[wordIndex]}</color>";
                    }
                }
            }
        }

        _colors.Clear();
        _intsColor.Clear();
        return string.Join(" ", words);
    }

    void ExportDialogue()
    {
        for (int i = 0; i <= Dialogues.Count; i++)
        {
            if (i < Dialogues.Count)
            {
                Dialogue asset = CreateInstance<Dialogue>();

                asset.DialogueData = Dialogues[i].DialogueDatas;

                string path = "Assets/_Data/Dialogues";
                if (!AssetDatabase.IsValidFolder(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/DialogueScene{i}.asset");

                AssetDatabase.CreateAsset(asset, assetPathAndName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;

                Debug.Log($"Dialogue ScriptableObject créé à : {assetPathAndName}");
            }
        }

        Dialogues.Clear();
    }
}
