using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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
    public void RegisterPlayerEntity(PlayerEntity playerEntity)
    {
        PlayerEntities.Add(playerEntity);
        RegisterEntity(playerEntity);
    }

    public void RegisterEnnemyEntity(EnemyEntity entity)
    {
        EnemyEntities.Add(entity);
        RegisterEntity(entity);
    }

    private void RegisterEntity(Entity entity)
    {

    }

    #endregion

    private async void Start()
    {
        await UniTask.Yield();
        await StartGame();
    }

    public async UniTask StartGame()
    {
        for(; ; )
        {
            foreach(PlayerEntity player in PlayerEntities)
            {
                if (player == null) continue;
                await player.PlayTurn();
            }
                
            foreach(EnemyEntity enemy in EnemyEntities)
            {
                if (enemy == null) continue;
                await enemy.PlayTurn();
            }

            await UniTask.Yield();
        }
    }
}
