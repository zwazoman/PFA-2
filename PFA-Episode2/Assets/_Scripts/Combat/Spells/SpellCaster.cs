using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] Entity _entity;

    [SerializeField] LayerMask _obstacleMask;

    WayPoint _entityWaypoint;

    List<WayPoint> _rangePoints = new();
    List<WayPoint> _zonePoints = new();

    private void Awake()
    {
        _entityWaypoint = _entity.CurrentPoint;
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

            _zonePoints.Add(choosenWaypoint);
        }

    }

    public void StopSpellRangePreview()
    {
        //repasser les points dans la couleur de base
        _rangePoints.Clear();
    }

    public void StopSpellZonePreview()
    {
        //repasser les points dans la couleur de base sauf ceux qui sont des rangePoints

        _rangePoints.Clear();
    }

    public async UniTask TryCastSpell(SpellData spell, WayPoint target)
    {
        //check si on peut cast le spell, sinon : stop les previews

        foreach(WayPoint point in _zonePoints)
        {
            //Appliquer le sort sur les cases
        }
    }
}
