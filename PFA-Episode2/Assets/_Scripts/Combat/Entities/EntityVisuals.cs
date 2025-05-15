using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    Entity owner;

    [SerializeField] Transform VisualsRoot;

    List<PooledObject> Arrows = new();

    private void Awake()
    {
        TryGetComponent(out owner);
    }

    private void Start()
    {
        owner.stats.healthFeedbackTasks.Add(OnHealthUpdated);

        //spell preview
        owner.OnPreviewSpell += (float newShield, float newHP, Vector3 direction) =>
        {
            try
            {
                for (int i = 1; i < Mathf.RoundToInt(direction.magnitude) + 1; i++)
                {
                    PooledObject o = PoolManager.Instance.ArrowPool
                        .PullObjectFromPool(transform.position + direction.normalized * i, transform)
                        .GetComponent<PooledObject>();

                    o.transform.right = direction.normalized;
                    Arrows.Add(o);
                }
            }catch(Exception ex) { Debug.LogException(ex); }
            
        };

        owner.OnSpellPreviewCancel += () =>
        {
            foreach(PooledObject obj in Arrows)
            {
                obj.GoBackIntoPool();
            }
        };
    }

    async UniTask OnHealthUpdated(float delta, float newValue)
    {
        if (Time.timeSinceLevelLoad < 1) return;
        if (delta < 0) //damage
        {
            await VisualsRoot.DOShakePosition(.5f, .2f).AsyncWaitForCompletion().AsUniTask();
        }
        else //heal
        {
            await VisualsRoot.DOPunchScale(Vector3.one * .2f, .5f, 5).AsyncWaitForCompletion().AsUniTask(); ;
        }
    }
}
