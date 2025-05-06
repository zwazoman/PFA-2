using System.Collections.Generic;
using UnityEngine;

public class SaveReference : MonoBehaviour
{
    #region Singleton
    public static SaveReference Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public List<Sprite> Sprites;
    public List<Ingredient> Ingredients;
    public List<Sauce> Sauces;
    public List<AreaOfEffect> AreaOfEffects;

    public Sprite GetSprite(string name)
    {
        Sprite sprite = null;

        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i].name == name)
            {
                sprite = Sprites[i];
            }
        }

        return sprite;
    }

    public Ingredient GetIngredient(string name)
    {
        Ingredient ingredient = null;

        for (int i = 0; i < Ingredients.Count; i++)
        {
            if (Ingredients[i].name == name)
            {
                ingredient = Ingredients[i];
            }
        }

        return ingredient;
    }

    public Sauce GetSauce(string name)
    {
        Sauce sauce = null;

        for (int i = 0; i < Sauces.Count; i++)
        {
            if (Sauces[i].name == name)
            {
                sauce = Sauces[i];
            }
        }

        return sauce;
    }

    public AreaOfEffect GetAreaOfEffect(string name)
    {
        AreaOfEffect areaOfEffect = null;

        for (int i = 0; i < AreaOfEffects.Count; i++)
        {
            if (AreaOfEffects[i].name == name)
            {
                areaOfEffect = AreaOfEffects[i];
            }
        }

        return areaOfEffect;
    }
}
