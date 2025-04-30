using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class Tools
{
    public static Dictionary<WayPoint, int> FloodDict = new();

    public static Vector3[] AllFlatDirections = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };

    public static Vector3Int SnapOnGrid(this Vector3 initialPos)
    {
        return new Vector3Int(Mathf.RoundToInt(initialPos.x), 0, Mathf.RoundToInt(initialPos.z));
    }

    public static T PickRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
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

    public static WayPoint FindClosestFloodPoint(this List<WayPoint> wayPoints, Dictionary<WayPoint, int> floodDict = null)
    {
        WayPoint closest = null;
        int closestDistance = int.MaxValue;

        if (floodDict == null)
            floodDict = FloodDict;

        foreach (WayPoint point in wayPoints)
        {
            if (!floodDict.ContainsKey(point))
            {
                Debug.Log("point not in flood");
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
        foreach (var pair in dict)
        {
            if (pair.Value.Equals(value))
            {
                return pair.Key; // Retourne la clé
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
    public static Dictionary<WayPoint, int> Flood(WayPoint startNode) //On part d'un node de départ avec une range donné pour regardé les voisins
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

            foreach (var neighbor in node.Neighbours)
                if (neighbor is WayPoint point && !visited.Contains(point) && point.State != WaypointState.Obstructed)
                {
                    queue.Enqueue((point, distance + 1));
                    visited.Add(point);
                }
        }
        FloodDict = PointDistanceDict;
        return PointDistanceDict;
    }

    public static Dictionary<WayPoint, int> SmallFlood(WayPoint startPoint, int range)
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
                    if (neighbor is WayPoint point && !visited.Contains(point) && point.State != WaypointState.Obstructed)
                    {
                        queue.Enqueue((point, distance + 1));
                        visited.Add(point);
                    }
        }
        FloodDict = PointDistanceDict;
        return PointDistanceDict;
    }
    public static void ClearFlood()
    {
        FloodDict.Clear();
    }

    public static List<WayPoint> GetWaypointsInRange(int range)
    {
        List<WayPoint> wayPoints = new();

        foreach (WayPoint point in FloodDict.Keys)
        {
            if (FloodDict[point] <= range)
                wayPoints.Add(point);
        }

        return wayPoints;
    }

    public static bool CheckMouseRay(out WayPoint point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit, Mathf.Infinity/*, LayerMask.GetMask("Waypoint")*/);

        point = null;

        if (hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint wayPoint))
        {
            point = wayPoint;
            return true;
        }
        return false;
    }

}
