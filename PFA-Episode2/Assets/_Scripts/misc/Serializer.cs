public static class Serializer
{
    //spell effect
    static readonly string[] SpellTypeToString =
    {
        "Damage","Recoil","Shield","Damage per hit ennemy","damage increase per distance","Fire"
    };
    static readonly char[] statTypeToString =
    {
        '+','x'
    };
    public static string GetSpellEffectString(SpellEffect e)
    {
        string s =
            SpellTypeToString[(int)e.effectType] + " "
            + statTypeToString[(int)e.statType]
            + " " + e.value;
        return s;
    }

    //ingredient effect
    static readonly string[] IngredientEffectTypeToString =
    {
        "Damage","Recoil","Shield"
    };

    public static string GetIngredientEffectString(Ingredient e)
    {
        string s =
            IngredientEffectTypeToString[(int)e.EffectType] + " "
            + statTypeToString[(int)e.EffectStatType]
            + " " + e.EffectValue;
        return s;
    }
}
