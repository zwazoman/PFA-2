using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SetupSpellInventory : MonoBehaviour
{
    [FormerlySerializedAs("_targetInventory")] [SerializeField] private List<Transform> _inventorySlots = new();
    [SerializeField] public List<Transform> _equippedInventory = new();
    [SerializeField] private GameObject _prefabItem;
    private int _index = 0;

    private void Start()
    {
        SetupInventory();
    }

    public void SetupInventory()
    {
        int mesBoules = 0;
        //create one draggable inventory slot for each item in the player's inventory
        for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
        {
            SpellData spell = GameManager.Instance.playerInventory.Spells[i];

            
            DraggableSpellContainer ItemSlot = Instantiate(_prefabItem, _inventorySlots[i]).GetComponentInChildren<DraggableSpellContainer>(); 
            ItemSlot.SetUp(spell,i);
            
            
            if(GameManager.Instance.playerInventory.playerEquipedSpellIndex.Contains(i))
            {
                ItemSlot.originalParent = ItemSlot.transform.parent;
                ItemSlot.DisplayAsEquipped(_equippedInventory[mesBoules]);
                mesBoules++;
            }
            
        }
    }

    public void PourNestror()
    {
        GameManager.Instance.playerInventory.playerEquipedSpell.Clear();
        foreach (int spellDataIndex in GameManager.Instance.playerInventory.playerEquipedSpellIndex)
        {
            GameManager.Instance.playerInventory.playerEquipedSpell.Add(GameManager.Instance.playerInventory.Spells[spellDataIndex]);
        }
    }
}
