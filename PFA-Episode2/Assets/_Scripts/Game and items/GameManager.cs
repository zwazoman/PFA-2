using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Game Manager");
                instance = go.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    #endregion

    public Inventory playerInventory = new();

    [SerializeField] List<PremadeSpell> premadeSpells = new();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        foreach(PremadeSpell premadeSpell in premadeSpells)
        {
            playerInventory.Spells.Add(premadeSpell.SpellData);
        }
    }
}
