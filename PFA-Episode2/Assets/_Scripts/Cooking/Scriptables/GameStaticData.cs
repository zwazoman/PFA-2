using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DishCombinationData", menuName = "Cooking/DishCombinationData")]
public class GameStaticData : ScriptableObject
{
    public SerializedDictionary<string, spellVisualData> Visuals = new();

    
   // public SerializedDictionary<Rarity, Sprite> framesPerRarity = new();

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
        public Mesh mesh;
    }

    [Header("Spawnable Entities")]
    public GameObject kamikaze;

    [Header("Tile materials")]
    public Material _mat_movementPreview;
    public Material _mat_spellAoePreview;
    public Material _mat_spellAoePreview_shield;

    public Material _mat_spellCastZonePreview;
    public Material _mat_OccludedspellCastZonePreview;
}
