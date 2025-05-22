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
    }
    #endregion

    [HideInInspector] public List<PlayerEntity> PlayerEntities = new();
    [HideInInspector] public List<EnemyEntity> EnemyEntities = new();

    [SerializeField] public List<SpawnSetup> Setups = new();
    [SerializeField] int _minEnnemiesCount;
    [SerializeField] float _excendentSpawnProba = .3f;

    public List<Entity> Entities { get; private set; } = new();

    [SerializeField] bool _summonEntities;
    [SerializeField] bool _startGameOnSceneStart = false;

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
        await UniTask.Yield();
        if (_startGameOnSceneStart)
            await StartGame();
    }

    public async UniTask StartGame()
    {
        SummonEntities();

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
        SpawnSetup choosenSetup = Setups.PickRandom();

        choosenSetup.playerSpawner.SummonEntity();

        List<Spawner> spawners = new();

        foreach (Spawner spawner in choosenSetup.Spawners)
            spawners.Add(spawner);

        //mélanger les spawners ?

        //check si _min ennemies > spawners
        if (_minEnnemiesCount > spawners.Count)
            throw new Exception("pas assez de spawners");

        for (int i = 0; i < _minEnnemiesCount; i++)
        {
            spawners[i].SummonEntity();
            spawners.Remove(spawners[i]);
        }

        foreach (Spawner spawner in spawners)
            if (UnityEngine.Random.value <= _excendentSpawnProba)
                spawner.SummonEntity();
    }

    async UniTask GameOver()
    {
        print("Game Over");
        SaveManager.DeleteAll();
        _gameOverPanel?.Show();
    }

    async UniTask Victory()
    {
        OnWin?.Invoke();
        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }

}
