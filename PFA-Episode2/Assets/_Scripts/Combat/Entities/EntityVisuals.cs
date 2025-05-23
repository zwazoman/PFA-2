using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    Entity owner;

    [SerializeField] public Transform VisualsRoot;

    [SerializeField] public Animator animator;
    [SerializeField] bool _enableAnimations = true;

    List<PooledObject> Arrows = new();

    private void Awake()
    {
        TryGetComponent(out owner);

        if (animator == null)
            animator = VisualsRoot.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        owner.stats.healthFeedbackTasks.Add(OnHealthUpdated);

        //spell preview
        owner.OnPreviewSpell += (float newShield, float newHP, Vector3 direction) =>
        {
            //try
            //{
            foreach (PooledObject obj in Arrows)
            {
                obj.GoBackIntoPool();
            }
            Arrows.Clear();

            bool diagonal = Mathf.RoundToInt(direction.x) != 0 && Mathf.RoundToInt(direction.z) != 0;

            int arrowNumber = Mathf.RoundToInt(direction.magnitude);
            if (diagonal)
                arrowNumber = Mathf.RoundToInt(direction.magnitude / (float)Math.Sqrt(2));

            for (int i = 1; i < arrowNumber + 1; i++)
            {
                Vector3 pose = transform.position + direction.normalized * i;
                if (diagonal)
                    pose = transform.position + direction.normalized * i * Mathf.Sqrt(2);
                pose.y = 0.7f;

                PooledObject o = PoolManager.Instance.ArrowPool
                    .PullObjectFromPool(pose, transform)
                    .GetComponent<PooledObject>();


                float angle = Mathf.Atan2(direction.z, -direction.x) * Mathf.Rad2Deg;
                o.transform.rotation = Quaternion.Euler(-90, angle, 0);
                Arrows.Add(o);
                o.transform.position = pose;
            }
            //if(direction!=Vector3.zero)EditorApplication.isPaused = true;
            // }catch(Exception ex) { Debug.LogException(ex); }

        };

        owner.OnSpellPreviewCancel += () =>
        {
            foreach (PooledObject obj in Arrows)
            {
                obj.GoBackIntoPool();
            }
            Arrows.Clear();

        };
    }

    public async UniTask DeathAnimation()
    {
        await animator.PlayAnimationTrigger(owner.deathTrigger);

        print("connard");

        gameObject.SetActive(false);
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
