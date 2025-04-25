using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity entity;

    [SerializeField] LayerMask _obstacleMask;

    WayPoint _entityWaypoint;

    List<WayPoint> _rangePoints = new();
    List<WayPoint> _zonePoints = new();

    private async void Start()
    {
        await UniTask.Yield();

        _entityWaypoint = entity.CurrentPoint;
    }

    public void PreviewSpellRange(SpellData spell,WayPoint center = null)
    {
        if (center == null) center = _entityWaypoint;

        Dictionary<WayPoint,int> reachablePoints = Tools.GetReachablePoints(center, spell.Range);

        foreach (WayPoint point in reachablePoints.Keys)
        {
            Vector3 pointPos = new Vector3(point.transform.position.x, transform.position.y, point.transform.position.z);
            Vector3 pointToEntity = transform.position - pointPos;

            if((spell.Range > 3 && reachablePoints[point] < spell.Range - 3) || Physics.Raycast(pointPos, pointToEntity, pointToEntity.magnitude, _obstacleMask))
            {
                reachablePoints.Remove(point);
            }
            else
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
        }
        _rangePoints.AddRange(reachablePoints.Keys);
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

            point.TryApplySpell(spell);
        }

        StopSpellRangePreview();
    }
}
