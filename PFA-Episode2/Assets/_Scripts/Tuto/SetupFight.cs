using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetupFight : MonoBehaviour
{
    public List<PremadeSpell> SpellListData = new();
    public bool GameStart;
    [SerializeField] private AnimatedPanel _gamePanel;
    public GameObject SpellSlotHUD;
    [SerializeField] private EventTrigger _trigger;

    public static SetupFight Instance;

    private void Awake() { Instance = this; }

    private async void Start()
    {
        SpellSlotHUD.SetActive(false);
        for (int i = 0; i < SpellListData.Count; i++)
        {
            PremadeSpell spell = SpellListData[i];
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(spell.SpellData);
        }
        _gamePanel.Show();
        await CombatManager.Instance.Play();
    }

    public async UniTask DialogueSpawn(int dialogueIndex)
    {
        _trigger.enabled = true;
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(dialogueIndex);
    }

    public async UniTask Victory()
    {
        await DialogueSpawn(3);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
    }

    public void DesactiveTrigger() { _trigger.enabled = false;  }
}
