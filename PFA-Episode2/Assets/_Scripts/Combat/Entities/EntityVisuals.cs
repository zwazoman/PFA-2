using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    Entity owner;

    [Header("Scene References")]
    [SerializeField] public Transform VisualsRoot;
    [SerializeField] public Animator animator;

    [Header("VFXs")]
    [SerializeField] private ParticleSystem _hitParticles;
    [SerializeField] private ParticleSystem  _wallHitParticles;
    [SerializeField] private ParticleSystem  _runParticles;

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
        //walk particle
        owner.OnMovement += (bool b) => {
            try
            {
                if (b) _runParticles.Play();
                else _runParticles.Stop();
            }catch(Exception e) { Debug.LogException(e); }
        };
        
        //health update
        owner.stats.healthFeedbackTasks.Add(OnHealthUpdated);
        
        //pushDamage vfx
        owner.OnPushDamageTaken += () => _wallHitParticles.Play();

        //spell preview
        owner.OnPreviewSpell += (float newShield, float newHP, Vector3 direction) =>
        {
            //arrows
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

    public async UniTask PlayDeathAnimation()
    {
        
        GameObject vfx = null;
        
        try
        {
            //sfx
            SFXManager.Instance.PlaySFXClip(Sounds.EntityDeath);
            
            //vfx
            try { vfx = PoolManager.Instance.vfx_GodrayPool.PullObjectFromPool(transform.position,Quaternion.Euler(-90,0,0)); }
            catch (Exception e) { Debug.LogException(e); }
            
            //animation
            await animator.PlayAnimationTrigger(Entity.deathTrigger);
            
        } catch (Exception e) { Debug.LogException(e); }
        
        //0 scale
        owner.eatSocket.transform.DOMoveY( 20, 0.2f);
        await owner.eatSocket.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutBack).ToUniTask();

        //kill vfx
        if(vfx) vfx.GetComponent<PooledObject>().GoBackIntoPool();
        
        gameObject.SetActive(false);
    }

    async UniTask OnHealthUpdated(float delta, float newValue)
    {
        if (Time.timeSinceLevelLoad < 1) return;
        if (delta < 0) //damage
        {
            _hitParticles.Play();
            await VisualsRoot.DOShakePosition(.25f, .3f,15).AsyncWaitForCompletion().AsUniTask();
            SFXManager.Instance.PlaySFXClip(Sounds.CombatHit);
        }
        else //heal
        {
            await VisualsRoot.DOPunchScale(Vector3.one * .2f, .5f, 5).AsyncWaitForCompletion().AsUniTask(); ;
        }
    }


}
