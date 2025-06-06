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
    public Mesh Mesh;

    public string IngredientsCombination;

    [Header("Casting")]

    public bool IsOccludedByWalls = true;
    public int Range = 2;
    public int CoolDown = 1;

    [Header("Effects")]
    public List<SpellEffect> Effects = new ();

    [Header("Zone")]
    public AreaOfEffect AreaOfEffect;

    public SpellData Copy()
    {
        return (SpellData)MemberwiseClone();
    }
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
        if (Effects.Length ==3) 
        {
            bool collapsedBontoA = TryCollapseBontoA(ref Effects[0], Effects[1]);
            bool collapsedContoA = TryCollapseBontoA(ref Effects[0], Effects[2]);

            if (collapsedBontoA && collapsedContoA) Effects = new SpellEffect[] { Effects[0] };
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
        else if(Effects.Length == 2)
        {
            bool collapsedBontoA = TryCollapseBontoA(ref Effects[0], Effects[1]);
            
            if (collapsedBontoA) Effects = new SpellEffect[] { Effects[0] };
            else Effects = new SpellEffect[] { Effects[0], Effects[1] };
        }
        else if(Effects.Length ==1)
        {
            Effects = new SpellEffect[] { Effects[0] };
        }
        else if(Effects.Length == 0)
        {
            Effects = new SpellEffect[] { new SpellEffect(SpellEffectType.Damage,StatType.FlatIncrease,2)};
        }
        else
        {
            Debug.LogError("y'a un pb là connard");
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
                    Debug.Log("Multiplier Collapsed !!!!!!");
                    break;

            }
            return true;
        }
        return (false);
    }

    

    #endregion
}

