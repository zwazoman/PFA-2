using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    public const byte RangeRingThickness = 3;

    public List<WayPoint> PreviewSpellRange(Spell spell, WayPoint center = null, bool showZone = true, bool ignoreTerrain = false)
    {
        if (center == null)
            center = entity.currentPoint;

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

    public List<WayPoint> PreviewSpellZone(Spell spell, WayPoint targetedPoint, List<WayPoint> rangePoints, bool showZone = true)
    {
        List<WayPoint> zonePoints = new();

        if (!rangePoints.Contains(targetedPoint)) return zonePoints;

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
            }
        }

        return zonePoints;
    }

    public void StopSpellRangePreview(ref List<WayPoint> rangePoints)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        rangePoints.Clear();
    }

    public void StopSpellRangePreview(ref List<WayPoint> rangePoints, ref List<WayPoint> zonePoints)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        rangePoints.Clear();

        StopSpellZonePreview(rangePoints, ref zonePoints);
    }

    public void StopSpellZonePreview(List<WayPoint> rangePoints, ref List<WayPoint> zonePoints, bool showZone = true)
    {
        if (zonePoints == null || zonePoints.Count == 0) return;

        foreach (WayPoint point in zonePoints)
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

        zonePoints.Clear();
    }

    public async UniTask<bool> TryCastSpell(Spell spell, WayPoint target, List<WayPoint> rangePoints, List<WayPoint> zonePoints)
    {
        if (zonePoints.Count == 0)
        {
            StopSpellRangePreview(ref rangePoints, ref zonePoints);
            return false;
        }

        print("cast Spell");

        SpellCastingContext context = new();

        List<Entity> hitEntities = new();

        print(zonePoints.Count);

        foreach (WayPoint point in zonePoints)
        {
            if (point.State == WaypointState.HasEntity)
            {
                hitEntities.Add(point.Content);
                print(point.Content.gameObject.name);
            }
        }

        context.numberOfHitEnnemies = (byte)hitEntities.Count;

        foreach (Entity entity in hitEntities)
        {
            Vector3Int entityTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(entity.currentPoint);
            Vector3Int targetTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(target);
            Vector3Int targetToEntity = entityTilePos - targetTilePos;

            int xPushDirection = targetToEntity.x != 0 ? (int)Mathf.Sign(targetToEntity.x) : 0;
            int zPushDirection = targetToEntity.z != 0 ? (int)Mathf.Sign(targetToEntity.z) : 0;

            context.distanceToPlayer = (byte)targetToEntity.magnitude;
            context.PushDirection = new Vector3(xPushDirection, 0, zPushDirection);

            await entity.ApplySpell(spell, context);
        }

        spell.StartCooldown();

        StopSpellRangePreview(ref rangePoints, ref zonePoints);

        return true;
    }
}

/// <summary>
/// utilisé pour appliquer des effets en plus
/// au moment de lancer un sort.
/// </summary>
public struct SpellCastingContext
{
    public byte numberOfHitEnnemies;
    public byte distanceToPlayer;
    public Vector3 PushDirection;
    public Vector3 casterPos;
}
