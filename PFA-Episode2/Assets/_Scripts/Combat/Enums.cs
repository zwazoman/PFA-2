public enum SauceEffectType
{
    None,
    DisableLineOfSight,
    /// <summary>
    /// degat = degat + DamageIncreaseForEachHitEnnemy * SpellCastingContext.numberOfHitEnnemies
    /// </summary>
    DamageIncreaseForEachHitEnnemy,

    /// <summary>
    /// en pourcentage : +n% par case
    /// degat = degat * (1+SpellCastingContext.DistanceToPlayer * DamageIncreaseByDistanceToCaster / 100f)
    /// </summary>
    DamageIncreasePercentageByDistanceToCaster,
    Fire
}

public enum IngredientEffectType
{
    Damage,
    Recoil,
    Shield,
    Range
}

public enum SpellEffectType
{
    Damage,
    Recoil,
    Shield,
    /// <summary>
    /// degat = degat + DamageIncreaseForEachHitEnnemy * SpellCastingContext.numberOfHitEnnemies
    /// </summary>
    DamageIncreaseForEachHitEnnemy,

    /// <summary>
    /// en pourcentage : +n% par case
    /// degat = degat * (1+SpellCastingContext.DistanceToPlayer * DamageIncreaseByDistanceToCaster / 100f)
    /// </summary>
    DamageIncreasePercentageByDistanceToCaster,
    Fire
}

public enum StatType
{
    FlatIncrease,
    Multiplier,
    PercentageIncrease
}

public static class IngredientsInfo
{
    public enum Family
    {
        Meat,
        Vegetables,
        Starchys,//feculent
        Dairys,
    }
    public const byte FamilyCount = 4;

    public static Family[] families = new IngredientsInfo.Family[]
    {
        IngredientsInfo.Family.Meat,
        IngredientsInfo.Family.Vegetables,
        IngredientsInfo.Family.Starchys,
        IngredientsInfo.Family.Dairys
    };
}


public enum CookingItem
{
    None,
    BBQ,
    Wok,
    Oven,
}

public enum Rarity
{
    Ordinaire,
    Savoureux,
    Divin
}

