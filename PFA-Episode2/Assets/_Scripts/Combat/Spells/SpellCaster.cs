using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    List<WayPoint> _rangePoints = new();
    List<WayPoint> _zonePoints = new();

    public void PreviewSpellRange(SpellData spell,WayPoint center = null)
    {
        if (center == null)
        {
            center = entity.CurrentPoint;
        }

        List<WayPoint> rangePoints = Tools.GetWaypointsInRange(spell.Range);

        foreach (WayPoint point in rangePoints)
        {
            Vector3 pointPos = new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z);
            Vector3 pointToEntity = transform.position - pointPos;

            if((spell.Range > 3 && Tools.FloodDict[point] < spell.Range - 3) || Physics.Raycast(pointPos, pointToEntity, pointToEntity.magnitude, _obstacleMask))
            {
                rangePoints.Remove(point);
            }
            else
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
        }
        _rangePoints.AddRange(rangePoints);
    }

    public void PreviewSpellZone(SpellData spell, WayPoint targetedPoint)
    {
        if (!_rangePoints.Contains(targetedPoint)) return;

        Vector3Int targetedPointPos = GraphMaker.Instance.PointDict.GetKeyFromValue(targetedPoint);

        foreach (Vector2Int pos in spell.AreaOfEffect.AffectedTiles)
        {
            Vector3Int newPos = new Vector3Int(pos.x,0,pos.y);

            WayPoint choosenWaypoint = GraphMaker.Instance.PointDict[targetedPointPos + newPos];

            choosenWaypoint.ChangeTileColor(choosenWaypoint._zoneMaterial);

            _zonePoints.Add(choosenWaypoint);
        }

    }

    public void StopSpellRangePreview()
    {
        foreach(WayPoint point in _rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        _rangePoints.Clear();

        StopSpellZonePreview();
    }

    public void StopSpellZonePreview()
    {
        if(_zonePoints.Count == 0) return;

        foreach(WayPoint point in _zonePoints)
        {
            if (_rangePoints.Contains(point))
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
            else
            {
                point.ChangeTileColor(point._normalMaterial);
            }
        }

        _zonePoints.Clear();
    }

    public async UniTask TryCastSpell(SpellData spell, WayPoint target)
    {
        if (_zonePoints.Count == 0)
        {
            StopSpellRangePreview();
            return;
        }

        foreach(WayPoint point in _zonePoints)
        {
            //await visual

            await point.TryApplySpell(spell);
        }

        StopSpellRangePreview();
    }
}
