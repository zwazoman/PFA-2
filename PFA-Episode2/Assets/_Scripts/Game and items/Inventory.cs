using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : ISavable<Inventory>
{
    public List<SpellData> Spells = new();
    public List<Ingredient> Ingredients = new();
    public List<Sauce> Sauces = new();
    public List<int> playerEquipedSpellIndex = new();
    public List<SpellData> playerEquipedSpell = new();

    public string NameSave = "InventorySave";

    public void Save(string nameSave)
    {
        SaveManager.Save<Inventory>(nameSave, this, false);
    }
}