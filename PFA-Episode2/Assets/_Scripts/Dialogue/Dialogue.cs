using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<DialogueData> DialogueData;
}

[Serializable]
public class SceneText
{
    public List<DialogueData> DialogueDatas = new();
}

[Serializable]
public class DialogueData
{
    public List<Sentence> Sentence = new();
}

[Serializable]
public class Sentence
{
    public string CharacterName;
    public string Text;
    public string Key;
    public string NextKey;
    public bool ShakeTextBox;
    public bool ShakeText;
    public bool LetterByLetter;
}