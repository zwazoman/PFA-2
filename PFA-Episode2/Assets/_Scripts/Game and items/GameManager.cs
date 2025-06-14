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

    public float progress;
    public bool FirstPlay = true;
    public Inventory playerInventory = new();
    public SerializablePlayer PlayerPosMap = new SerializablePlayer();

    [Header("Data")]
    public GameStaticData staticData;

    [SerializeField] List<string> combatScenesName = new();

#if UNITY_EDITOR

    //[SerializeField] List<SceneAsset> _combatScenes;

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
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            //load data
            staticData = Resources.Load<GameStaticData>("GameStaticData");
            //LoadOrCreateSave();

#if UNITY_EDITOR
            //@temp
            if (tests)
            {
                foreach (PremadeSpell premadeSpell in premadeSpells)
                {
                    playerInventory.playerEquipedSpell.Add(premadeSpell.SpellData);
                }
            }
#endif

        }
        else
        {
            Destroy(this);
            throw new System.Exception("Y'avait d�j� un singleton gamemanager dans la scene");
        }
    }

    public void LoadOrCreateSave()
    {
        playerInventory = SaveManager.Load<Inventory>(playerInventory.NameSave, SaveMapGeneration.Instance.Encrypt);
    }

    public void DeleteSave()
    {
        SaveManager.Delete(playerInventory.NameSave);
    }

/* #if UNITY_EDITOR
    private void OnValidate()
    {

        if (_combatScenes.Count == 0 || _combatScenes == null)
            return;
        foreach(SceneAsset scene in _combatScenes)
        {
            if (!combatScenesName.Contains(scene.name) && combatScenesName!=null)
                combatScenesName.Add(scene.name);
        }
    }

#endif*/

    public string GetRandomCombatScene()
    {
        int index = Random.Range(0, combatScenesName.Count);
        return combatScenesName[index];
    }
    public void CalculateProgress() //d�finit au start de la WorldMap soit � chaque fois qu'on y retourne
    {
        int currentNode = Mathf.Clamp(PlayerMap.Instance.PositionMap, 0, MapMaker2.Instance.MapRange);
        progress = (float)currentNode / MapMaker2.Instance.MapRange; // ENTRE 0 ET 1
    }

    public int ComputeEnnemiesCount() { return Mathf.FloorToInt(progress * 4) + 1; }
}
