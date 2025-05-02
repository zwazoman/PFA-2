using System.Collections.Generic;
using UnityEngine;

public class Inventory : ISavable<Inventory>
{
    public List<SpellData> Spells;
    public List<Ingredient> Ingredients;
    public List<Sauce> Sauces;

    public void Save(byte SaveFileID)
    {
        SaveManager.Save(this, SaveFileID);
    }

    public Inventory Load(byte SaveFileID)
    {
        return SaveManager.Load<Inventory>(SaveFileID);
    }
}
