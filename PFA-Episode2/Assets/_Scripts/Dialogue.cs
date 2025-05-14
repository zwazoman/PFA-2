using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextAsset _textData;

    [SerializeField] TMP_Text _nameCharacter;
    [SerializeField] TMP_Text _text;

    [SerializeField] private float _speed = 0.05f;

    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _textBox;

    private List<Tweener> _shakeTweeners = new(); // Stocke tous les tweens actifs pour pouvoir les arr�ter
    private bool _textEffect = false;
    private bool _isWriting = false;
    private string _nextDialogueKey;

    #region Singleton
    public static Dialogue Instance;

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

    // appelle la cl� de fin de dialogue du Excel
    public void Skip()
    {
        SearchDialogue("Fin");
    }

    // Divise les diff�rents �l�ments pour les ranger par ligne puis par �l�ments, ensuite cherche la cl� corresspondante
    public void SearchDialogue(string textKey)
    {
        if (_isWriting) return;

        StopShakeEffect();

        _panel.transform.DOLocalMoveY(0, 0.5f, false);

        string[] lines = _textData.text.Split("\n");

        foreach (string line in lines)
        {
            string[] elements = line.Split(";");

            if (elements.Length >= 6 && elements[0] == textKey)
            {
                _nameCharacter.text = elements[1];

                _nextDialogueKey = elements[5];
                FontStyle(elements[2]);
                _textEffect = TextEffect(elements[3]);
                TextBoxEffect(elements[4]);

                break;
            }
        }
    }
    
    // Interprete les diff�rents �l�ment pour savoir quel effet appliqu�
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

        message = message.Substring(subtext);
        _text.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            _text.text += message[i];

            if (_textEffect) StartShakeEffect();

            await UniTask.Delay(System.TimeSpan.FromSeconds(_speed));
        }

        _isWriting = false;
    }

    // Regarde s'il faut mettre un effet au texte
    private bool TextEffect(string effect)
    {
        return effect.ToUpper().StartsWith("T");
    }

    // Enregistre les diff�rents sommets de chaque lettres du texte pour leur mettre un effet de shake
    // Peut �tre trop co�teux pour mobile donc � voir
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

            // Permet de faire en sorte que le tween dure m�me sans update (trouv� sur internet)
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

    // Lis les diff�rent caract�re pour potentielement changer le style de la police
    private void FontStyle(string style)
    {
        style = style.ToUpper();

        FontStyles fontStyle = FontStyles.Normal;

        if (style.Contains("B")) fontStyle |= FontStyles.Bold;
        if (style.Contains("I")) fontStyle |= FontStyles.Italic;

        _text.fontStyle = fontStyle;
    }
}
