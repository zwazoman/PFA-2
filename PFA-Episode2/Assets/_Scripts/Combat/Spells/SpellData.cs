using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class SpellData
{

    /// <summary>
    /// le nom du spell
    /// </summary>
    public string Name;

    List<String> IngredientNames = new List<String>();

    public short SpriteReference;

    public bool IsOccludedByWalls = true;

    [Header("Stats")]

    /// <summary>
    /// le cout en mana du spell
    /// </summary>
    public float ManaCost = 0;

    /// <summary>
    /// Les dégats du spell
    /// </summary>
    public float Damage = 0;

    /// <summary>
    /// Le nombre de cases de recul du spell
    /// </summary>
    public int Recoil = 0;

    /// <summary>
    /// Baissera au fur à mesur du temps après avoir été appliqué.
    /// </summary>
    public float ShieldAmount = 0;

    /// <summary>
    /// Le nombre de tour avant de pouvoir réutiliser le sort
    /// </summary>
    public int CoolDown = 1;

    public StatusEffect StatusEffect = StatusEffect.None;


    [Header("Zone")]
    public AreaOfEffect AreaOfEffect;
    
    
}

[CreateAssetMenu(fileName = "new spell", menuName = "Combat/PremadeSpell")]
public class PremadeSpell : ScriptableObject
{
    public SpellData SpellData;
}
