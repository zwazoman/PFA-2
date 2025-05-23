using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DishCombinationData", menuName = "Cooking/DishCombinationData")]
public class DishCombinationData : ScriptableObject
{
    public SerializedDictionary<string, spellVisualData> Visuals = new();

    public void RebuildTable()
    {
        Crafting.ComputeAllFamilyCombination(out HashSet<string> keys);

        Visuals.Clear();
        foreach (string key in keys)
        {
            Visuals.Add(key, new spellVisualData());
        }
    }

    [Serializable]
    public struct spellVisualData
    {
        public Sprite sprite;
        public string name;
    }
}
