using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingIngredientsInfoPanel : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] TMP_Text text;
    [SerializeField] Image image;

    public void UpdateVisual(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            text.text = "";
            image.sprite = null;
        }
        else
        {
            text.text = Serializer.GetIngredientEffectString(ingredient);
            image.sprite = ingredient.sprite;
        }
    }
}
