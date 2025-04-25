using UnityEngine;


[CreateAssetMenu(fileName = "newIngredient", menuName = "Cooking/Ingredient")]
public class Ingredient : IngredientBase
{
    public IngredientsInfo.Family family;

    [Header("Stats flat")]

    public float DamageIncrease = 0;
    public float HealIncrease = 0;
    public byte RecoilIncrease = 0;
    public float ShieldAmountIncrease = 0;
    public byte CoolDownIncrease = 1;

    [Header("Multipliers")]

    public float DamageMultiplier = 1;
    public float HealMultiplier = 1,
        RecoilMultiplier = 1,
        ShieldAmountMultiplier = 1,
        CoolDownMultiplier = 1;

    public override void ModifySpellEffect(SpellData Spell)
    {
        Spell.Damage += DamageIncrease;
        Spell.Heal += HealIncrease;
        Spell.Recoil += RecoilIncrease;
        Spell.ShieldAmount += ShieldAmountIncrease;
        Spell.CoolDown += CoolDownIncrease;
    }

    public override void OnAfterModifySpellEffect(SpellData Spell)
    {
        Spell.Damage *= DamageMultiplier;
        Spell.Heal *= HealMultiplier;
        Spell.Recoil = Mathf.CeilToInt(Spell.Recoil * RecoilMultiplier);
        Spell.ShieldAmount *= ShieldAmountMultiplier;
        Spell.CoolDown *= Mathf.CeilToInt(Spell.CoolDown * CoolDownMultiplier); ;

        base.OnAfterModifySpellEffect(Spell);
    }
}
