using UnityEngine;

public static class Serializer
{
    //spell effect
    static readonly string[] SpellEffectTypeToString =
    {
        "Damage","Knockback","Shield","Multi-hit Damage","damage with distance","Melee multiplier","Summon Entity"
    };

    static readonly char[] statTypeToString =
    {
        '+','x'
    };

    static readonly string[] SauceEffectTypeToString =
    {
        "No Special effect","Cast through walls","Multi-hit Damage","Damage with range", "Melee multiplier", "Summon Entity"
    };

    public static string GetSpellEffectString(SpellEffect e)
    {
        string s =
            SpellEffectTypeToString[(int)e.effectType] + " "
            + statTypeToString[(int)e.statType]
            + (e.value != -1 ? " " + e.value : "");
        return s;
    }

    //ingredient effect
    static readonly string[] IngredientEffectTypeToString =
    {
        "Damage","Knockback","Shield","Range"
    };

    public static string GetIngredientEffectString(Ingredient e)
    {
        string s =
            IngredientEffectTypeToString[(int)e.EffectType] + " "
            + statTypeToString[(int)e.EffectStatType]
            + " " + e.EffectValue;
        return s;
    }

    public static string GetSauceEffectString(Sauce e)
    {
        return SauceEffectTypeToString[(int)e.effect];
    }

    const string s = "s", cd = " turn", r = " tile", t = " - ",e = "";
    public  static string GetCoolDownString(byte cooldown) => cooldown.ToString() + cd + (cooldown> 1 ? s : e);
    public static string GetRangeString(byte Range)
        => Mathf.Max(0, Range - SpellCaster.RangeRingThickness).ToString() + t + Range.ToString() + r + (Range > 1 ? s : e);
}
