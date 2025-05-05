using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(SpellCaster))]
public class Entity : MonoBehaviour
{
    public EntityStats entityStats = new();

    [HideInInspector] public WayPoint currentPoint;
    [HideInInspector] public SpellCaster entitySpellCaster;

    protected Dictionary<WayPoint,int> WaypointDistance = new Dictionary<WayPoint,int>();
    protected List<WayPoint> Walkables = new List<WayPoint>();

    protected virtual void Awake()
    {
        TryGetComponent(out entitySpellCaster);
    }

    protected virtual void Start()
    {
        Vector3Int roundedPos = transform.position.SnapOnGrid();
        //transform.position = roundedPos;
        //transform.position += Vector3.up * 1.3f;

        currentPoint = GraphMaker.Instance.serializedPointDict[roundedPos];
        currentPoint.StepOn(this);
    }

    public virtual async UniTask PlayTurn()
    {
        print(gameObject.name);
        Tools.Flood(currentPoint);
        entityStats.currentMovePoints = entityStats.maxMovePoints;
    }

    public virtual async UniTask EndTurn()
    {
        Tools.ClearFlood();
        ClearWalkables();
    }

    public async UniTask ApplySpell(SpellData spell)
    {
        foreach (SpellEffect effect in spell.Effects)
        {
            {
                switch (effect.effectType)
                {
                    case SpellEffectType.Damage:
                        entityStats.ApplyHealth(-effect.value);
                        break;
                    case SpellEffectType.Recoil:
                        throw new NotImplementedException();
                    case SpellEffectType.Shield:
                        entityStats.ApplyShield(effect.value);
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
    }
    public virtual async UniTask TryMoveTo(WayPoint targetPoint, bool showTiles = true)
    {
        Stack<WayPoint> path = Tools.FindBestPath(currentPoint, targetPoint);
        int pathlength = path.Count;

        if(pathlength > entityStats.currentMovePoints)
        {
            print("plus de pm !");
            return;
        }

        foreach(WayPoint p in path)
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

            entityStats.currentMovePoints--;
        }
        Tools.Flood(currentPoint);
        ClearWalkables();
        ApplyWalkables(showTiles);
    }

    async UniTask StartMoving(Vector3 targetPos, float moveSpeed = 8)
    {
        targetPos.y = transform.position.y;
        Vector3 offset = targetPos - (Vector3)transform.position;
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg, 0);
        transform.rotation = targetRotation;
        while ((Vector3)transform.position != targetPos)
        {
            Vector3 offset2 = targetPos - (Vector3)transform.position;
            offset2 = Vector3.ClampMagnitude(offset2, Time.deltaTime * moveSpeed);
            transform.Translate(offset2, Space.World);
            await Task.Yield();
        }
    }

    public void ApplyWalkables(bool showTiles = true)
    {
        print(entityStats.currentMovePoints);

        if(Walkables.Count == 0)
            Walkables.AddRange(Tools.GetWaypointsInRange(entityStats.currentMovePoints));

        foreach (WayPoint point in Walkables)
        {
            if(point.State == WaypointState.Free)
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

    /// <summary>
    /// fait se déplacer l'entité vers la case la plus proche de la target
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask<bool> MoveToward(WayPoint targetPoint)
    {
        await UniTask.Delay(1000);

        if (targetPoint == currentPoint)
            return true;

        if (Walkables.Contains(targetPoint))
        {
            print("target in range !");
            await TryMoveTo(targetPoint);
            return true;
        }

        print("target not in range yet ! getting closer...");
        print(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint])));

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
}
