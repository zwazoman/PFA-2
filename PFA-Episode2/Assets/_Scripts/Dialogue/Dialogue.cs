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
    public List<DialogueData> DialogueDatas;
}

[Serializable]
public class DialogueData
{
    public List<Sentence> Sentence;
}

[Serializable]
public class Sentence
{
    public string CharacterName;
    public string Text;
    public bool ShakeTextBox;
    public bool ShakeText;
    public bool LetterByLetter;
}