using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;
using Cysharp.Threading.Tasks.Triggers;

[RequireComponent(typeof(SpellCaster))]
public class Entity : MonoBehaviour
{
    public Team team = Team.Enemy;
    public bool isDead { get; private set; }

    public EntityStats stats = new();


    [Header("Asset References")]
    public Sprite Icon;

    [Header("Scene References")]
    public EntityVisuals visuals;
    public Transform eatSocket;

    //navigation
    protected Dictionary<WayPoint, int> WaypointDistance = new Dictionary<WayPoint, int>();
    protected List<WayPoint> Walkables = new List<WayPoint>();
    protected List<Spell> spells = new();

    //spells
    [HideInInspector] public WayPoint currentPoint;
    [HideInInspector] public SpellCaster entitySpellCaster;

    //events
    public event Action OnDead;
    /// <summary>
    /// float newShield,float newHP,Vector3 direction
    /// </summary>
    public event Action<float,float,Vector3> OnPreviewSpell;
    public event Action OnSpellPreviewCancel;
    public event Action OnPushDamageTaken;
    public event Action<bool> OnMovement;
    

    #region AnimationTriggers
    [HideInInspector] public const string moveBool = "Move";
    [HideInInspector] public const string attackTrigger = "Attack";
    [HideInInspector] public const string idleTrigger = "Idle";
    [HideInInspector] public const string pushBool = "Push";
    [HideInInspector] public const string hitTrigger = "Hit";
    [HideInInspector] public const string deathTrigger = "Death";

    #endregion


    protected virtual void Awake()
    {
        TryGetComponent(out entitySpellCaster);
        TryGetComponent(out visuals);
        stats.owner = this;
    }

    protected virtual void Start()
    {
        //set up position on graph
        Vector3Int roundedPos = transform.position.SnapOnGrid();
        try
        {
            currentPoint = GraphMaker.Instance.serializedPointDict[roundedPos];
        }
        catch(Exception e) { Debug.LogException(e); }

        currentPoint.StepOn(this);
    }

    // game management
    public virtual async UniTask PlayTurn()
    {
        SFXManager.Instance.PlaySFXClip(Sounds.TurnChange);
        Tools.Flood(currentPoint);
        stats.currentMovePoints = stats.maxMovePoints;
        await stats.ApplyShield(-1);
        
        TickCooldown();
    }

    public virtual async UniTask EndTurn()
    {
        Tools.ClearFlood();
        ClearWalkables();
    }

    void TickCooldown()
    {
        foreach(Spell spell in spells)
        {
            spell.TickSpellCooldown();
        }
    }

    //spell preview
    public void PreviewSpellEffect(BakedTargetedSpellEffect e)
    {
        float newShield = stats.shieldAmount + e.shield;

        newShield = Mathf.Max(0, newShield - e.damage);

        float tankedDamage = Mathf.Abs(newShield - stats.shieldAmount);
        float damage = Mathf.Max(e.damage + e.pushDamage - tankedDamage, 0);

        float newHP = stats.currentHealth - damage;


        OnPreviewSpell?.Invoke(newShield, newHP,e.pushPoint != null ? e.pushPoint.transform.position-transform.position : Vector3.zero);
    }

    public void StopPreviewingSpellEffect()
    {
        OnSpellPreviewCancel?.Invoke();
    }

    //spell effect
    public async UniTask ApplySpell(BakedTargetedSpellEffect effect)
    {
        try
        {
            if (effect.shield != 0) await stats.ApplyShield(effect.shield);
            if (effect.damage != 0 && effect.pushPoint == null) await stats.ApplyDamage(effect.damage);
            if (effect.pushPoint != null) await Push(Mathf.RoundToInt(effect.damage),Mathf.RoundToInt(effect.pushDamage ), effect.pushPoint);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    //walkable tiles
    public void ApplyWalkables(bool showTiles = true)
    {
        if (Walkables.Count == 0)
            Walkables.AddRange(Tools.SmallFlood(currentPoint, stats.currentMovePoints, true, true).Keys);

        foreach (WayPoint point in Walkables)
        {
            if (point.State == WaypointState.Free)
            {
                point.SetPreviewState(WayPoint.PreviewState.Movement);
            }
        }
    }

    public void HideWalkables()
    {
        foreach (WayPoint point in Walkables)
        {
            point.SetPreviewState(WayPoint.PreviewState.NoPreview);
        }
    }

    public void ClearWalkables()
    {
        foreach (WayPoint point in Walkables)
        {
            point.SetPreviewState(WayPoint.PreviewState.NoPreview);
        }
        Walkables.Clear();
    }

    //recoil
    async UniTask Push(int baseDamage,int pushDamages, WayPoint pushTarget)
    {
        currentPoint.StepOff();

        if(pushTarget != currentPoint)
        {
            visuals.animator.PlayAnimationBool(pushBool);
            //await UniTask.Delay(200);

            await StartMoving(pushTarget.transform.position,13,-1);

            visuals.animator.EndAnimationBool(pushBool);
        }

        if (pushDamages > 0)
            OnPushDamageTaken?.Invoke();
        
        await stats.ApplyDamage(pushDamages + baseDamage);
        
        if(!isDead) 
            pushTarget.StepOn(this);
    }

    public List<Entity> GetEnemyList()
    {
        List<Entity> enemyEntities = new List<Entity>();

        if (team == Team.Enemy)
            foreach (Entity entity in CombatManager.Instance.PlayerEntities)
                enemyEntities.Add(entity);
        else if(team == Team.Player)
            foreach (Entity entity in CombatManager.Instance.EnemyEntities)
                enemyEntities.Add(entity);

        return enemyEntities;
    }

    public List<Entity> GetAllyEntities()
    {
        List<Entity> enemyEntities = new List<Entity>();

        if (team == Team.Player)
            foreach (Entity entity in CombatManager.Instance.PlayerEntities)
                enemyEntities.Add(entity);
        else if (team == Team.Enemy)
            foreach (Entity entity in CombatManager.Instance.EnemyEntities)
                enemyEntities.Add(entity);

        return enemyEntities;
    }

    //movement

    /// <summary>
    /// fait se d�placer l'entit� vers la case la plus proche de la target
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask<bool> MoveToward(WayPoint targetPoint)
    {
        ApplyWalkables(true);

        await UniTask.Delay(300);

        if (targetPoint == currentPoint)
            return true;

        if (Walkables.Contains(targetPoint) && targetPoint.State == WaypointState.Free)
        {
            print("target in range !");
            await TryMoveTo(targetPoint);
            return true;
        }

        await UniTask.Delay(400);

        WayPoint moveToPoint;

        Walkables.FindClosestFloodPoint(out moveToPoint, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint], false, true));

        await TryMoveTo(moveToPoint);
        return false;
    }

