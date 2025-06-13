using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SetupFight : MonoBehaviour
{
    public List<PremadeSpell> SpellListData = new();
    public RectTransform Pain;
    public bool GameStart;
    [SerializeField] private AnimatedPanel _gamePanel;

    public static SetupFight Instance;

    private void Awake() { Instance = this; }

    private async void Start()
    {
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
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(dialogueIndex);
        await UniTask.Delay(250);
        await Pain.DOAnchorPos(new Vector2(0, 226), 0.4f).SetEase(Ease.OutBack);
    }

    public async UniTask Victory()
    {
        await DialogueSpawn(3);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
    }
}
