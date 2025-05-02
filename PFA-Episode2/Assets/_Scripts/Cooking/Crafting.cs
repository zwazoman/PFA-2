using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    public static SpellData CraftNewSpell(Ingredient[] ingredients, Sauce sauce)
    {
        SpellData spell = new SpellData();
        foreach(Ingredient i in ingredients)
        {
            i.ModifySpellEffect(spell);
        }

        foreach (Ingredient i in ingredients)
        {
            i.OnAfterModifySpellEffect(spell);
        }

        sauce.ModifySpellEffect(spell);

        SpellEffect[] effects = spell.Effects.ToArray();
        SpellEffect.CollapseSpellEffects(ref effects);
        spell.Effects = effects.ToList();

        return spell;
    }

    #region combinations
    public static void ComputeAllFamilyCombination(out HashSet<string> combinations)
    {
        combinations = new();
        List<IngredientsInfo.Family> combination = new();
        for (int i = 0; i < IngredientsInfo.FamilyCount; i++)
        {
            for (int j = 0; j < IngredientsInfo.FamilyCount; j++)
            {
                for (int k = 0; k < IngredientsInfo.FamilyCount; k++)
                {
                    combinations.Add(ComputeFamilyCombinaison(IngredientsInfo.families[i], IngredientsInfo.families[j], IngredientsInfo.families[k], ref combination));
                }
            }
        }
    }

    public static string ComputeFamilyCombinaison(
        Ingredient i1,
        Ingredient i2,
        Ingredient i3)
    {
        return ComputeFamilyCombinaison(i1.Family, i2.Family, i3.Family);
    }

    public static string ComputeFamilyCombinaison(
        IngredientsInfo.Family firstFamily,
        IngredientsInfo.Family secondFamily,
        IngredientsInfo.Family thirdFamily)
    {
        List<IngredientsInfo.Family> workerlist = new();
        return ComputeFamilyCombinaison(firstFamily, secondFamily, thirdFamily, ref workerlist);
    }
    public static string ComputeFamilyCombinaison(
        IngredientsInfo.Family firstFamily,
        IngredientsInfo.Family secondFamily,
        IngredientsInfo.Family thirdFamily,
        ref List<IngredientsInfo.Family> workerlist)
    {
        workerlist.Clear();
        workerlist.Add(firstFamily);
        workerlist.Add(secondFamily);
        workerlist.Add(thirdFamily);
        workerlist.Sort();

        string s = "";
        foreach (var c in workerlist) s += c.ToString();
        return s;
    }

    #endregion
}
