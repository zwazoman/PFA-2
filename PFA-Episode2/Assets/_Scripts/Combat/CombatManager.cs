using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

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
 
    public bool IsPlaying = false;

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
            
            //victoire : tous les ennemis sont morts
            if (EnemyEntities.Count == 0)
            {
                foreach(Entity connard in PlayerEntities)
                    if(connard is PlayerEntity)
                        GameManager.Instance.playerInventory.playerHealth.health = Mathf.RoundToInt(connard.stats.currentHealth);
                await Victory();
            }
        }

        //game over
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
        await UniTask.NextFrame();
        
        IsPlaying = true;
        Debug.Log("started playing");
        while( Entities.Count>1 )
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
            foreach (Entity player in PlayerEntities)
            {
                if (player.isDead)
                {
                    Destroy(player);
                }
            }

            foreach (Entity e in EnemyEntities)
            {
                if (e.isDead)
                {
                    Destroy(e);
                }
            }

            await UniTask.Yield();
        }
        Debug.Log("finished playing");
        IsPlaying = false;
    }

    async void SummonEntities()
    {
        Debug.Log("spawn ennemies");
        int ennemiesCount = GameManager.Instance.ComputeEnnemiesCount();
        
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Forest_Combat_Boss") { ennemiesCount = 1; }
        Debug.Log("enemy count : " + ennemiesCount);
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
            Spawner choosenSpawner = spawners.PickRandom();

            choosenSpawner.SummonEntity();
            spawners.Remove(choosenSpawner);
            //await UniTask.Delay(300);
        }
    }

    async UniTask GameOver()
    {
        await UniTask.Delay(1000);
        
        SaveManager.DeleteAll();
        
        if(PlaytestDataRecorder.Instance !=null)
            await PlaytestDataRecorder.Instance.OnGameOver();

        MusicManager.Instance.ChangeVolume(0, 1);
        SFXManager.Instance.PlaySFXClip(Sounds.GameOverJingle);
        _gameOverPanel?.Show();
    }

    async UniTask Victory()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Forest_Combat_Tuto") { await SetupFight.Instance.Victory(); }

        await UniTask.Delay(1000);

        MusicManager.Instance.ChangeMusic("WorldMap");
        SFXManager.Instance.PlaySFXClip(Sounds.VictoryJingle);
        SFXManager.Instance.PlaySFXClip(Sounds.UiTwinkle);
        _rewardPanel?.Show();
        Time.timeScale = 1;
        //await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
