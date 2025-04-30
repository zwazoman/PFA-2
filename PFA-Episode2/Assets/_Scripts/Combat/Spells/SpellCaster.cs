using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    public List<WayPoint> RangePoints = new();
    public List<WayPoint> ZonePoints = new();

    public void PreviewSpellRange(SpellData spell, WayPoint center = null, bool showZone = true, int spellRange = 0)
    {
        Dictionary<WayPoint, int> floodDict = new();

        if (center == null)
        {
            center = entity.CurrentPoint;
            floodDict = Tools.FloodDict;
        }
        else
        {
            floodDict = Tools.SmallFlood(center, spellRange);
        }

        List<WayPoint> rangePoints = Tools.GetWaypointsInRange(spell.Range, floodDict);

        foreach (WayPoint point in rangePoints)
        {
            Vector3 pointPos = new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z);
            Vector3 pointToEntity = transform.position - pointPos;

            if ((spell.Range > 3 && floodDict[point] < spell.Range - 3) || Physics.Raycast(pointPos, pointToEntity, pointToEntity.magnitude, _obstacleMask))
                rangePoints.Remove(point);
            else if (showZone)
                point.ChangeTileColor(point._rangeMaterial);
        }
        RangePoints.AddRange(rangePoints);
    }

    public void PreviewSpellZone(SpellData spell, WayPoint targetedPoint, bool showZone = true)
    {
        if (!RangePoints.Contains(targetedPoint)) return;

        Vector3Int targetedPointPos = GraphMaker.Instance.PointDict.GetKeyFromValue(targetedPoint);

        foreach (Vector2Int pos in spell.AreaOfEffect.AffectedTiles)
        {
            Vector3Int posOffset = new Vector3Int(pos.x, 0, pos.y);
            Vector3Int newPos = targetedPointPos + posOffset;

            if (GraphMaker.Instance.PointDict.ContainsKey(newPos))
            {
                WayPoint choosenWaypoint = GraphMaker.Instance.PointDict[newPos];

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

        foreach (WayPoint point in ZonePoints)
        {
            //await visual

            await point.TryApplySpell(spell);
        }

        StopSpellRangePreview();
    }
}
