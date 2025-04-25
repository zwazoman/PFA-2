using UnityEngine;

[CreateAssetMenu(fileName = "newSauce", menuName = "Cooking/Sauce")]
public class Sauce : IngredientBase
{
    public AreaOfEffect areaOfEffect;
    public StatusEffect statusEffect;

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

    public override void ModifySpellEffect(SpellData Spell)
    {
        throw new System.NotImplementedException();
    }
}
