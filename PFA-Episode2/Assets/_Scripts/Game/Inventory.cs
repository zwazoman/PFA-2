using System.Collections.Generic;
using UnityEngine;

public class Inventory : ISavable<Inventory>
{
    public List<Spell> Spells;
    public List<Ingredient> Ingredients;

    public Inventory Load(byte SaveFileID)
    {
        throw new System.NotImplementedException();
    }

    public void Save(byte SaveFileID)
    {
        throw new System.NotImplementedException();
    }


}
