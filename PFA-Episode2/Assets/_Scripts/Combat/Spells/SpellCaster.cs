using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity castingEntity;

    [SerializeField] LayerMask _obstacleMask;

    public const byte RangeRingThickness = 3;

    private void Awake()
    {
        if(castingEntity == null)
            TryGetComponent(out castingEntity);
    }

    public List<WayPoint> PreviewSpellRange(Spell spell, WayPoint center = null, bool showZone = true, bool ignoreTerrain = false)
    {
        if (center == null)
            center = castingEntity.currentPoint;

        Dictionary<WayPoint, int> floodDict = Tools.SmallFlood(center, spell.spellData.Range);

        List<WayPoint> rangePoints = new();

        foreach (WayPoint point in floodDict.Keys)
        {
            if ((!ignoreTerrain && (spell.spellData.IsOccludedByWalls && Tools.CheckWallsBetween(center, point) || point.State == WaypointState.Obstructed)) || spell.spellData.Range > RangeRingThickness && (floodDict[point] - RangeRingThickness) < 0)
                continue;
            else if (showZone)
                point.ChangeTileColor(point._rangeMaterial);

            rangePoints.Add(point);
        }

        return rangePoints;
    }

    public SpellZoneData PreviewSpellZone(Spell spell, WayPoint targetedPoint, List<WayPoint> rangePoints, bool showZone = true)
    {
        SpellZoneData zoneData = new();

        List<WayPoint> zonePoints = new();
        Dictionary<Entity, SpellCastingContext> hitEntityCTXDict = new();

        List<Entity> hitEntities = new();

        if (!rangePoints.Contains(targetedPoint))
        {
            return zoneData;
        }

        Vector3Int targetedPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetedPoint);

        foreach (Vector2Int pos in spell.spellData.AreaOfEffect.AffectedTiles)
        {
            Vector3Int posOffset = new Vector3Int(pos.x, 0, pos.y);
            Vector3Int newPos = targetedPointPos + posOffset;

            if (GraphMaker.Instance.serializedPointDict.ContainsKey(newPos))
            {
                WayPoint choosenWaypoint = GraphMaker.Instance.serializedPointDict[newPos];

                if (showZone)
                    choosenWaypoint.ChangeTileColor(choosenWaypoint._zoneMaterial);

                zonePoints.Add(choosenWaypoint);

                if (choosenWaypoint.State == WaypointState.HasEntity)
                    hitEntities.Add(choosenWaypoint.Content);
            }
        }
        zoneData.zonePoints = zonePoints;

        foreach (Entity entity in hitEntities)
        {
            SpellCastingContext context = new();

            context = ComputeCTX(spell, entity, hitEntities, targetedPoint);

            hitEntityCTXDict.Add(entity, context);

            zoneData.hitEntityCTXDict = hitEntityCTXDict;

            PreviewSpellEffect(spell, entity, ref zoneData);
        }

        return zoneData;
    }

    SpellCastingContext ComputeCTX(Spell spell, Entity hitEntity, List<Entity> hitEntities, WayPoint target)
    {
        Vector3Int entityTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(hitEntity.currentPoint);
        Vector3Int targetTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(target);
        Vector3Int targetToEntity = entityTilePos - targetTilePos;

        int xPushDirection = targetToEntity.x != 0 ? (int)Mathf.Sign(targetToEntity.x) : 0;
        int zPushDirection = targetToEntity.z != 0 ? (int)Mathf.Sign(targetToEntity.z) : 0;
        Vector3 pushDirection = new Vector3(xPushDirection, 0, zPushDirection);

        SpellCastingContext context = new();

        context.numberOfHitEnnemies = (byte)hitEntities.Count;
        context.distanceToPlayer = (byte)targetToEntity.magnitude;
        context.pushDirection = pushDirection;
        context.casterPos = hitEntity.transform.position;

        return context;
    }

    WayPoint ComputePushPoint(Vector3 pushDirection, Entity hitEntity, int pushForce, out int pushDamages)
    {
        WayPoint choosenPoint = null;
        pushDamages = 0;

        Vector3 posWithHeigth = hitEntity.transform.position + Vector3.up * 0.2f;

        bool isDiagonal = pushDirection.x != 0 && pushDirection.z != 0;

        if (pushDirection == Vector3.zero)
        {
            Vector3 casterPosWithHeight = transform.position + Vector3.up * 0.2f;
            Vector3 casterToEntity = posWithHeigth - casterPosWithHeight;

            if (casterToEntity == Vector3.zero)
                return hitEntity.currentPoint;

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
                pushDamages = pushForce;
                Vector3 hitPos = hit.point/*.SnapOnGrid()*/;

                Debug.DrawLine(hitPos, (hitPos - pushDirection * .3f).SnapOnGrid(), Color.blue, 20);

                pushDamages -= Mathf.FloorToInt(hit.distance);
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
                pushDamages = pushForce;

                Debug.DrawLine(posWithHeigth, hit.point, Color.black, 20);

                Vector3 hitPos = hit.point;

                Debug.DrawLine(hitPos, hitPos - pushDirection * .3f, Color.blue, 20);

                pushDamages -= Mathf.FloorToInt(hit.distance);
                choosenPoint = GraphMaker.Instance.serializedPointDict[(hitPos - pushDirection * .3f).SnapOnGrid()];
            }
            else
            {
                choosenPoint = GraphMaker.Instance.serializedPointDict[(posWithHeigth + pushDirection * pushForce).SnapOnGrid()];
            }
        }

        return choosenPoint;
    }

    void PreviewSpellEffect(Spell spell, Entity entity, ref SpellZoneData zoneData)
    {
        foreach (SpellEffect effect in spell.spellData.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    // preview
                    break;
                case SpellEffectType.Recoil:
                    int pushDamages = 0;
                    WayPoint pushPoint = ComputePushPoint(zoneData.hitEntityCTXDict[entity].pushDirection, entity, (int)effect.value, out pushDamages);

                    SpellCastingContext context = new();
                    context = zoneData.hitEntityCTXDict[entity];
                    context.PushDamage = pushDamages;
                    context.PushPoint = pushPoint;

                    zoneData.hitEntityCTXDict[entity] = context;
                    break;
                case SpellEffectType.Shield:
                    //await stats.ApplyShield(effect.value);
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

    public void StopSpellRangePreview(ref List<WayPoint> rangePoints, ref SpellZoneData zoneData)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        rangePoints.Clear();

        StopSpellZonePreview(rangePoints, ref zoneData);
    }

    public void StopSpellZonePreview(List<WayPoint> rangePoints, ref SpellZoneData zoneData, bool showZone = true)
    {
        if (zoneData.zonePoints == null || zoneData.zonePoints.Count == 0) return;

        foreach (WayPoint point in zoneData.zonePoints)
        {
            if (rangePoints.Count != 0 && rangePoints.Contains(point) && showZone)
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
            else
            {
                point.ChangeTileColor(point._normalMaterial);
            }
        }

        zoneData.zonePoints.Clear();
    }



    public async UniTask<bool> TryCastSpell(Spell spell, WayPoint target, List<WayPoint> rangePoints, SpellZoneData zoneData)
    {
        if (zoneData.zonePoints == null || zoneData.zonePoints.Count == 0)
        {
            StopSpellRangePreview(ref rangePoints, ref zoneData);
            return false;
        }

        if(zoneData.hitEntityCTXDict.Keys != null)
            foreach(Entity entity in zoneData.hitEntityCTXDict.Keys)
                await entity.ApplySpell(spell, zoneData.hitEntityCTXDict[entity]);

        spell.StartCooldown();

        StopSpellRangePreview(ref rangePoints, ref zoneData);

        return true;
    }
}

public struct SpellZoneData
{
    public List<WayPoint> zonePoints;
    public Dictionary<Entity, SpellCastingContext> hitEntityCTXDict;
}

/// <summary>
/// utilisé pour appliquer des effets en plus
/// au moment de lancer un sort.
/// </summary>
public struct SpellCastingContext
{
    public byte numberOfHitEnnemies;
    public byte distanceToPlayer;
    public Vector3 pushDirection;
    public Vector3 casterPos;

    public WayPoint PushPoint;
    public int PushDamage;
}
