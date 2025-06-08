using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

//controle la camera pendant les combats.
public class CameraBehaviour : MonoBehaviour
{

    Vector3 basePosition;
    private Vector3 TargetPosition;
    private Vector3 yOffset;
    [SerializeField] Transform _repereWorldCenter;
    [SerializeField] Camera _cam;

    [SerializeField] private float _offsetMagnitude,_dampingDuration;

    private Vector3 vel;
    private async void Start()
    {
        _repereWorldCenter.transform.parent = null;
        basePosition = transform.position;
        TargetPosition = basePosition;
        
        CombatManager.Instance.OnNewTurn += OnNewTurn;

        await Awaitable.NextFrameAsync();

        foreach(Entity e in CombatManager.Instance.PlayerEntities)
        {
            if(e is PlayerEntity )
            {
                e.stats.OnHealthUpdated+=(OnPlayerHit);
            }
        }
    }

    private void Update()
    {
        Bounds bounds = new();
        foreach (Entity e in CombatManager.Instance.Entities)
        {
            bounds.Encapsulate(e.transform.position);
        }

        Vector3 offset = (bounds.center - _repereWorldCenter.position).XZ();
        TargetPosition = basePosition + offset * _offsetMagnitude;
        
        //transform.position = 
    }

    private void OnPlayerHit(float delta,float newValue )
    {
        if(delta < 0 && Time.timeSinceLevelLoad > 1)
        {
            transform.DOShakePosition(.3f, .4f);
        }
    }


    void OnNewTurn(Entity e)
    {
        if(e is PlayerEntity)
        {
            yOffset = Vector3.zero;
            //transform.DOMove(TargetPosition, .8f).SetEase(Ease.InOutSine);
            _cam.DOOrthoSize(3.6f,.8f);
        }
        else
        {
            yOffset =  Vector3.up*1f;
            //transform.DOMove(TargetPosition + Vector3.up*1f, .8f).SetEase(Ease.InOutSine);
            _cam.DOOrthoSize(4,.8f);
        }
    }

}
