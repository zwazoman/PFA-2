using UnityEngine;

public enum StatusEffect
{
    None,
    Fire
}

public enum SauceEffect
{
    None,
    DisableLineOfSight
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
    Common,
    Rare,
    Epic,
    Legendary
}

