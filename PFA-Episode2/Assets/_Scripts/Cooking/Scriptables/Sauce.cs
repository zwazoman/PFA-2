using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newSauce", menuName = "Cooking/Sauce")]
public class Sauce : IngredientBase
{
    [Header("Sauce Effect")]
    public AreaOfEffect areaOfEffect;
    public SauceEffectType effect;

    public override void ModifySpellEffect(SpellData Spell)
    {
        switch (effect)
        {
            case SauceEffectType.None:
                break;
            case SauceEffectType.DisableLineOfSight:
                Spell.IsOccludedByWalls = false; 
                break;
            case SauceEffectType.DamageIncreaseForEachHitEnnemy:
                Spell.Effects.Add(new(SpellEffectType.DamageIncreaseForEachHitEnnemy,StatType.FlatIncrease,-1));
                break;
            case SauceEffectType.DamageIncreasePercentageByDistanceToCaster:
                Spell.Effects.Add(new(SpellEffectType.DamageIncreasePercentageByDistanceToCaster, StatType.Multiplier, 1.1f));
                break;
            case SauceEffectType.DamageIncreaseMeleeRange:
                Spell.Effects.Add(new(SpellEffectType.DamageIncreaseMeleeRange, StatType.Multiplier, 1.5f));
                break;
            case SauceEffectType.SummonEntity:
                Spell.Effects.Add(new(SpellEffectType.EntitySummon, StatType.FlatIncrease, 1));
                break;

        }

        Spell.AreaOfEffect = areaOfEffect;
    }
}
