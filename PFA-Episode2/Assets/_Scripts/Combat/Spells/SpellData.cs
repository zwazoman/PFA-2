using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellData
{
    [Header("Visuals")]
    /// <summary>
    /// le nom du spell
    /// </summary>
    public string Name;
    public Sprite Sprite;

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
    
    public SpellEffect(SpellEffectType effectType, StatType statType, float value)
    {
        this.effectType = effectType;
        this.statType = statType;
        this.value = value;
    }

    #region spellCollapse
    public static void CollapseSimilarSpellEffects(ref SpellEffect[] Effects)
    {
        bool collapsedBontoA = TryCollapseBontoA(ref Effects[0], Effects[1]); 
        bool collapsedContoA = TryCollapseBontoA(ref Effects[0], Effects[2]);

        if (collapsedBontoA && collapsedContoA) Effects = new SpellEffect[] { Effects[0]};
        else if (collapsedBontoA && !collapsedContoA)
        {
            Effects = new SpellEffect[] { Effects[0], Effects[2] };
        }
        else if (!collapsedBontoA && collapsedContoA)
        {
            Effects = new SpellEffect[] { Effects[0], Effects[1] };
        }
        else if (!(collapsedBontoA || collapsedContoA))
        {
            bool collapsedCOntoB = TryCollapseBontoA(ref Effects[1], Effects[2]);
            if (collapsedCOntoB) Effects = new SpellEffect[] { Effects[0], Effects[2] };
            else return;
        }
    }

    static bool TryCollapseBontoA(ref SpellEffect a, SpellEffect b)
    {
        if( a.effectType == b.effectType && a.statType == b.statType) 
        {
            switch (a.statType)
            {
                case StatType.FlatIncrease:
                    a.value += b.value;
                    break;
                case StatType.Multiplier:
                    a.value *= b.value;
                    break;
                case StatType.PercentageIncrease:
                    float m1 = a.value * .01f + 1;
                    float m2 = b.value * .01f + 1;
                    a.value = (m1 * m2 - 1f) * 100f;
                    break;
            }
            return true;
        }
        return (false);
    }

    #endregion
}

