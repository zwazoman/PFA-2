using System.Collections.Generic;

public class Inventory : ISavable<Inventory>
{
    public List<SpellData> Spells = new();
    public List<Ingredient> Ingredients = new();
    public List<Sauce> Sauces = new();


    public void Save(byte SaveFileID)
    {
        SaveManager.Save(this, SaveFileID);
    }

    public Inventory Load(byte SaveFileID)
    {
        return SaveManager.Load<Inventory>(SaveFileID);
    }
}
