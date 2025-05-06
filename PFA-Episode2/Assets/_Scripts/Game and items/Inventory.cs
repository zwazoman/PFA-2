using System.Collections.Generic;
using UnityEngine;

public class Inventory : ISavable<Inventory>
{
    #region Singleton
    public static Inventory Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public List<SpellData> Spells = new();
    public List<Ingredient> Ingredients = new();
    public List<Sauce> Sauces = new();


    public void Save(byte SaveFileID)
    {
        SaveManager.Save(this, SaveFileID);
    }

    public void Load(byte LoadFileID)
    {
        SaveManager.Load(this, LoadFileID);
    }
}
