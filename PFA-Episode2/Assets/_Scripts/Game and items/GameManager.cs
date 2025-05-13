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
    public GameStaticData staticData;

#if UNITY_EDITOR
    //@temp
    [Header("Tests")]
    [SerializeField] List<PremadeSpell> premadeSpells = new();
    [SerializeField] bool tests = false;
#endif


    private void Awake()
    {
        if(instance == this || instance == null)
        {
            //singleton
            Debug.Log("Initializing game manager",this);
            instance = this;
            DontDestroyOnLoad(this);
            
            //load data
            staticData = Resources.Load<GameStaticData>("DishCombinationData");
            LoadOrCreateSave();

#if UNITY_EDITOR
            Debug.Log("Filling inventory with test Items and spells");
            //@temp
            if (tests)
            {
                foreach (PremadeSpell premadeSpell in premadeSpells)
                {
                    Debug.Log("- test spell ");
                    playerInventory.Spells.Add(premadeSpell.SpellData);
                }
            }
#endif

        }
        else
        {
            Destroy(this);
            throw new System.Exception("Y'avait déjà un singleton gamemanager dans la scene");
        }
        
    }

    void LoadOrCreateSave()
    {
        playerInventory = SaveManager.Load<Inventory>(playerInventory.NameSave, false);
    }
}
