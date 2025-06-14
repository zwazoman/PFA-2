using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoHeader : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] TMP_Text text;
    [SerializeField] Image image;

    public void UpdateVisual(string title = "", Sprite sprite = null)
    {
        if (title == "")
        {
            text.text = "...";
            image.enabled = sprite != null;
        }
        else
        {
            text.text = title;
            image.sprite = sprite;
            image.enabled = sprite != null;
        }
            
        
        //while(text.textBounds)
    }

    public void UpdateVisual(Ingredient ingredient)
    {
        if (ingredient == null)
        {
            UpdateVisual();
        }
        else
        {
            UpdateVisual(Serializer.GetIngredientEffectString(ingredient), ingredient.sprite);
        }
    }

    public void UpdateVisual(SpellEffect? spellEffect,Sprite sprite)
    {
        if (!spellEffect.HasValue)
        {
            UpdateVisual();
        }
        else
        {
            UpdateVisual(Serializer.GetSpellEffectString(spellEffect.Value), sprite);
        }
    }
}
