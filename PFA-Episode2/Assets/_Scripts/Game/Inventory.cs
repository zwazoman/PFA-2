using System.Collections.Generic;
using UnityEngine;

public class Inventory : ISavable<Inventory>
{
    public List<SpellData> Spells;
    public List<IngredientBase> Ingredients;

    //@save
    public Inventory Load(byte SaveFileID)
    {
        throw new System.NotImplementedException();
    }

    //@save
    public void Save(byte SaveFileID)
    {
        throw new System.NotImplementedException();
    }


}
