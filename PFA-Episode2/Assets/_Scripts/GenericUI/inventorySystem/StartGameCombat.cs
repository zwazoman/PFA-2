using DG.Tweening;
using UnityEngine;

public class StartGameCombat : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _equippedSpellSlot;
    public async void StartGame() 
    { 
        if(GameManager.Instance.playerInventory.playerEquipedSpell.Count > 0) 
        {
            gameObject.SetActive(false);
            await CombatManager.Instance.StartGame();
        }
        else
        {
            _inventoryPanel.transform.DOShakePosition(0.4f, 15);
            _equippedSpellSlot.transform.DOShakePosition(0.4f, 27);
        }
    }
}
