using System.Collections.Generic;
using UnityEngine;

public class GraphMaker : MonoBehaviour
{
    //singleton
    private static GraphMaker instance;

    public static GraphMaker Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Graph Maker");
                instance = go.AddComponent<GraphMaker>();
            }
            return instance;
        }
    }

    [HideInInspector] public List<WayPoint> SelectedPoints = new List<WayPoint>();
    [HideInInspector] public List<WayPoint> TargetPoints = new List<WayPoint>();


    [SerializeField] public Vector3Int StartPos; // get set
    [SerializeField] public Vector3Int EndPos; // get set

    [SerializeField] GameObject _waypointPrefab;

    public Dictionary<Vector3Int, WayPoint> PointDict = new Dictionary<Vector3Int, WayPoint>();

    public Entity Test;


    private void Awake()
    {
        instance = this;

        int xPos = StartPos.x;
        int zPos = StartPos.z;

        for (int i = 0; i < EndPos.z * 2; i++)
        {
            for (int j = 0; j < EndPos.x * 2; j++)
            {
                Vector3Int spawnPos = new Vector3Int(xPos + j, 0, zPos + i);
                WayPoint point = Instantiate(_waypointPrefab, (Vector3)spawnPos, Quaternion.identity).GetComponent<WayPoint>();
                point.transform.parent = this.transform;
                point.gameObject.layer = 3;
                PointDict.Add(spawnPos, point);

                Vector3Int down = new Vector3Int(spawnPos.x, 0, spawnPos.z - 1);
                Vector3Int left = new Vector3Int(spawnPos.x - 1, 0, spawnPos.z);

                if (PointDict.ContainsKey(down))
                {
                    point.Neighbours.Add(PointDict[down]);
                    PointDict[down].Neighbours.Add(point);
                }
                if (PointDict.ContainsKey(left))
                {
                    point.Neighbours.Add(PointDict[left]);
                    PointDict[left].Neighbours.Add(point);
                }
            }
        }
    }
}
