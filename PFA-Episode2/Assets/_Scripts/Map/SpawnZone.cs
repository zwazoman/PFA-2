using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> _zoneList;
    public List<Vector3> PositionForSpawn { get; private set; } = new();

    [SerializeField] private int _pointsPerZone = 1; // nombre de points à générer par zone

    private void OnEnable()
    {
        PositionForSpawn.Clear();
        foreach (BoxCollider zone in _zoneList)
        {
            Bounds bounds = zone.bounds;
            Vector3 size = bounds.size;
            float marginRatio = 0.1f;

            float x = Random.Range(bounds.min.x + size.x * marginRatio, bounds.max.x - size.x * marginRatio);
            float y = Random.Range(bounds.min.y + size.y * marginRatio, bounds.max.y - size.y * marginRatio);
            float z = Random.Range(bounds.min.z + size.z * marginRatio, bounds.max.z - size.z * marginRatio);

            PositionForSpawn.Add(new Vector3(x, y, z));

        }
    }
}
