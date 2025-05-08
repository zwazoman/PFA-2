using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Drawing;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    public const byte RangeRingThickness = 3;

    public List<WayPoint> PreviewSpellRange(SpellData spell, WayPoint center = null, bool showZone = true, bool ignoreTerrain = false)
    {
        if (center == null)
            center = entity.currentPoint;

        Dictionary<WayPoint, int> floodDict = Tools.SmallFlood(center, spell.Range);

        List<WayPoint> rangePoints = new();

        int removedCpt = 0;

        foreach (WayPoint point in floodDict.Keys)
        {
            Vector3 pointWallCheckPos = point.transform.position + Vector3.up * 0.7f;
            Vector3 wallCheckPos = center.transform.position + Vector3.up * 0.7f;
            Vector3 pointToEntity = pointWallCheckPos - wallCheckPos;

            Debug.DrawLine(pointWallCheckPos, wallCheckPos, UnityEngine.Color.red, 10);

            if (!ignoreTerrain && (spell.IsOccludedByWalls && Tools.CheckWallsBetween(center, point) || point.State == WaypointState.Obstructed))
            {
                removedCpt++;
                continue;
            }
            else if (showZone)
                point.ChangeTileColor(point._rangeMaterial);

            rangePoints.Add(point);
        }

        return rangePoints;
    }

    public List<WayPoint> PreviewSpellZone(SpellData spell, WayPoint targetedPoint, List<WayPoint> rangePoints, bool showZone = true)
    {
        if (!rangePoints.Contains(targetedPoint)) return null;

        List<WayPoint> zonePoints = new();

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

    public void StopSpellRangePreview(ref List<WayPoint> rangePoints,ref List<WayPoint> zonePoints)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        rangePoints.Clear();

        StopSpellZonePreview(rangePoints,ref zonePoints);
    }

    public void StopSpellZonePreview(List<WayPoint> rangePoints,ref List<WayPoint> zonePoints)
    {
        if (zonePoints.Count == 0) return;

        foreach (WayPoint point in zonePoints)
        {
            if (rangePoints.Count != 0 && rangePoints.Contains(point))
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

    public async UniTask TryCastSpell(SpellData spell, WayPoint target,List<WayPoint> rangePoints, List<WayPoint> zonePoints)
    {
        if (zonePoints.Count == 0)
        {
            StopSpellRangePreview(ref rangePoints,ref zonePoints);
            return;
        }

        SpellCastingContext context = new();

        List<Entity> hitEntities = new();

        foreach (WayPoint point in zonePoints)
        {
            if (point.State == WaypointState.HasEntity)
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
        StopSpellRangePreview(ref rangePoints,ref zonePoints);
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
