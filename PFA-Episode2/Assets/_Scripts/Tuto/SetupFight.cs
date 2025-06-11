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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _combatManager.OnNewTurn += FocusOnPlayer;
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear(); //équipe les spells
        for (int i = 0; i < _spellListData.Count; i++)
        {
            PremadeSpell spell = _spellListData[i];
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(spell.SpellData);
        }
        DialogueSpawn(0); //Dialogue
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

    public void FocusOnPlayer(Entity entity)
    {
        if (entity.team == Team.Player)
        {
            _gameRound++;
            print("C'est le tour du joueur");
            if (_gameRound == 2)
            {
                DialogueManager.Instance.GetDialogue(1);
                _combatManager.OnNewTurn -= FocusOnPlayer;
            }
        }
    }
}
