using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

//controle la camera pendant les combats.
public class CameraBehaviour : MonoBehaviour
{

    Vector3 basePosition;

    private void Start()
    {
        basePosition = transform.position;

        CombatManager.Instance.OnNewTurn += OnNewTurn;
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
