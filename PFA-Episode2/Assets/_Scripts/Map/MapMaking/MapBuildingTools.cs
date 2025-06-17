using System.Collections.Generic;
using UnityEngine;
using static NodeTypes;

[RequireComponent(typeof(MapMaker2))]
public class MapBuildingTools : MonoBehaviour
{
    [SerializeField][Tooltip("Mesh qui sera dupliqué pour faire les chemins entre les nodes")] private List<GameObject> _listRoadPath;
    public List<GameObject> TrueListPath;
    [SerializeField][Tooltip("GameObject parent des chemins, si null alors c'est le porteur du script le parent")] private GameObject _parent;
    public bool FirstTimeDraw = true;
    public List<GameObject> _savePath = new();
    private List<Vector3> _pathPos = new();

    #region Singleton
    public static MapBuildingTools Instance;
    private void Awake()
    {
        Instance = this;
        if (_parent == null)
        {
            _parent = gameObject;
        }
        for (int i = 0; i <= 50; i++)
        {
            int index = Random.Range(0, _listRoadPath.Count);
            GameObject Path = Instantiate(_listRoadPath[index], _parent.transform);
            Path.SetActive(false);
            TrueListPath.Add(Path);
        }
    }
    #endregion

    /// <summary>
    /// Fonction qui trace un trait entre le point A et B
    /// </summary>
    /// <param name="PointA">Point A (_fatherOfNode)</param>
    /// <param name="PointB">Point B (CurrentNode)</param>
    /// <param name="Drawing">Booléen qui permet de dessiner ou non</param>
    public void TraceTonTrait(Node PointA, Node PointB)
    {
        if (FirstTimeDraw)
        {
            // Sprite entre father et current
            GameObject CurrentPath = TrueListPath[0];
            TrueListPath.RemoveAt(0);
            CurrentPath.SetActive(true);

            Vector3 trailPos = (PointA.transform.localPosition + PointB.transform.localPosition) / 2f; //au milleu des 2
            CurrentPath.transform.localPosition = trailPos;

            // Rotation du sprite
            //Vector3 dir = PointB.transform.localPosition - PointA.transform.localPosition;
            //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (_pathPos.Contains(CurrentPath.transform.localPosition)) { Destroy(CurrentPath); return; }
            else { _pathPos.Add(CurrentPath.transform.localPosition); }

            if (PointA.transform.localPosition.y > PointB.transform.localPosition.y)
            {
                CurrentPath.transform.localRotation = Quaternion.Euler(25, 90, -90);
                //CurrentPath.transform.localPosition = new Vector3(CurrentPath.transform.localPosition.x, CurrentPath.transform.localPosition.y - 50, CurrentPath.transform.localPosition.z);
            }

            else if (PointA.transform.localPosition.y < PointB.transform.localPosition.y)
            {
                CurrentPath.transform.localRotation = Quaternion.Euler(-25, 90, -90);
                //CurrentPath.transform.localPosition = new Vector3(CurrentPath.transform.localPosition.x, CurrentPath.transform.localPosition.y + 50, CurrentPath.transform.localPosition.z);
            }

            else { CurrentPath.transform.localRotation = Quaternion.Euler(0, 90, -90); }

            PointA.PathBetweenNode.Add(CurrentPath);
            _savePath.Add(CurrentPath);
        }
        else { FirstTimeDraw = true; }
    }

    public bool Intersection(int mapRangeCurrentIndex, int probaIntersection)
    {
        int max = 3;
        if (PlayerPrefs.GetInt("FirstLaunch") == 1) { max = 4; } 
        if (mapRangeCurrentIndex > max)
        {
            int result = Random.Range(1, 11);
            if (result <= probaIntersection) //Intersection si true
            {
                return true;
            }
            return false;
        }
        else { return false; }

    }

    public void AttributeEvent(int _mapRange)
    {
        if (MapMaker2.Instance.CurrentNode.Position + 1 == _mapRange) { MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Cuisine; }
        if (MapMaker2.Instance.CurrentNode.Position == _mapRange) { MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Boss; }
        if (PlayerPrefs.GetInt("FirstLaunch") == 1) 
        {
            switch (MapMaker2.Instance.CurrentNode.Position)
            {
                case 1:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Tuto;
                    break;
                case 2:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Ingredient;
                    break;
                case 3:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Cuisine;
                    break;
                case 4:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Combat;
                    break;
            }
        }
        else
        {
            switch (MapMaker2.Instance.CurrentNode.Position)
            {
                case 1:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Ingredient;
                    break;
                case 2:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Cuisine;
                    break;
                case 3:
                    MapMaker2.Instance.CurrentNode.EventName = NodesEventTypes.Combat;
                    break;
            }
        }

    }

    public Vector3Int GetKeyFromNode(Node node)
    {
        foreach (var kvp in MapMaker2.Instance.DicoNode)
        {
            if (kvp.Value == node)
                return kvp.Key;
        }
        return Vector3Int.zero; // par defaut si pas trouve
    }

    public Node ReturnNodeFromNodePosition(int nodePosition, int distanceVerif)
    {
        foreach(Node node in MapMaker2.Instance.DicoNode.Values)
        {
            if (node.Position == nodePosition + distanceVerif)
            {
                return node;
            }
        }
        return null;
    }

    public List<Node> ReturnListOfNodeFromNodePosition(int nodePosition)
    {
        List<Node> nodeList = new();
        foreach (Node node in MapMaker2.Instance.DicoNode.Values)
        {
            if (node.Position == nodePosition)
            {
                nodeList.Add(node);
            }
        }
        return nodeList;
    }
}