    /// <summary>
    /// fait se d�plcaer l'entit� le plus loin possible de l'entit� cibl�e
    /// </summary>
    /// 
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask MoveAwayFrom(WayPoint targetPoint)
    {
        float distanceToFurthestPoint = 0;
        WayPoint furthestPoint = null;

        foreach (WayPoint point in Walkables)
        {
            float pointDistanceToTarget = Tools.FloodDict[point] + Tools.FloodDict[targetPoint];

            if (pointDistanceToTarget > distanceToFurthestPoint)
            {
                distanceToFurthestPoint = pointDistanceToTarget;
                furthestPoint = point;
            }
        }

        print(furthestPoint.transform.position);
        await TryMoveTo(furthestPoint);
    }
    public virtual async UniTask TryMoveTo(WayPoint targetPoint, bool showTiles = true)
    {
        Stack<WayPoint> path = Tools.FindBestPath(currentPoint, targetPoint);
        int pathlength = path.Count;

        if (pathlength > stats.currentMovePoints)
        {
            return;
        }

        foreach (WayPoint p in path)
        {
            p.SetPreviewState(WayPoint.PreviewState.Movement);
        }

        visuals.animator.PlayAnimationBool(moveBool);
        
        OnMovement?.Invoke(true);
        
        for (int i = 0; i < pathlength; i++)
        {
            currentPoint.StepOff();

            WayPoint steppedOnPoint = path.Pop();

            await StartMoving(steppedOnPoint.transform.position);

            currentPoint = steppedOnPoint;
            steppedOnPoint.StepOn(this);

            steppedOnPoint.SetPreviewState(WayPoint.PreviewState.NoPreview);

            stats.currentMovePoints--;
        }

        OnMovement?.Invoke(false);
        visuals.animator.EndAnimationBool(moveBool);
        Tools.Flood(currentPoint);
        ClearWalkables();
        ApplyWalkables(showTiles);
    }


    async UniTask StartMoving(Vector3 targetPos, float moveSpeed = 5, float rotmultiplyer = 1)
    {
        targetPos.y = transform.position.y;
        Vector3 offset = targetPos - (Vector3)transform.position;
        float rotation = 90f * rotmultiplyer;
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(-offset.z, offset.x) * Mathf.Rad2Deg + rotation, 0);
        visuals.VisualsRoot.DORotateQuaternion(targetRotation, 1f / moveSpeed);
        while ((Vector3)transform.position != targetPos)
        {
            Vector3 offset2 = targetPos - (Vector3)transform.position;
            offset2 = Vector3.ClampMagnitude(offset2, Time.deltaTime * moveSpeed);
            transform.Translate(offset2, Space.World);
            await Task.Yield();
        }
    }

    public async UniTask LookAt(WayPoint point, float speed = .2f)
    {
        Vector3 lookPos = new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z);
        await visuals.VisualsRoot.DOLookAt(lookPos, speed);
    }

    //death
    public async UniTask Die()
    {
        //visuals.PlayDeathAnimation();
        OnDead?.Invoke();

        //if (this is PlayerEntity || team == Team.Enemy && CombatManager.Instance.EnemyEntities.Count == 1)
        Debug.Log("about to play death animation");
        await visuals.PlayDeathAnimation();

        currentPoint.StepOff();
        isDead = true;

        await CombatManager.Instance.UnRegisterEntity(this);

        EndTurn();
    }
}
