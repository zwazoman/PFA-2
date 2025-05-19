using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset _textData;

    [SerializeField] TMP_Text _nameCharacter;
    [SerializeField] TMP_Text _text;

    [SerializeField] private float _characterDelay = 0.05f;
    [SerializeField] private float _punctuationDelay = 0.1f;

    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _textBox;

    private List<Tweener> _shakeTweeners = new();
    private bool _textEffect = false;
    private bool _isWriting = false;
    private string _nextDialogueKey;
    private List<string> _colors = new();
    private List<string[]> _intsColor = new();
    private List<string> _styles = new();
    private List<string[]> _intsStyle = new();
    private bool IsPunctuation(char c) => c is '?' or '.' or '!' or ',';

    #region Singleton
    public static DialogueManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    // Lis quel dialogue suis pour l'activer
    public void NextDialogue()
    {
        SearchDialogue(_nextDialogueKey);
    }

    // Appelle la clé de fin de dialogue du Excel
    public void Skip()
    {
        SearchDialogue("Fin");
    }

    // Divise les différents éléments pour les ranger par ligne puis par éléments, ensuite cherche la clé corresspondante
    public void SearchDialogue(string textKey)
    {
        if (_isWriting) return;

        StopShakeEffect();

        _panel.transform.DOLocalMoveY(0, 0.5f, false);

        string[] lines = _textData.text.Split("\n");

        foreach (string line in lines)
        {
            string[] elements = line.Split(";");

            if (elements.Length >= 7 && elements[0] == textKey)
            {
                _colors.Clear();
                _intsColor.Clear();

                if (elements[6].StartsWith("#"))
                {
                    string[] color = elements[6].Split("/");

                    foreach (string infocolor in color)
                    {
                        if (infocolor.StartsWith("#"))
                        {
                            _colors.Add(infocolor);
                            Debug.Log(infocolor);
                        }
                        else if (infocolor.StartsWith(":"))
                        {
                            string[] indices = infocolor.Substring(1).Split(":");
                            _intsColor.Add(indices);
                            Debug.Log(infocolor);
                        }
                    }
                }

                string[] style = elements[2].Split("/");

                foreach (string infostyle in style)
                {
                    if (infostyle.StartsWith(":"))
                    {
                        string[] indices = infostyle.Substring(1).Split(":");
                        _intsStyle.Add(indices);
                    }
                    else
                    {
                        infostyle.ToLower();
                        _styles.Add(infostyle);
                    }
                }

                _nameCharacter.text = elements[1];

                _nextDialogueKey = elements[5];
                string dialogue = FontStyle(elements[4]);
                dialogue = ColorWord(dialogue);
                _textEffect = TextEffect(elements[3]);
                TextBoxEffect(dialogue);

                break;
            }
        }
    }
    
    // Interprete les différents élément pour savoir quel effet appliqué
    private void TextBoxEffect(string dialogue)
    {
        #region Effect
        if (dialogue.StartsWith("*"))
        {
            LetterByLetter(dialogue, 1).Forget();
        }
        else if (dialogue.StartsWith("!*"))
        {
            _textBox.transform.DOShakePosition(0.5f, 50);
            LetterByLetter(dialogue, 2).Forget();
        }
        else if (dialogue.StartsWith("!"))
        {
            dialogue = dialogue.Substring(1);
            _textBox.transform.DOShakePosition(0.5f, 50);
            _text.text = dialogue;
        }
        else if (dialogue.StartsWith("-"))
        {
            _panel.transform.DOLocalMoveY(-500, 0.5f, false);
        }
        else
        {
            _text.text = dialogue;
        }
        #endregion
    }

    // Affiche lettre par lettre le dialogue
    private async UniTaskVoid LetterByLetter(string message, byte subtext)
    {
        if (message.Length <= subtext)
        {
            _text.text = "";
            return;
        }

        _isWriting = true;
        _text.text = "";

        message = message.Substring(subtext);
        bool insideTag = false;

        for (int i = 0; i < message.Length; i++)
        {
            float speed = _characterDelay;
            char currentChar = message[i];

            // Gestion des balises <...>
            if (currentChar == '<') insideTag = true;
            if (currentChar == '>') insideTag = false;
            if (IsPunctuation(currentChar)) speed = _punctuationDelay;

            _text.text += currentChar;

            if (_textEffect && !insideTag) StartShakeEffect(); // Évite l'effet sur les balises

            if (!insideTag) await UniTask.Delay(System.TimeSpan.FromSeconds(speed));
        }

        _isWriting = false;
    }

    // Regarde s'il faut mettre un effet au texte
    private bool TextEffect(string effect)
    {
        return effect.ToUpper().StartsWith("T");
    }

    // Enregistre les différents sommets de chaque lettres du texte pour leur mettre un effet de shake
    // Peut être trop coûteux pour mobile donc à voir
    private void StartShakeEffect()
    {
        _text.ForceMeshUpdate();
        TMP_TextInfo textInfo = _text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;

            Vector3[] origVerts = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                origVerts[j] = vertices[vertexIndex + j];
            }

            // Permet de faire en sorte que le tween dure même sans update (trouvé sur internet)
            #region Tween Infini
            Tweener shake = DOTween.To(
                () => 0f,
                _ =>
                {
                    for (int j = 0; j < 4; j++)
                    {
                        float randomX = Random.Range(-5f, 5f);
                        float randomY = Random.Range(-5f, 5f);
                        vertices[vertexIndex + j] = origVerts[j] + new Vector3(randomX, randomY, 0);
                    }

                    _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                },
                0f,
                0.05f
            ).SetEase(Ease.Flash)
             .SetLoops(-1, LoopType.Restart);
            #endregion

            _shakeTweeners.Add(shake);
        }
    }

    // Arrete l'effet de shake en arretant tout les tween concernant les ShakeEffect
    private void StopShakeEffect()
    {
        foreach (var tween in _shakeTweeners)
        {
            if (tween.IsActive()) tween.Kill();
        }

        _shakeTweeners.Clear();
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
}
