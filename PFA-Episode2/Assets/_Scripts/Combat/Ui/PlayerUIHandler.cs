using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(PlayerEntity))]
public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] PlayerEntity _player;

    [SerializeField] GameObject _uiSpellPrefab;

    Inventory inventory;

    private void Awake()
    {
        if(_player == null)
            TryGetComponent(out _player);
    }

    private void Start()
    {
        AssignEndTurnButton();

        if (inventory != null)
            foreach (SpellData spell in inventory.Spells)
            {
                CreateSpellUI(spell);
            }
    }

    void CreateSpellUI(SpellData spellData)
    {
        GameObject uiSpell = Instantiate(_uiSpellPrefab, CombatUiManager.Instance.playerSpellGroup.transform);

        Image spellImage;
        DraggableSpell draggableSpell;

        uiSpell.TryGetComponent(out spellImage);
        uiSpell.TryGetComponent(out draggableSpell);

        spellImage.sprite = spellData.Sprite;
        draggableSpell.spell = spellData;
        draggableSpell.spellCaster = _player.EntitySpellCaster;

        _player.spellsUI.Add(draggableSpell);
    }

    void AssignEndTurnButton()
    {
        _player.endTurnButton = CombatUiManager.Instance.endButton;
    }

}
