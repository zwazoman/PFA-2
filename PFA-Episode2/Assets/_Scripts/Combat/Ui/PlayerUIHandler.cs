using UnityEngine;

[RequireComponent (typeof(PlayerEntity))]
public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] PlayerEntity _player;

    [SerializeField] DraggableSpell _uiSpellPrefab;

    private void Awake()
    {
        if(_player == null)
            TryGetComponent(out _player);
    }

    private void Start()
    {
        AssignEndTurnButton();

        if (GameManager.Instance.playerInventory != null)
        {
            //instantiate draggable spell slots
            Debug.Log("Forrrrrr");
            for (
                int i = 0;
                i < Mathf.Min(3, GameManager.Instance.playerInventory.Spells.Count);
                i++)
            {
                Spell spell = new();
                spell.spellData = GameManager.Instance.playerInventory.Spells[i];

                CreateSpellUI(spell,i);
            }
        }
    }

    void CreateSpellUI(Spell spellData,int i)
    {
        Debug.Log("nique sa mere ptn - " + i.ToString());
        DraggableSpell draggableSpellSlot = Instantiate(_uiSpellPrefab, CombatUiManager.Instance.SpellSlots[i]);
        draggableSpellSlot.SetUp(spellData, _player);

        _player.spellsUI.Add(draggableSpellSlot);
    }

    void AssignEndTurnButton()
    {
        _player.endTurnButton = CombatUiManager.Instance.endButton;
    }
}
