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

    List<String> IngredientNames = new List<String>();

    public short SpriteReference;

    [Header("Casting")]

    public bool IsOccludedByWalls = true;
    public byte Range;

    [Header("Flat Stats")]


    /// <summary>
    /// Les dégats du spell
    /// </summary>
    public float Damage = 0;

    /// <summary>
    /// Les dégats du spell
    /// </summary>
    public float Heal = 0;

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



    [Header("Context Dependant Stats")] //y'a un struct SpellCastingContext

    /// <summary>
    /// degat = degat + DamageIncreaseForEachHitEnnemy * SpellCastingContext.numberOfHitEnnemies
    /// </summary>
    public float DamageIncreaseForEachHitEnnemy = 0;

    [Tooltip("Pourcentage de degats bonus par case")]
    /// <summary>
    /// en pourcentage : +n% par case
    /// degat = degat * (1+SpellCastingContext.DistanceToPlayer * DamageIncreaseByDistanceToCaster / 100f)
    /// </summary>
    public float DamageIncreasePercentageByDistanceToCaster = 0;

    [Header("Effect")]
    public StatusEffect StatusEffect = StatusEffect.None;

    [Header("Zone")]
    public AreaOfEffect AreaOfEffect;
    
}

[CreateAssetMenu(fileName = "new spell", menuName = "Combat/PremadeSpell")]
public class PremadeSpell : ScriptableObject
{
    public SpellData SpellData;
}
