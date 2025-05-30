using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Crafting : MonoBehaviour
{
    public static SpellData CraftNewSpell(Ingredient[] ingredients, Sauce sauce)
    {
        //apply ingredients effects
        SpellData spell = new SpellData();
        foreach(Ingredient i in ingredients)
        {
            i.ModifySpellEffect(spell);
        }


        Debug.Log(" = Raw spell effects = ");
        foreach(SpellEffect e in  spell.Effects)Debug.Log(e.effectType);

        //collapse similar effects
        SpellEffect[] effects = spell.Effects.ToArray();
        Debug.Log(effects.Length);
        SpellEffect.CollapseSimilarSpellEffects(ref effects);
        spell.Effects = effects.ToList();

        Debug.Log(" = collapsed spell effects = ");
        foreach (SpellEffect e in spell.Effects) Debug.Log(e.effectType);

        sauce.ModifySpellEffect(spell);

        //apply multipliers
        SortSpellEffects(spell.Effects);

        //compute family combination
        spell.IngredientsCombination = ComputeFamilyCombinaison(ingredients[0], ingredients[1], ingredients[2]);

        //fetch visual data
        GameStaticData.spellVisualData data = GameManager.Instance.staticData.Visuals[spell.IngredientsCombination];
        spell.Name = data.name;
        spell.Sprite = data.sprite;
        spell.Mesh = data.mesh;

        //remove ingredients from player inventory
        try
        {
            foreach (Ingredient i in ingredients)
            {
                GameManager.Instance.playerInventory.Ingredients.Remove(i);
            }
            GameManager.Instance.playerInventory.Sauces.Remove(sauce);
        }
        catch(Exception e)
        {
            Debug.LogException(new Exception("Couldnt remove ingredients from player inventory.",e));
        }

        Debug.Log(" = Final spell effects =");
        foreach (SpellEffect e in spell.Effects) Debug.Log(e.effectType);

        return spell;
    }

    public static void SortSpellEffects(List<SpellEffect> effects) //@todo
    {

        effects.Sort(new SpellComparer());
        foreach(SpellEffect effect in effects)
        {
            Debug.Log(effect.statType);
        }
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

public class SpellComparer : IComparer<SpellEffect>
{
    public int Compare(SpellEffect x, SpellEffect y)
    {
        return ((int)x.statType).CompareTo((int)y.statType);
    }
}

