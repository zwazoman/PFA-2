using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static Vector3[] AllFlatDirections = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right};

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

    public static T FindClosest<T>(this List<T> elements, Vector3 origin) where T : Component
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

    public static T1 GetKeyFromValue<T1,T2>(this Dictionary<T1,T2> dict, T2 value)
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

    //public static WayPoint FindClosest(this List<WayPoint> points, WayPoint origin)
    //{
    //    if (points.Count == 0)
    //        Debug.LogError("List Is Empty");

    //    Dictionary<WayPoint,int> waypointDistance = new Dictionary<WayPoint,int>();

    //    foreach (WayPoint point in points)
    //    {
    //        Stack<WayPoint> path = FindBestPath(origin, point);
    //        waypointDistance.Add(point, path.Count);
    //    }

    //    WayPoint closest = waypointDistance.

    //    foreach(WayPoint point in waypointDistance.Keys)
    //    {
    //        if()
    //    }

        
       
    //}

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

}
