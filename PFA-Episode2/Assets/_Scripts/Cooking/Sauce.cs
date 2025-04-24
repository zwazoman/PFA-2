using UnityEngine;

[CreateAssetMenu(fileName = "newSauce", menuName = "Cooking/Sauce")]
public class Sauce : IngredientBase
{
    public AreaOfEffect areaOfEffect;
    public StatusEffect statusEffect;

    public override void ModifySpellEffect(SpellData Spell)
    {
        throw new System.NotImplementedException();
    }
}
