using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

//controle la camera pendant les combats.
public class CameraBehaviour : MonoBehaviour
{

    Vector3 basePosition;

    private async void Start()
    {
        basePosition = transform.position;

        CombatManager.Instance.OnNewTurn += OnNewTurn;

        await Awaitable.NextFrameAsync();

        foreach(Entity e in CombatManager.Instance.PlayerEntities)
        {
            if(e is PlayerEntity )
            {
                e.stats.healthFeedbackTasks.Add(OnPlayerHit);
            }
        }
    }

    private async UniTask OnPlayerHit(float delta, float newHealth)
    {
        if(delta < 0 && Time.timeSinceLevelLoad > 1)
        {
            transform.DOShakePosition(.3f, .4f);
        }

        await UniTask.Yield();
        return;
    }


    void OnNewTurn(Entity e)
    {
        if(e is PlayerEntity)
        {
            transform.DOMove(basePosition, .8f).SetEase(Ease.InOutSine);
        }
        else
        {
            transform.DOMove(basePosition + Vector3.up*1f, .8f).SetEase(Ease.InOutSine);
        }
    }

}
