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

    public List<PlayerEntity> PlayerEntities = new List<PlayerEntity>();
    
    public List<EnemyEntity> EnemyEntities = new List<EnemyEntity>();

    private async void Start()
    {
        await UniTask.Yield();
        StartGame();
    }

    public async void StartGame()
    {
        while (true)
        {
            foreach (PlayerEntity playerEntity in PlayerEntities)
            {
                await playerEntity.PlayTurn();
            }
            foreach (EnemyEntity enemy in EnemyEntities)
            {
                await enemy.PlayTurn();
            }

            await UniTask.Yield();
        }
    }
}
