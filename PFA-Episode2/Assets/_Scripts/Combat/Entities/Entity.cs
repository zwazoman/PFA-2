using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

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

    public async UniTask ApplySpell(Spell spell, SpellCastingContext context)
    {
        print("applySpell");

        foreach (SpellEffect effect in spell.spellData.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    await stats.ApplyDamage(effect.value);
                    break;
                case SpellEffectType.Recoil:
                    await Push(effect.value, context.PushDirection, context.casterPos);
                    break;
                case SpellEffectType.Shield:
                    await stats.ApplyShield(effect.value);
                    break;
                case SpellEffectType.DamageIncreaseForEachHitEnnemy:
                    throw new NotImplementedException();
                case SpellEffectType.DamageIncreasePercentageByDistanceToCaster:
                    throw new NotImplementedException();
                case SpellEffectType.Fire:
                    throw new NotImplementedException();
            }
        }
    }

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

    async UniTask Push(float pushForce, Vector3 pushDirection, Vector3 casterPos)
    {
        Debug.DrawLine(transform.position, transform.position + pushDirection, Color.red, 20);

        WayPoint choosenPoint = null;
        float damages = 0;

        Vector3 posWithHeigth = transform.position + Vector3.up * 0.2f;

        bool isDiagonal = pushDirection.x != 0 && pushDirection.z != 0;

        if (pushDirection == Vector3.zero)
        {
            Vector3 casterPosWithHeight = casterPos + Vector3.up * 0.2f;
            Vector3 casterToEntity = posWithHeigth - casterPos;

            if (casterToEntity == Vector3.zero)
                return;

            int xPushDirection = casterToEntity.x != 0 ? (int)Mathf.Sign(casterToEntity.x) : 0;
            int zPushDirection = casterToEntity.z != 0 ? (int)Mathf.Sign(casterToEntity.z) : 0;

            pushDirection = new Vector3(xPushDirection, 0, zPushDirection);
        }
            
        if (isDiagonal)
        {
            RaycastHit hit;
            if (Physics.SphereCast(posWithHeigth, .45f, pushDirection, out hit, pushForce * Mathf.Sqrt(2), LayerMask.GetMask("Wall")))
            {
                print("wall hit");

                Debug.DrawLine(transform.position, hit.point, Color.black, 20);
                damages = pushForce;
                Vector3 hitPos = hit.point/*.SnapOnGrid()*/;

                Debug.DrawLine(hitPos, (hitPos - pushDirection * .3f).SnapOnGrid(),Color.blue, 20);

                damages -= Mathf.FloorToInt(hit.distance);
                choosenPoint = GraphMaker.Instance.serializedPointDict[(hitPos - pushDirection * .3f).SnapOnGrid()];
            }
            else
            {
                Debug.DrawLine(posWithHeigth, posWithHeigth + (pushDirection * pushForce), Color.black, 20);
                Vector3Int choosenPos = (posWithHeigth + (pushDirection * pushForce)).SnapOnGrid();
                choosenPoint = GraphMaker.Instance.serializedPointDict[choosenPos];
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(posWithHeigth, pushDirection, out hit, pushForce, LayerMask.GetMask("Wall")))
            {
                damages = pushForce;

                Debug.DrawLine(posWithHeigth, hit.point, Color.black, 20);

                Vector3 hitPos = hit.point;

                Debug.DrawLine(hitPos, hitPos - pushDirection * .3f, Color.blue, 20);

                damages -= Mathf.FloorToInt(hit.distance);
                choosenPoint = GraphMaker.Instance.serializedPointDict[(hitPos - pushDirection * .3f).SnapOnGrid()];
            }
            else
            {
                choosenPoint = GraphMaker.Instance.serializedPointDict[(posWithHeigth + pushDirection * pushForce).SnapOnGrid()];
            }
        }

        print(damages);
        print(choosenPoint.gameObject.name);

        choosenPoint.ChangeTileColor(choosenPoint._walkedMaterial);

        currentPoint.StepOff();

        await StartMoving(choosenPoint.transform.position,20);

        if (damages > 0)
            await stats.ApplyDamage(damages);

        choosenPoint.StepOn(this);
    }

    /// <summary>
    /// fait se déplacer l'entité vers la case la plus proche de la target
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask<bool> MoveToward(WayPoint targetPoint)
    {
        print("move !");

        await UniTask.Delay(1000);

        if (targetPoint == currentPoint)
            return true;

        if (Walkables.Contains(targetPoint) && targetPoint.State == WaypointState.Free)
        {
            print("target in range !");
            await TryMoveTo(targetPoint);
            return true;
        }

        await TryMoveTo(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint])));
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

    public async UniTask Die()
    {
        print("Die");

        currentPoint.StepOff();
        Destroy(gameObject);
        await CombatManager.Instance.UnRegisterEntity(this);
        //si il ets entrain de jouer EndTurn
    }
}
