using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> _zoneList;
    public List<Vector3> PositionForSpawn;
    public void Start()
    {
        foreach (BoxCollider _zone in _zoneList)
        {
            Bounds Bounds = _zone.bounds;
            float PosX = Random.Range(Bounds.min.x, Bounds.max.x);
            float PosY = Random.Range(Bounds.min.y, Bounds.max.y);
            PositionForSpawn.Add(new Vector3(PosX, PosY));
        }
    }
}
