using System.Collections.Generic;
using UnityEditor;
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

    public List<string> combatScenesName = new();

#if UNITY_EDITOR

    [SerializeField] List<SceneAsset> _combatScenes = new();

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
            staticData = Resources.Load<GameStaticData>("GameStaticData");
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

    public void DeleteSave()
    {
        SaveManager.Delete(playerInventory.NameSave);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_combatScenes.Count == 0)
            return;
        foreach(SceneAsset scene in _combatScenes)
        {
            if (!combatScenesName.Contains(scene.name))
                combatScenesName.Add(scene.name);
        }
    }

#endif
    public string ReturnSceneCombat()
    {
        int index = Random.Range(0, combatScenesName.Count);
        return combatScenesName[index];
    }
}
