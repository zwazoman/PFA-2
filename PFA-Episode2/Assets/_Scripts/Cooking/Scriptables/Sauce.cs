using UnityEngine;

[CreateAssetMenu(fileName = "newSauce", menuName = "Cooking/Sauce")]
public class Sauce : IngredientBase
{
    [Header("Sauce Effect")]
    public AreaOfEffect areaOfEffect;
    public SauceEffectType effect;
    public Rarity rarity;

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
                Spell.Effects.Add(new(SpellEffectType.DamageIncreasePercentageByDistanceToCaster, StatType.PercentageIncrease, -1));
                break;
            case SauceEffectType.Fire:
                Spell.Effects.Add(new(SpellEffectType.Fire, StatType.PercentageIncrease, -1));
                break;
        }

        Spell.AreaOfEffect = areaOfEffect;
    }
}
