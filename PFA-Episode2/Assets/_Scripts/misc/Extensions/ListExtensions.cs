using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T PickRandom<T>(this List<T> list)
    {
        return list[ Random.Range(0, list.Count)];
    }
}
