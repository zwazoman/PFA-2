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

    public static SetupFight Instance;

    private void Awake() { Instance = this; }

    private void Start()
    {
        _combatManager.OnNewTurn += FocusOnBattle;
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear(); //�quipe les spells
        for (int i = 0; i < _spellListData.Count; i++)
        {
            PremadeSpell spell = _spellListData[i];
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(spell.SpellData);
        }

        StartGame();

    }

    private void DialogueSpawn(int dialogueIndex)
    {
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(dialogueIndex);
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
                DialogueManager.Instance.GetDialogue(1);
                _combatManager.OnNewTurn -= FocusOnBattle;
            }
        }
    }

    public async UniTask Victory()
    {
        DialogueManager.Instance.GetDialogue(2);
        await UniTask.WaitUntil(() => !DialogueManager.Instance.Panel.activeSelf);
    }
}
