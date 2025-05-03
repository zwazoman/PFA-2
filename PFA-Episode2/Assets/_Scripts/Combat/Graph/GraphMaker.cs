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

    [SerializeField] bool _generatesGraph = true;

    [Header("Graph Builder")]

    [SerializeField] public Vector3Int StartPos; // get set
    [SerializeField] public Vector3Int EndPos; // get set

    [SerializeField] GameObject _waypointPrefab;

    [Header("Graph Composer")]

    [SerializeField] WayPoint _bottomLeftPoint;

    [SerializeField] List<WayPoint> _allWaypoints;

    public Dictionary<Vector3Int, WayPoint> PointDict = new Dictionary<Vector3Int, WayPoint>();


    private void Awake()
    {
        instance = this;

        if (_generatesGraph)
            GenerateGraph();
        else
            ComposeGraph();


    }

    void GenerateGraph()
    {
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

    void ComposeGraph()
    {
        Vector3 posOffset = _bottomLeftPoint.transform.position;

        foreach(WayPoint point in _allWaypoints)
        {
            Vector3Int pointPos = (point.transform.position - posOffset).SnapOnGrid();
            PointDict.Add(pointPos, point);
        }
    }
    
}
