using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    #region Singleton
    private static CombatManager instance;

    public static CombatManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Combat Manager");
                instance = go.AddComponent<CombatManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        TotalEncounteredCombatsCountOverRun++;
    }
    #endregion

    [HideInInspector] public List<Entity> PlayerEntities = new();
    [HideInInspector] public List<Entity> EnemyEntities = new();

    [SerializeField] public List<SpawnSetup> Setups = new();

    public List<Entity> Entities { get; private set; } = new();

    [SerializeField] bool _summonEntities;
    [SerializeField] bool _startGameOnSceneStart = false;

    [SerializeField] GameOverPanel _gameOverPanel;
    [SerializeField] RewardBundle _rewardPanel;

    public event Action<Entity> OnNewTurn;
    public event Action OnWin;

    #region entity registration

    public void RegisterEntity(Entity entity)
    {
        Entities.Add(entity);
        if (entity.team == Team.Player)
        {
            PlayerEntities.Add(entity);
        }
        else if (entity.team == Team.Enemy)
        {
            EnemyEntities.Add(entity);
        }
    }

    public async UniTask UnRegisterEntity(Entity entity)
    {
        Entities.Remove(entity);
        if (entity.team == Team.Player && PlayerEntities.Contains(entity))
            PlayerEntities.Remove(entity);
        else if (entity.team == Team.Enemy && EnemyEntities.Contains(entity))
        {
            EnemyEntities.Remove(entity);
            print(EnemyEntities.Count);
            if (EnemyEntities.Count == 0)
            {
                foreach(Entity connard in PlayerEntities)
                    if(connard is PlayerEntity)
                        GameManager.Instance.playerInventory.playerHealth.health = Mathf.RoundToInt(connard.stats.currentHealth);
                await Victory();
            }
        }

        if(entity is PlayerEntity)
            await GameOver();
    }

    #endregion

    public static int TotalEncounteredCombatsCountOverRun = 0;
    
    private async void Start()
    {
        await UniTask.Yield();
        if (_startGameOnSceneStart)
            await Play();
    }

    public async UniTask Play()
    {
        if(_summonEntities)
            SummonEntities();

        for (; ; )
        {
            //player entities
            for (int i = 0; i < PlayerEntities.Count; i++)
            {
                Entity player = PlayerEntities[i];
                if (player == null) continue;

                foreach (Entity e in Entities) e.StopPreviewingSpellEffect();
                
                OnNewTurn?.Invoke(player);
                await player.PlayTurn();
                
            }

            //enemy entities
            for (int i = 0; i < EnemyEntities.Count; i++)
            {
                Entity enemy = EnemyEntities[i];
                if (enemy == null) continue;

                foreach (Entity e in Entities) e.StopPreviewingSpellEffect();

                OnNewTurn?.Invoke(enemy);
                await enemy.PlayTurn();
            }

            //cleanup corpses
            foreach (Entity player in PlayerEntities) if (player.isDead) Destroy(player);
            foreach (Entity e in EnemyEntities) if (e.isDead) Destroy(e);

            await UniTask.Yield();
        }
    }

    void SummonEntities()
    {
        int ennemiesCount = ComputeEnnemiesCount();

        SpawnSetup choosenSetup = Setups.PickRandom();

        choosenSetup.playerSpawner.SummonEntity();

        List<Spawner> spawners = new();

        foreach (Spawner spawner in choosenSetup.Spawners)
            spawners.Add(spawner);

        //m�langer les spawners ?

        //check si _min ennemies > spawners
        if (ennemiesCount > spawners.Count)
            foreach (Spawner spawner in spawners)
                spawner.SummonEntity();

        for (int i = 0; i < ennemiesCount; i++)
        {
            print(ennemiesCount); // 3
            print(spawners.Count); // 4 // 3 // 2

            Spawner choosenSpawner = spawners.PickRandom();

            choosenSpawner.SummonEntity();
            spawners.Remove(choosenSpawner);
        }
    }

    int ComputeEnnemiesCount()
    {
        int positionMap = PlayerMap.Instance?.PositionMap??  0;
        if (positionMap > 9) { return 4; }
        else if (positionMap > 6) { return 3; }
        else if(positionMap > 3) { return 2; }
        else { return 1; }
    }

    async UniTask GameOver()
    {
        await UniTask.Delay(1000);
        
        SaveManager.DeleteAll();
        
        if(PlaytestDataRecorder.Instance !=null)
            await PlaytestDataRecorder.Instance.OnGameOver();
        
        _gameOverPanel?.Show();
    }

    async UniTask Victory()
    {
        print("VICTORY");
        print(GameManager.Instance.playerInventory.playerHealth.health);

        await UniTask.Delay(1000);

        _rewardPanel?.Show();
        Time.timeScale = 1;
        //await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
