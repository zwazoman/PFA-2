using System.Collections.Generic;
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
        else if (container.item is Sauce && sauce == null)
        {
            items[3] = container;
            sauce = (Sauce)container.item;
            container.Reset();
        }
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
            return true;
        }
        //sauce
        else if (container.item is Sauce && sauce == null)
        {
            items[3] = container;
            sauce = (Sauce)container.item;
            return true;
        }

        return false;
    }

}
