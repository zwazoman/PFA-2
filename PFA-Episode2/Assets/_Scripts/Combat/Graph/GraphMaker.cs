using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEditor;
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

    public SerializedDictionary<Vector3Int, WayPoint> serializedPointDict = new();

    [Header("Visuals")]
    [SerializeField] GameObject _tileVisualsPrefab;

    private void Awake()
    {
        instance = this;
    }

#if UNITY_EDITOR
 /*   void GenerateGraph()
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
                serializedPointDict.Add(spawnPos, point);

                Vector3Int down = new Vector3Int(spawnPos.x, 0, spawnPos.z - 1);
                Vector3Int left = new Vector3Int(spawnPos.x - 1, 0, spawnPos.z);

                if (serializedPointDict.ContainsKey(down))
                {
                    point.Neighbours.Add(serializedPointDict[down]);
                    serializedPointDict[down].Neighbours.Add(point);
                }
                if (serializedPointDict.ContainsKey(left))
                {
                    point.Neighbours.Add(serializedPointDict[left]);
                    serializedPointDict[left].Neighbours.Add(point);
                }


            }
        }
    }*/

    public void ComposeGraph()
    {
        ResetGraph();

        foreach(WayPoint point in _allWaypoints)
        {
            
            //add points to dictionary
            Vector3Int pointPos = point.transform.position.SnapOnGrid();
            point.graphPos = pointPos;
            serializedPointDict.Add(pointPos, point);

            //set neighbours
            foreach(Vector3 flatDirection in Tools.AllFlatDirections)
            {
                RaycastHit hit;
                if (Physics.Raycast(point.transform.position /*+ Vector3.up * 0.5f*/, flatDirection, out hit, 1, LayerMask.GetMask("Waypoint")))
                    if (hit.collider.TryGetComponent(out WayPoint wayPoint) && !point.Neighbours.Contains(wayPoint))
                        point.Neighbours.Add(wayPoint);
            }
            
            //Debug.Log(point==null);
        }
    }

    public void SetUpVisuals()
    {
        foreach (WayPoint point in _allWaypoints)
        {
            //setup visuals
            if (point.State != WaypointState.Obstructed ||true)
            {
                for (int i = 0; i < point.gameObject.transform.childCount; i++)
                {
                    DestroyImmediate(point.transform.GetChild(0).gameObject);
                    //Debug.Log(i);
                }

                Vector3 pose = point.transform.position + Vector3.up * .6f;
                Vector3 rayOrigin = point.transform.position + Vector3.up * 10;
                if (Physics.SphereCast(rayOrigin, .4f, Vector3.down,out RaycastHit hit,20,~LayerMask.GetMask("Wall")))
                {
                    Vector3 toPoint = hit.point - rayOrigin;
                    pose = rayOrigin + Vector3.Project(toPoint, Vector3.down) + Vector3.up*.05f;
                }

                Transform t = ((GameObject)PrefabUtility.InstantiatePrefab(_tileVisualsPrefab, point.transform)).transform;
                t.transform.position = pose;
                t.localScale = Vector3.zero;
                t.rotation = Quaternion.Euler(-90, 0, 0);
                t.gameObject.SetActive(false);
                
                point._previewVisuals = t.gameObject.GetComponent<MeshRenderer>();
                EditorUtility.SetDirty(point);
            }

        }
    }

    public void ResetGraph()
    {
        serializedPointDict.Clear();

        foreach (WayPoint point in _allWaypoints)
        {
            point.Neighbours.Clear();
            point.graphPos = Vector3Int.zero;
        }
    }
}


[CustomEditor(typeof(GraphMaker))]
public class GraphMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GraphMaker graphMaker = (GraphMaker)target;

        GUILayout.Space(20);

        if(GUILayout.Button("G�n�rer le Graph"))
            graphMaker.ComposeGraph();
        

        GUILayout.Space(20);

        if(GUILayout.Button("Reset le Graph"))
            graphMaker.ResetGraph();
        
        GUILayout.Space(20);

        if(GUILayout.Button("generer les visuels de preview"))
            graphMaker.SetUpVisuals();

    }
#endif

}
