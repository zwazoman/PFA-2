using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class SpellData
{
    [Header("Visuals")]
    /// <summary>
    /// le nom du spell
    /// </summary>
    public string Name;

    public string IngredientsCombination;

    [Header("Casting")]

    public bool IsOccludedByWalls = true;
    public byte Range;
    public byte CoolDown;

    [Header("Effects")]
    public List<SpellEffect> Effects = new ();

    [Header("Zone")]
    public AreaOfEffect AreaOfEffect;
    
}

[Serializable]
public struct SpellEffect
{
    public SpellEffectType effectType;
    public StatType statType;
    public float value;
    
    static readonly string[] SpellTypeToString = 
    {
        "Damage","Recoil","Shield","Damage per hit ennemy","damage increase per distance","Fire"
    };

    static readonly char[] statTypeToString =
    {
        '+','x'
    };

    public SpellEffect(SpellEffectType effectType, StatType statType, float value)
    {
        this.effectType = effectType;
        this.statType = statType;
        this.value = value;
    }

    public override string ToString()
    {
        string s = 
            SpellTypeToString[(int)effectType] + " " 
            + statTypeToString[(int)statType] 
            + " " + value;
        return s;
    }
}

