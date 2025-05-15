using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UI;

[RequireComponent(typeof(SpellCaster))]
public class Entity : MonoBehaviour
{
    public EntityStats stats = new();

    [HideInInspector] public WayPoint currentPoint;
    [HideInInspector] public SpellCaster entitySpellCaster;

    protected Dictionary<WayPoint, int> WaypointDistance = new Dictionary<WayPoint, int>();
    protected List<WayPoint> Walkables = new List<WayPoint>();
    protected List<Spell> spells = new();

    public Sprite Icon;

    //events
    public event Action OnDead;

    /// <summary>
    /// float newShield,float newHP,Vector3 direction
    /// </summary>
    public event Action<float,float,Vector3> OnPreviewSpell;
    public event Action OnSpellPreviewCancel;

    public bool isDead { get; private set; }

    protected virtual void Awake()
    {
        TryGetComponent(out entitySpellCaster);
        stats.owner = this;
    }

    protected virtual void Start()
    {
        //set up position on graph
        Vector3Int roundedPos = transform.position.SnapOnGrid();
        currentPoint = GraphMaker.Instance.serializedPointDict[roundedPos];
        currentPoint.StepOn(this);
    }

    // game management
    public virtual async UniTask PlayTurn()
    {
        print(gameObject.name);
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
    public void previewSpellEffect(BakedSpellEffect e)
    {
        float newShield = stats.shieldAmount + e.shield;
        //Debug.Log("-new shield : " + newShield.ToString());

        newShield = Mathf.Max(0, newShield - e.damage);
        //Debug.Log("new shield : " + newShield.ToString());

        float tankedDamage = Mathf.Abs(newShield - stats.shieldAmount);
        float damage = Mathf.Max(e.damage - tankedDamage, 0);

        float newHP = stats.currentHealth - damage;
        

        OnPreviewSpell?.Invoke(newShield, newHP,e.pushPoint != null ? e.pushPoint.transform.position-transform.position : Vector3.zero);
    }

    public void StopPreviewingSpellEffect()
    {
        OnSpellPreviewCancel?.Invoke();
    }

    //spell effect
    public async UniTask ApplySpell(BakedSpellEffect effect)
    {
        if(effect.shield != 0) await stats.ApplyShield(effect.shield);
        if (effect.damage != 0) await stats.ApplyDamage(effect.damage);
        if (effect.pushPoint != null) await Push(Mathf.RoundToInt(effect.pushDamage), effect.pushPoint);
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
                point.ChangeTileColor(point._walkableMaterial);
            }
        }
    }

    public void ClearWalkables()
    {
        foreach (WayPoint point in Walkables)
        {
            point.ChangeTileColor(point._normalMaterial);
        }
        Walkables.Clear();
    }

    //recoil
    async UniTask Push(int pushDamages, WayPoint pushTarget)
    {
        print(pushDamages);

        currentPoint.StepOff();

        await StartMoving(pushTarget.transform.position,20);

        if (pushDamages > 0)
            await stats.ApplyDamage(pushDamages);

        pushTarget.StepOn(this);
    }

    //movement

    /// <summary>
    /// fait se déplacer l'entité vers la case la plus proche de la target
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

        //print(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint], true, true)));

        await UniTask.Delay(500);

        WayPoint moveToPoint;

        Walkables.FindClosestFloodPoint(out moveToPoint, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint], false, true));

        await TryMoveTo(moveToPoint);
        return false;
    }

    /// <summary>
    /// fait se déplcaer l'entité le plus loin possible de l'entité ciblée
    /// </summary>
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

        await TryMoveTo(furthestPoint);
    }
    public virtual async UniTask TryMoveTo(WayPoint targetPoint, bool showTiles = true)
    {
        Stack<WayPoint> path = Tools.FindBestPath(currentPoint, targetPoint);
        int pathlength = path.Count;

        if (pathlength > stats.currentMovePoints)
        {
            print("plus de pm !");
            return;
        }

        foreach (WayPoint p in path)
        {
            p.ChangeTileColor(p._walkedMaterial);
        }

        for (int i = 0; i < pathlength; i++)
        {
            currentPoint.StepOff();

            WayPoint steppedOnPoint = path.Pop();

            await StartMoving(steppedOnPoint.transform.position);

            currentPoint = steppedOnPoint;
            steppedOnPoint.StepOn(this);

            steppedOnPoint.ChangeTileColor(steppedOnPoint._normalMaterial);

            stats.currentMovePoints--;
        }
        Tools.Flood(currentPoint);
        ClearWalkables();
        ApplyWalkables(showTiles);
    }

    async UniTask StartMoving(Vector3 targetPos, float moveSpeed = 8)
    {
        targetPos.y = transform.position.y;
        Vector3 offset = targetPos - (Vector3)transform.position;
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(-offset.z, offset.x) * Mathf.Rad2Deg, 0);
        transform.DORotateQuaternion(targetRotation, 1f / moveSpeed);
        while ((Vector3)transform.position != targetPos)
        {
            Vector3 offset2 = targetPos - (Vector3)transform.position;
            offset2 = Vector3.ClampMagnitude(offset2, Time.deltaTime * moveSpeed);
            transform.Translate(offset2, Space.World);
            await Task.Yield();
        }
    }

    //death
    public async UniTask Die()
    {
        print("Die");
        
        currentPoint.StepOff();
        isDead = true;
        OnDead?.Invoke();
        gameObject.SetActive(false);
        await CombatManager.Instance.UnRegisterEntity(this);

        EndTurn();
    }
}
