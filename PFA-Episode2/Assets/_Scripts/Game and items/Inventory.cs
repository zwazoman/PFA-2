using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : ISavable<Inventory>
{
    public PlayerHealth playerHealth = new(20);

    public List<SpellData> Spells = new();
    public List<Ingredient> Ingredients = new();
    public List<Sauce> Sauces = new();
    public List<int> playerEquipedSpellIndex = new();
    public List<SpellData> playerEquipedSpell = new();

    public int TotalTirageIngredient = 0;

    public string NameSave = "InventorySave";

    public void Save(string nameSave)
    {
        SaveManager.Save<Inventory>(nameSave, this, false);
    }

    public void AddIngredient(IngredientBase ing)
    {
        if(ing is Sauce sauce) { Sauces.Add(sauce); }
        else if (ing is Ingredient ingredient) { Ingredients.Add(ingredient); }
    }
}