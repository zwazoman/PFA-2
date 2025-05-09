using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    public List<PlayerEntity> PlayerEntities = new();
    public List<EnemyEntity> EnemyEntities = new();

    #region entity registration

    public void RegisterEntity(Entity entity)
    {
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
        await StartGame();
    }

    public async UniTask StartGame()
    {
        for (; ; )
        {
            for (int i = 0; i < PlayerEntities.Count; i++)
            {
                PlayerEntity player = PlayerEntities[i];
                if (player == null) continue;
                await player.PlayTurn();
            }

            for (int i = 0; i < EnemyEntities.Count; i++)
            {
                EnemyEntity enemy = EnemyEntities[i];
                if (enemy == null) continue;
                await enemy.PlayTurn();
            }

            await UniTask.Yield();
        }
    }

    async UniTask GameOver()
    {
        print("Game Over");
        Application.Quit();
    }

    async UniTask Victory()
    {
        print("Victory");
        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }

}
