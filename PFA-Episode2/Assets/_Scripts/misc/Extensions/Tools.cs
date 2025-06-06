using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public static class Tools
{
    public static Dictionary<WayPoint, int> FloodDict = new();

    public static Vector3[] AllFlatDirections = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };

    public static Vector3 GetPositionAboveFinger()
    {
        return Input.mousePosition + Vector3.up * 10*1080/Screen.height;
    }
    public static Vector3Int SnapOnGrid(this Vector3 initialPos)
    {
        return new Vector3Int(Mathf.RoundToInt(initialPos.x), 0, Mathf.RoundToInt(initialPos.z));
    }

    public static bool CheckWallsBetween(WayPoint a, WayPoint b, float heightOffset = 0.7f)
    {
        Vector3 aPos = a.transform.position + Vector3.up * heightOffset;
        Vector3 bPos = b.transform.position + Vector3.up * heightOffset;
        Vector3 offset = bPos - aPos;

        return Physics.Raycast(aPos, offset, offset.magnitude, LayerMask.GetMask("Wall"));
    }

    [MenuItem("Data/testAverageFunc")]
    public static void TestAverage()
    {
        float? av = 10;
        int? c = 1;
        Debug.Log(av);
        Debug.Log(c);
        AccumulateAverage(ref av, ref c, 12);
        Debug.Log(av);
        Debug.Log(c);
    }

    /// <summary>
    /// prend une moyenne et la met à jour en ajoutant un nouvel élément dedans.
    /// </summary>
    /// <param name="AverageOfValues"></param>
    /// <param name="ValueCount"></param>
    /// <param name="newValueToAccumulate"></param>
    public static void AccumulateAverage(ref float AverageOfValues, ref int ValueCount, float newValueToAccumulate)
    {
        float currentSum = AverageOfValues * ValueCount;
        float newSum = currentSum + newValueToAccumulate;
        
        ValueCount++;
        float newAverage = newSum / ValueCount;
        AverageOfValues = newAverage;
    }

    public static void AccumulateAverage(ref float? AverageOfValues, ref int? ValueCount, float newValueToAccumulate)
    {
        if (AverageOfValues != null && ValueCount != null)
        {
            float currentSum = AverageOfValues.Value * ValueCount.Value;
            float newSum = currentSum + newValueToAccumulate;

            ValueCount++;
            float newAverage = newSum / ValueCount.Value;
            AverageOfValues = newAverage;
        }
        else throw new ArgumentNullException();

    }
    public static string FormatPlaytestValueNameString(string ValueName)
    {
        return ValueName + "_" + Application.version.ToString();
    }

    public static T PickRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static Vector3 FindClosest(this List<Vector3> points, Vector3 origin)
    {
        if (points.Count == 0)
        {
            Debug.LogError("List Is Empty");
            return Vector3.zero;
        }
        if (points.Count == 1)
        {
            return points[0];
        }

        Vector3 closest = points[0];

        foreach (Vector3 point in points)
        {
            Vector3 pointOffset = point - origin;
            Vector3 closestOffset = closest - origin;

            if (pointOffset.sqrMagnitude < closestOffset.sqrMagnitude)
                closest = point;
        }

        return closest;
    }

    public static Transform FindClosest(this List<Transform> transforms, Vector3 origin)
    {
        if (transforms.Count == 0)
        {
            Debug.LogError("List Is Empty");
            return null;
        }
        if (transforms.Count == 1)
        {
            return transforms[0];
        }

        Transform closest = transforms[0];

        foreach (Transform t in transforms)
        {
            Vector3 pointOffset = t.position - origin;
            Vector3 closestOffset = closest.position - origin;

            if (pointOffset.sqrMagnitude < closestOffset.sqrMagnitude)
                closest = t;
        }

        return closest;

    }

    public static T FindClosest<T>(this List<T> elements, Vector3 origin) where T : UnityEngine.Component
    {
        if (elements.Count == 0)
        {
            Debug.LogError("List Is Empty");
            return null;
        }
        if (elements.Count == 1)
        {
            return elements[0];
        }

        T closest = elements[0];

        float closestDistance = (closest.transform.position - origin).sqrMagnitude;

        foreach (T element in elements)
        {

            if ((element.transform.position - origin).sqrMagnitude < closestDistance)
            {
                closest = element;
                closestDistance = (closest.transform.position - origin).sqrMagnitude;
            }
        }

        return closest;
    }

    public static bool FindClosestFloodPoint(this List<WayPoint> wayPoints, out WayPoint closest, Dictionary<WayPoint, int> floodDict = null)
    {
        closest = null;
        int closestDistance = int.MaxValue;

        if (floodDict == null)
            floodDict = FloodDict;

        foreach (WayPoint point in wayPoints)
        {
            if (!floodDict.ContainsKey(point))
                continue;

            int pointDistance = floodDict[point];
            if (pointDistance < closestDistance)
            {
                closest = point;
                closestDistance = pointDistance;
            }
        }

        if(closest != null)
            return true;
        return false;
    }

    public static WayPoint FindClosestFloodPoint(this Dictionary<WayPoint, List<WayPoint>>.KeyCollection wayPoints, Dictionary<WayPoint, int> floodDict = null)
    {
        WayPoint closest = null;
        int closestDistance = int.MaxValue;

        if (floodDict == null)
            floodDict = FloodDict;

        foreach (WayPoint point in wayPoints)
        {
            if (!floodDict.ContainsKey(point))
            {
                continue;
            }

            int pointDistance = floodDict[point];
            if (pointDistance < closestDistance)
            {
                closest = point;
                closestDistance = pointDistance;
            }
        }
        return closest;
    }

    public static T1 GetKeyFromValue<T1, T2>(this Dictionary<T1, T2> dict, T2 value)
    {
        foreach (KeyValuePair<T1,T2> pair in dict)
        {
            if (pair.Value.Equals(value))
            {
                return pair.Key; // Retourne la cl�
            }
        }
        return default;
    }

    public static Stack<WayPoint> FindBestPath(WayPoint startPoint, WayPoint endPoint)
    {
        List<WayPoint> openWayPoints = new List<WayPoint>();
        List<WayPoint> closedWayPoints = new List<WayPoint>();

        Stack<WayPoint> shorterPath = new Stack<WayPoint>();

        startPoint.TravelThrough(ref openWayPoints, ref closedWayPoints, ref shorterPath, endPoint, startPoint);

        foreach (WayPoint point in openWayPoints) point.ResetState();
        foreach (WayPoint point in closedWayPoints) point.ResetState();
        return shorterPath;
    }

    /// <summary>
    /// Retourne les tuiles accessibles dans un certain rayon en utilisant une recherche en largeur (BFS).
    /// </summary>
    public static Dictionary<WayPoint, int> Flood(WayPoint startNode) //On part d'un node de d�part avec une range donn� pour regard� les voisins
    {
        FloodDict.Clear();
        Dictionary<WayPoint, int> PointDistanceDict = new();
        Queue<(WayPoint, int)> queue = new();
        HashSet<WayPoint> visited = new() { startNode };

        queue.Enqueue((startNode, 0));

        while (queue.Count > 0)
        {
            var (node, distance) = queue.Dequeue();
            PointDistanceDict.Add(node, distance);

            foreach (WayPoint neighbor in node.Neighbours)
                if (!visited.Contains(neighbor) && neighbor.State != WaypointState.Obstructed)
                {
                    queue.Enqueue((neighbor, distance + 1));
                    visited.Add(neighbor);
                }
        }
        FloodDict = PointDistanceDict;
        return PointDistanceDict;
    }

    public static Dictionary<WayPoint, int> SmallFlood(WayPoint startPoint, int range, bool includesEntities = false, bool includesObstructed = false)
    {
        Dictionary<WayPoint, int> PointDistanceDict = new();
        Queue<(WayPoint, int)> queue = new();
        HashSet<WayPoint> visited = new() { startPoint };

        queue.Enqueue((startPoint, 0));

        while (queue.Count > 0)
        {
            var (node, distance) = queue.Dequeue();
            PointDistanceDict.Add(node, distance);

            if (distance < range)
                foreach (var neighbor in node.Neighbours)
                    if (!visited.Contains(neighbor) && !(includesEntities && neighbor.State == WaypointState.HasEntity) && !(includesObstructed && neighbor.State == WaypointState.Obstructed))
                    {
                        queue.Enqueue((neighbor, distance + 1));
                        visited.Add(neighbor);
                    }
        }
        return PointDistanceDict;
    }
    public static void ClearFlood()
    {
        FloodDict.Clear();
    }

    public static bool CheckMouseRay(out WayPoint point, bool blockedByUi = false,bool fingerOffset = false)
    {
        point = null;

        if (blockedByUi && EventSystem.current.IsPointerOverGameObject(0))
        {
            return false;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(fingerOffset? GetPositionAboveFinger() : Input.mousePosition);

        Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Waypoint"));

        if (hit.collider != null && hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint wayPoint))
        {
            point = wayPoint;
            return true;
        }
        return false;
    }

    //images

    public static void Hide(this CanvasGroup group)
    {
        group.DOComplete();
        group.DOFade(0, 1);
    }

    public static void Show(this CanvasGroup group)
    {
        group.DOComplete();
        group.DOFade(1, 1);
    }

}
