using UnityEngine;


[CreateAssetMenu(fileName = "newIngredient", menuName = "Cooking/Ingredient")]
public class Ingredient : IngredientBase
{
    [Header("Ingredient")]
    public IngredientsInfo.Family Family;
    public Mesh mesh;

    [Header("Effect")]
    public IngredientEffectType EffectType;
    public StatType EffectStatType;
    public float EffectValue;

    [Header("Stats")]
    public byte CoolDownIncrease;

    public override void ModifySpellEffect(SpellData Spell)
    {
        switch (EffectType)
        {
            case IngredientEffectType.Damage:
                Spell.Effects.Add(new(SpellEffectType.Damage, EffectStatType, EffectValue));
                break;
            case IngredientEffectType.Recoil:
                Spell.Effects.Add(new(SpellEffectType.Recoil, EffectStatType, EffectValue));
                break;
            case IngredientEffectType.Shield:
                Spell.Effects.Add(new(SpellEffectType.Shield, EffectStatType, EffectValue));
                break;
            case IngredientEffectType.Range:
                Spell.Range = (byte)Mathf.Max(1, Spell.Range + EffectValue);
                break;
        }

        Spell.CoolDown += CoolDownIncrease;

    }


}
