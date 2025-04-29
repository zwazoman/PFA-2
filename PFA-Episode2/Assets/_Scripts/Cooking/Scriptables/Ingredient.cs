using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


[CreateAssetMenu(fileName = "newIngredient", menuName = "Cooking/Ingredient")]
public class Ingredient : IngredientBase
{
    [Header("Ingredient")]
    public IngredientsInfo.Family family;

    [Header("Effect")]
    public IngredientEffectType effectType;
    public StatType effectStatType;
    public float effectValue;

    [Header("Stats")]
    public byte CoolDownIncrease;

    public override void ModifySpellEffect(SpellData Spell)
    {
        switch (effectType)
        {
            case IngredientEffectType.Damage:
                Spell.Effects.Add(new(SpellEffectType.Damage,effectStatType, effectValue));
                break;
            case IngredientEffectType.Recoil:
                Spell.Effects.Add(new(SpellEffectType.Recoil, effectStatType, effectValue));
                break;
            case IngredientEffectType.Range:
                Spell.Range += (byte)Mathf.RoundToInt(effectValue);
                break;
            case IngredientEffectType.Shield:
                Spell.Effects.Add(new(SpellEffectType.Shield, effectStatType, effectValue));
                break;
        }

        Spell.CoolDown += CoolDownIncrease;

    }


}
