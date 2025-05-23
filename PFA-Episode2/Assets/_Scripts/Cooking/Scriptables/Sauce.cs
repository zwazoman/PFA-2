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
            case SauceEffectType.Fire:
                throw new NotImplementedException("Le feu est pas codé dans le jeu encore");
                break;
        }

        Spell.AreaOfEffect = areaOfEffect;
    }
}
