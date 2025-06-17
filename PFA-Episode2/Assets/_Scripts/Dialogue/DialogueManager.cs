using DG.Tweening;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Dialogue TextData;

    [SerializeField] TMP_Text _nameCharacter;
    [SerializeField] TMP_Text _text;

    [SerializeField] private float _characterDelay = 0.01f;
    [SerializeField] private float _punctuationDelay = 0.4f;

    public GameObject Panel;
    [SerializeField] private GameObject _panelDeco;
    [SerializeField] private GameObject _textBox;

    private readonly List<Tweener> _shakeTweeners = new();
    private bool _isWriting;
    private int _numberDialogue = 0;
    private int _numberSentence = 0;
    private bool _isFinish;
    public bool StartDialogue;
    private string _currentFullMessage = "";
    private bool _skipRequested = false;
    public bool IsEndingDialogue = false;



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
    private bool _isAdvancingDialogue = false;

    public async UniTask NextDialogue()
    {
        if (_isAdvancingDialogue) return;

        _isAdvancingDialogue = true;

        _characterDelay = 0.02f;
        _punctuationDelay = 0.08f;

        SearchDialogue(_numberDialogue);

        _isAdvancingDialogue = false;
    }

    public void GetRandomSequenceDialogue() { SearchDialogue(UnityEngine.Random.Range(0, TextData.DialogueData.Count)); }
    public void GetDialogue(int NumberDialogue) { SearchDialogue(NumberDialogue); }

    // Divise les diff�rents �l�ments pour les ranger par ligne puis par �l�ments, ensuite cherche la cl� corresspondante
    private async void SearchDialogue(int NumberDialogue)
    {
        Panel.SetActive(true);
        _panelDeco.SetActive(true);
        if (Panel.transform.localScale==Vector3.zero) await Panel.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuart);
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
    
    // Interprete les diff�rents �l�ment pour savoir quel effet appliqu�
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
    private async UniTask LetterByLetter(string message, bool shakeText)
    {
        _isWriting = true;
        _skipRequested = false;
        _currentFullMessage = message;
        _text.text = "";

        bool insideTag = false;

        for (int i = 0; i < message.Length; i++)
        {
            if (_skipRequested)
            {
                _text.text = _currentFullMessage;
                _isWriting = false;
                return;
            }

            float delay = _characterDelay;
            char currentChar = message[i];

            if (currentChar == '<') insideTag = true;
            if (currentChar == '>') insideTag = false;
            bool isPunctuation = IsPunctuation(currentChar) && (i<message.Length-1 && message[i+1] == ' '); 
            if (isPunctuation) delay = _punctuationDelay;

            _text.text += currentChar;

            if (shakeText && !insideTag) StartShakeEffect();

            if (!insideTag && (i%3==0 ||isPunctuation)) await UniTask.WaitForSeconds(delay);
        }

        _isWriting = false;
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
                        float randomX = UnityEngine.Random.Range(-5f, 5f);
                        float randomY = UnityEngine.Random.Range(-5f, 5f);
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
        if (IsEndingDialogue) return;
        IsEndingDialogue = true;

        string currentSceneName = SceneManager.GetActiveScene().name;

        await Panel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuart);
        Panel.SetActive(false);

        if (!_isFinish && currentSceneName == "Heal")
        {
            _isFinish = true;
            await UniTask.Delay(500);
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
        else if (currentSceneName == "Forest_Combat_Tuto" && !SetupFight.Instance.GameStart)
        {
            //await SetupFight.Instance.Pain.DOAnchorPos(new Vector2(0, -875), 0.2f).SetEase(Ease.InOutBack);
        }

        _panelDeco.SetActive(false);

        _numberSentence = 0;
        _nameCharacter.text = "";
        _text.text = "";
        IsEndingDialogue = false;
    }


    public void Click()
    {
        if (!StartDialogue) return;

        if (_isWriting)
        {
            _skipRequested = true;
        }
        else
        {
            _characterDelay = 0;
            _punctuationDelay = 0;
            NextDialogue().Forget();
        }
    }


}
