using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SetupFight : MonoBehaviour
{
    [SerializeField] private AnimatedPanel _gamePanel;
    [SerializeField] private List<PremadeSpell> _spellListData = new();
    [SerializeField] private GameObject _dialogue;

    public static SetupFight Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear(); //équipe les spells
        for (int i = 0; i < _spellListData.Count; i++)
        {
            PremadeSpell spell = _spellListData[i];
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(spell.SpellData);
        }
        DialogueSpawn(); //Dialogue
    }

    private void DialogueSpawn()
    {
        DialogueManager.Instance.StartDialogue = true;
        DialogueManager.Instance.GetDialogue(0);
    }

    public async void StartGame() //StartGame
    {
        _gamePanel.Show();
        await CombatManager.Instance.Play();
    }
}
