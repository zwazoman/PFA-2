using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    public bool isFull = false;

    DraggableItemContainer[] items = new DraggableItemContainer[4];

    //ingredients
    List<Ingredient> ingredients = new ();
    Sauce sauce;

    [Header("sceneReferences")]
    [SerializeField] InfoHeader ingredientInfo0;
    [SerializeField] InfoHeader ingredientInfo1;
    [SerializeField] InfoHeader ingredientInfo2;


    [SerializeField] TMP_Text txt_sauceName;
    [SerializeField] TMP_Text txt_sauceEffect;
    [SerializeField] TMP_Text _txt_cooldown, _txt_range;

    private void UpdateCooldownAndRange()
    {
        Debug.Log("fzqesgrhzfegrht");
        byte cd = 0, range = 0;
        foreach(Ingredient ing in ingredients)
        {
            cd += ing.CoolDownIncrease;
            range += ing.RangeIncrease;
        }
        _txt_cooldown.text = Serializer.GetCoolDownString(cd);
        _txt_range.text = Serializer.GetRangeString(range);

        Debug.Log("RDCYFGTVUYIHUIJPKO¨POJHYGULDKYTCIHJUIOKPµPIHGUPYFKCGT");
    }

    void UpdateIngredientsStats()
    {
        ingredientInfo0.UpdateVisual(ingredients.Count >= 1 ? ingredients[0] : null);
        ingredientInfo1.UpdateVisual(ingredients.Count >= 2 ? ingredients[1] : null);
        ingredientInfo2.UpdateVisual(ingredients.Count >= 3 ? ingredients[2] : null);
    }

    public void RemoveIngredient(DraggableIngredientContainer container)
    {
        //ingredient
        if (container.item is Ingredient && ingredients.Contains((Ingredient)container.item))
        {
            items[ingredients.IndexOf((Ingredient)container.item)] = null;
            ingredients.Remove ((Ingredient)container.item);
            container.Reset();
            UpdateIngredientsStats();
        }
        //sauce
        else if (container.item is Sauce && sauce != null)
        {
            items[3] = container;
            sauce = (Sauce)container.item;
            container.Reset();

            txt_sauceName.text = "";
            txt_sauceEffect.text = "";
        }

        UpdateCooldownAndRange();
    }

    public bool TryAddIngredient(DraggableIngredientContainer container)
    {
        Debug.Log("Tried adding new ingredient : " + ((IngredientBase)(container.item)).name);
        //ingredient
        if (container.item is Ingredient && ingredients.Count < 3)
        {
            items[ingredients.Count] = container;
            ingredients.Add((Ingredient)container.item);
            UpdateIngredientsStats();

            UpdateCooldownAndRange();

            return true;
        }
        //sauce
        else if (container.item is Sauce && sauce == null)
        {
            items[3] = container;
            sauce = (Sauce)container.item;
            txt_sauceName.text = sauce.name;
            txt_sauceEffect.text = Serializer.GetSauceEffectString(sauce);

            UpdateCooldownAndRange();

            return true;
        }

        

        return false;
    }

    public bool TryCookSpell(out SpellData spell)
    {
        spell = null;
        if (ingredients.Count != 3) return false;
        if (sauce == null) return false;

        spell = Crafting.CraftNewSpell(ingredients.ToArray(),sauce);

        return true;
    }

}
