using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public Dialogue TextData;

    [SerializeField] TMP_Text _nameCharacter;
    [SerializeField] TMP_Text _text;

    [SerializeField] private float _characterDelay = 0.05f;
    [SerializeField] private float _punctuationDelay = 0.1f;

    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _textBox;

    private readonly List<Tweener> _shakeTweeners = new();
    private bool _isWriting;
    private int _numberDialogue = 0;
    private int _numberSentence = 0;
    private bool _isFinish;

    [SerializeField] private BreadGuy _bread;

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
    }
    #endregion

    // Lis quel dialogue suis pour l'activer
    public async UniTask NextDialogue() 
    { 
        await UniTask.Delay(100);
        _characterDelay = 0.06f;
        _punctuationDelay = 0.01f;
        SearchDialogue(_numberDialogue);
    }
    public void GetRandomSequenceDialogue() { SearchDialogue(Random.Range(0, TextData.DialogueData.Count)); }
    public void GetDialogue(int NumberDialogue) { SearchDialogue(NumberDialogue); }

    // Divise les différents éléments pour les ranger par ligne puis par éléments, ensuite cherche la clé corresspondante
    private void SearchDialogue(int NumberDialogue)
    {
        _numberDialogue = NumberDialogue;
        if (_isWriting) return;

        StopShakeEffect();

        for (int i = 0; i < TextData.DialogueData.Count; i++)
        {
            if (i == NumberDialogue)
            {
                Sentence sentence = TextData.DialogueData[i].Sentence[_numberSentence];
                _nameCharacter.text = sentence.CharacterName;
                WriteText(sentence.Text, sentence.ShakeTextBox, sentence.ShakeText, sentence.LetterByLetter);
            }
        }
    }
    
    // Interprete les différents élément pour savoir quel effet appliqué
    private void WriteText(string dialogue, bool shakeTextBox, bool shakeText, bool letterByletter)
    {
        if (shakeTextBox) _textBox.transform.DOShakePosition(0.5f, 50);

        if (letterByletter)
        {
            LetterByLetter(dialogue, shakeText).Forget();
            _numberSentence++;
        }
        else
        {
            if (dialogue.StartsWith("-"))
            {
                EndDialogue();
                return;
            }
            _text.text = dialogue;
            if (shakeText) StartShakeEffect();
            _numberSentence++;
        }
    }

    // Affiche lettre par lettre le dialogue
    private async UniTaskVoid LetterByLetter(string message, bool shakeText)
    {
        _isWriting = true;
        _text.text = "";

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

            if (shakeText && !insideTag) StartShakeEffect(); // Évite l'effet sur les balises

            if (!insideTag) await UniTask.Delay(System.TimeSpan.FromSeconds(speed));
        }

        _isWriting = false;
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
        foreach (Tweener tween in _shakeTweeners)
        {
            if (tween.IsActive()) tween.Kill();
        }

        _shakeTweeners.Clear();
    }

    private async void EndDialogue()
    {
        if (!_isFinish)
        {
            _isFinish = true;
            await _panel.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InOutBack);
            _panel.SetActive(false);
            await UniTask.Delay(500);
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
        _numberSentence = 0;
        _nameCharacter.text = "";
        _text.text = "";
    }

    public void Click()
    {
        if(_bread.StartDialogue)
        {
            _characterDelay = 0;
            _punctuationDelay = 0;
            NextDialogue();
        }
    }
}
