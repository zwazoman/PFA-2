using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Kitchen : MonoBehaviour
{

    HashSet<List<IngredientFamily>> combinations;

    IngredientFamily[] families = new IngredientFamily[]
    {
        IngredientFamily.Meat,
        IngredientFamily.Vegetables,
        IngredientFamily.Starchys,
        IngredientFamily.Dairys
    };

    void boule()
    {
        List<IngredientFamily> combination = new();

        for(int i = 0; i < 4; i++) 
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    combination.Add(families[i]);
                    combination.Add(families[j]);
                    combination.Add(families[k]);
                    combination.Sort();
                    combinations.Add(combination);
                }
            }
        }
    }

    public void CraftNewSpell(Ingredient[] ingredients, Sauce sauce)
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

    }
}
