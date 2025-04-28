using System.Collections.Generic;
using UnityEngine;

public class CookingPot : MonoBehaviour
{
    public bool isFull = false;

    DraggableItemContainer[] items = new DraggableItemContainer[4];
    Ingredient[] ingredients = new Ingredient[3];
    byte ingredientCount = 0;

    Sauce sauce;

    public bool TryAddIngredient(DraggableItemContainer container)
    {
        //ingredient
        if(container.item is Ingredient && ingredientCount < 3)
        {
            items[ingredientCount] = container;
            ingredients[ingredientCount] = ((Ingredient)container.item);

            ingredientCount++;

            return true;
        }
        //sauce
        else if(container.item is Sauce && sauce == null)
        {
            items[3] = container;
            sauce = (Sauce)container.item;
            return true;
        }

        return false;
    }
}
