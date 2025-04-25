using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DishCombinationData", menuName = "Cooking/DishCombinationData")]
public class DishCombinationData : ScriptableObject
{
    public SerializedDictionary<string,Sprite> Sprites = new();

    public void RebuildTable()
    {
        Crafting.ComputeAllFamilyCombination(out HashSet<string> keys);
        
        Sprites.Clear();
        foreach (string key in keys)
        {
            Sprites.Add(key, null);
        }
    }
}
