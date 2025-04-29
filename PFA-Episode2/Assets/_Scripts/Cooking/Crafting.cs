using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Crafting : MonoBehaviour
{
    public static void CraftNewSpell(Ingredient[] ingredients, Sauce sauce)
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
        return ComputeFamilyCombinaison(i1.family, i2.family, i3.family);
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
