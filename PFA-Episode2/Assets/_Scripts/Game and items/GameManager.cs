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
                Debug.LogWarning("no gamemanager instance found. Creating new game manager.");
                GameObject go = new GameObject("Game Manager");
                instance = go.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    #endregion

    public Inventory playerInventory = new();

    [Header("Data")]
    public DishCombinationData dishCombinationData;

    [Header("Tests")]
    [SerializeField] List<PremadeSpell> premadeSpells = new();

    private void Awake()
    {
        if(instance == this || instance == null)
        {
            Debug.Log("Initializing game manager");
            instance = this;
            DontDestroyOnLoad(this);
            
            dishCombinationData = Resources.Load<DishCombinationData>("DishCombinationData");

            //@temp
            foreach (PremadeSpell premadeSpell in premadeSpells)
            {
                playerInventory.Spells.Add(premadeSpell.SpellData);
            }

            LoadOrCreateSave();
        }
        else
        {
            Destroy(this);
            throw new System.Exception("Y'avait déjà un singleton gamemanager dans la scene");
        }
        
    }

    void LoadOrCreateSave()
    {
        playerInventory = SaveManager.Load(0);
    }
}
