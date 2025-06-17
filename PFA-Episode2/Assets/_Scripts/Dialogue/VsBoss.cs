using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class VsBoss : MonoBehaviour
{
    [SerializeField] private AnimatedPanel _gamePanel;
    [SerializeField] private EventTrigger _trigger;
    public static VsBoss Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        CombatManager.Instance.Play();
    }

    public async UniTask Test()
    {
        await DialogueSpawn(0);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
        _trigger.enabled = false;
    }

    public async UniTask DialogueSpawn(int dialogueIndex)
    {
        _trigger.enabled = true;
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(dialogueIndex);
    }

    public async UniTask Victory()
    {
        await DialogueSpawn(1);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
        _trigger.enabled = false;
    }
}
