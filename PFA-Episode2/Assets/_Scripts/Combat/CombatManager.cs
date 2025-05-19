using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;

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
    }
    #endregion

    [HideInInspector] public List<PlayerEntity> PlayerEntities = new();
    [HideInInspector] public List<EnemyEntity> EnemyEntities = new();

    [SerializeField] public List<SpawnSetup> Setups = new();

    public List<Entity> Entities { get; private set; } = new();

    [SerializeField] bool _summonEntities;
    [SerializeField] GameOverPanel _gameOverPanel;

    public event Action<Entity> OnNewTurn;
    public event Action OnWin;

    #region entity registration

    public void RegisterEntity(Entity entity)
    {
        Entities.Add(entity);
        if (entity is PlayerEntity)
        {
            PlayerEntities.Add((PlayerEntity)entity);
        }
        else if (entity is EnemyEntity)
        {
            EnemyEntities.Add((EnemyEntity)entity);
        }
    }

    public async UniTask UnRegisterEntity(Entity entity)
    {
        Entities.Remove(entity);
        if (entity is PlayerEntity && PlayerEntities.Contains(entity))
        {
            PlayerEntities.Remove((PlayerEntity)entity);
            await GameOver();
        }
        else if (entity is EnemyEntity && EnemyEntities.Contains(entity))
        {
            EnemyEntities.Remove((EnemyEntity)entity);
            if (EnemyEntities.Count == 0)
                await Victory();
        }
    }

    #endregion

    private async void Start()
    {
        SummonEntities();
        await UniTask.Yield();
        print(EnemyEntities.Count);
        await StartGame();
    }

    public async UniTask StartGame()
    {
        for (; ; )
        {
            //player entities
            for (int i = 0; i < PlayerEntities.Count; i++)
            {
                PlayerEntity player = PlayerEntities[i];
                if (player == null) continue;

                foreach (Entity e in Entities) e.StopPreviewingSpellEffect();

                OnNewTurn?.Invoke(player);
                await player.PlayTurn();


            }

            //enemy entities
            for (int i = 0; i < EnemyEntities.Count; i++)
            {
                EnemyEntity enemy = EnemyEntities[i];
                if (enemy == null) continue;

                foreach (Entity e in Entities) e.StopPreviewingSpellEffect();

                OnNewTurn?.Invoke(enemy);
                await enemy.PlayTurn();
            }

            //cleanup corpses
            foreach (PlayerEntity player in PlayerEntities) if (player.isDead) Destroy(player);
            foreach (EnemyEntity e in EnemyEntities) if (e.isDead) Destroy(e);

            await UniTask.Yield();
        }
    }

    void SummonEntities()
    {
        if (!_summonEntities)
            return;

        if (Setups.Count == 0)
        {
            throw new Exception("T'as pas setup les entit�s mon fr�re");
        }

        SpawnSetup setup = Setups[UnityEngine.Random.Range(0, Setups.Count)];
        setup.playerSpawner.SummonSingleEntity();
        foreach (EnemySpawnerGroup enemySpawnerGroup in setup.enemySpawnerGroups)
        {
            Spawner choosenSpawner = enemySpawnerGroup.spawners[UnityEngine.Random.Range(0, enemySpawnerGroup.spawners.Count)];
            choosenSpawner.SummonRandomEntity();
        }
        
    }

    async UniTask GameOver()
    {
        print("Game Over");
        SaveManager.DeleteAll();
        _gameOverPanel?.Show();
    }

    async UniTask Victory()
    {
        print("Victory");
        OnWin?.Invoke();
        //await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }

}
