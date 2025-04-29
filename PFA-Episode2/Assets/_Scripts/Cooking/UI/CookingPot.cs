using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    public bool isFull = false;

    DraggableItemContainer[] items = new DraggableItemContainer[4];

    //ingredients
    Ingredient[] ingredients = new Ingredient[3];
    byte ingredientCount = 0;
    Sauce sauce;

    [Header("sceneReferences")]
    [SerializeField] CookingIngredientsInfoPanel ingredientInfo0;
    [SerializeField] CookingIngredientsInfoPanel ingredientInfo1;
    [SerializeField] CookingIngredientsInfoPanel ingredientInfo2;

    void UpdateIngredientsStats()
    {
        ingredientInfo0.UpdateVisual(ingredients[0]);
        ingredientInfo1.UpdateVisual(ingredients[1]);
        ingredientInfo2.UpdateVisual(ingredients[2]);
    }

    public bool TryAddIngredient(DraggableItemContainer container)
    {
        Debug.Log("Tried adding new ingredient : " + ((IngredientBase)(container.item)).name);
        //ingredient
        if (container.item is Ingredient && ingredientCount < 3)
        {
            items[ingredientCount] = container;
            ingredients[ingredientCount] = ((Ingredient)container.item);

            ingredientCount++;

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
