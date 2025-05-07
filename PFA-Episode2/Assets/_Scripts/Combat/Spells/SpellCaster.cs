using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Drawing;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    public List<WayPoint> RangePoints = new();
    public List<WayPoint> ZonePoints = new();

    public const byte RangeRingThickness = 3;

    public void PreviewSpellRange(SpellData spell, WayPoint center = null, bool showZone = true)
    {
        if (center == null)
        {
            center = entity.currentPoint;
        }

        Dictionary<WayPoint, int> floodDict = Tools.SmallFlood(center,spell.Range);

        List<WayPoint> rangePoints = new();
        rangePoints.AddRange(floodDict.Keys);

        print(rangePoints.Count);

        for (byte i = 0; i < rangePoints.Count; i++)
        {
            WayPoint point = rangePoints[i];
            Vector3 pointPos = new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z);
            Vector3 pointToEntity = transform.position - pointPos;

            if (spell.IsOccludedByWalls && Physics.Raycast(pointPos, pointToEntity, pointToEntity.magnitude, _obstacleMask) || point.State == WaypointState.Obstructed)
                rangePoints.RemoveAt(i);
            else if (showZone)
                point.ChangeTileColor(point._rangeMaterial);
        }
        RangePoints.AddRange(rangePoints);
    }

    public void PreviewSpellZone(SpellData spell, WayPoint targetedPoint, bool showZone = true)
    {
        if (!RangePoints.Contains(targetedPoint)) return;

        Vector3Int targetedPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetedPoint);

        foreach (Vector2Int pos in spell.AreaOfEffect.AffectedTiles)
        {
            Vector3Int posOffset = new Vector3Int(pos.x, 0, pos.y);
            Vector3Int newPos = targetedPointPos + posOffset;

            if (GraphMaker.Instance.serializedPointDict.ContainsKey(newPos))
            {
                WayPoint choosenWaypoint = GraphMaker.Instance.serializedPointDict[newPos];

                if (showZone)
                    choosenWaypoint.ChangeTileColor(choosenWaypoint._zoneMaterial);

                ZonePoints.Add(choosenWaypoint);
            }
        }
    }

    public void StopSpellRangePreview()
    {
        foreach (WayPoint point in RangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        RangePoints.Clear();

        StopSpellZonePreview();
    }

    public void StopSpellZonePreview()
    {
        if (ZonePoints.Count == 0) return;

        foreach (WayPoint point in ZonePoints)
        {
            if (RangePoints.Count != 0 && RangePoints.Contains(point))
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
            else
            {
                point.ChangeTileColor(point._normalMaterial);
            }
        }

        ZonePoints.Clear();
    }

    public async UniTask TryCastSpell(SpellData spell, WayPoint target)
    {
        if (ZonePoints.Count == 0)
        {
            StopSpellRangePreview();
            return;
        }

        SpellCastingContext context = new();

        List<Entity> hitEntities = new();

        foreach (WayPoint point in ZonePoints)
        {
            if(point.State == WaypointState.HasEntity)
            {
                hitEntities.Add(point.Content);
            }
        }

        context.numberOfHitEnnemies = (byte)hitEntities.Count;

        foreach (Entity entity in hitEntities)
        {
            Vector3Int entityTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(entity.currentPoint);
            Vector3Int targetTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(target);
            Vector3Int targetToEntity = entityTilePos - targetTilePos;
            context.distanceToPlayer = (byte)targetToEntity.magnitude;
            context.PushDirection = new Vector3Int((int)Mathf.Sign(targetToEntity.x), targetToEntity.y, (int)Mathf.Sign(targetToEntity.z));

            await entity.ApplySpell(spell, context);
        }
        StopSpellRangePreview();
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
    public Vector3Int PushDirection;
}
