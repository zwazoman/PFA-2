using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory : ISavable<Inventory>
{
    public List<SpellData> Spells = new();
    public List<Ingredient> Ingredients = new();
    public List<Sauce> Sauces = new();


    public void Save(byte SaveFileID)
    {
        SaveManager.Save<Inventory>(SaveFileID);
    }
}
