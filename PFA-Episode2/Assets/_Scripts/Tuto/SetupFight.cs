using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SetupFight : MonoBehaviour
{
    [SerializeField] private AnimatedPanel _gamePanel;
    [SerializeField] private List<PremadeSpell> _spellListData = new();
    [SerializeField] private CombatManager _combatManager;
    public bool GameStart;
    private int _gameRound;
    public RectTransform Pain;

    public static SetupFight Instance;

    private void Awake() { Instance = this; }

    private void Start()
    {
        _combatManager.OnNewTurn += FocusOnBattle;
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear(); //équipe les spells
        for (int i = 0; i < _spellListData.Count; i++)
        {
            PremadeSpell spell = _spellListData[i];
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(spell.SpellData);
        }
        DialogueSpawn(0); //Dialogue
    }

    private async UniTask DialogueSpawn(int dialogueIndex)
    {
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(dialogueIndex);
        await UniTask.Delay(250);
        await Pain.DOAnchorPos(new Vector2(0, 226), 0.4f).SetEase(Ease.OutBack);
    }

    public async void StartGame() //StartGame
    {
        _gamePanel.Show();
        await CombatManager.Instance.Play();
    }

    public void FocusOnBattle(Entity entity)
    {
        if (entity.team == Team.Player)
        {
            _gameRound++;
            if (_gameRound == 2)
            {
                DialogueSpawn(1);
                _combatManager.OnNewTurn -= FocusOnBattle;
            }
        }
    }

    public async UniTask Victory()
    {
        DialogueSpawn(2);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
    }
}
