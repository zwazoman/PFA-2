using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Image _sauceAreaImage;
    [SerializeField] TMP_Text _txt_cooldown, _txt_range;

    [Header("Asset References")]
    [SerializeField] Sauce _defaultSauce;

    #region Display

    private void UpdateCooldownAndRangeDisplay()
    {
        byte cd = 0, range = 0;
        foreach(Ingredient ing in ingredients)
        {
            cd += ing.CoolDownIncrease;
        }
        _txt_cooldown.text = Serializer.GetCoolDownString(cd);
    }

    void UpdateIngredientsStatsDisplay()
    {
        ingredientInfo0.UpdateVisual(ingredients.Count >= 1 ? ingredients[0] : null);
        ingredientInfo1.UpdateVisual(ingredients.Count >= 2 ? ingredients[1] : null);
        ingredientInfo2.UpdateVisual(ingredients.Count >= 3 ? ingredients[2] : null);
    }

    void UpdateSauceDisplay() //@revoir image zone
    {
        Sauce s = sauce == null ? _defaultSauce : sauce;

        txt_sauceName.text = s.name;
        txt_sauceEffect.text = Serializer.GetSauceEffectString(s);
        _sauceAreaImage.sprite = s.areaOfEffect.sprite;

    }

    void UpdateDisplay()
    {
        UpdateSauceDisplay();
        UpdateIngredientsStatsDisplay();
        UpdateCooldownAndRangeDisplay();
    }

    private void Start()
    {
        UpdateDisplay();
    }

    #endregion

    #region logic

    public void RemoveAllIngredients()
    {
        foreach (DraggableIngredientContainer item in items) 
        {
            if(item!=null) RemoveIngredient(item,false);
        }

        UpdateDisplay();
    }

    public void RemoveIngredient(DraggableIngredientContainer container , bool shake = true)
    {
        bool removed = false;
        //ingredient
        if (removed |= (container.item is Ingredient && ingredients.Contains((Ingredient)container.item)))
        {
            items[ingredients.IndexOf((Ingredient)container.item)] = null;
            ingredients.Remove ((Ingredient)container.item);
            container.Reset();

            UpdateIngredientsStatsDisplay();
            UpdateCooldownAndRangeDisplay();
            
        }
        //sauce
        else if (removed |=  (container.item is Sauce && sauce != null))
        {
            sauce = null;
            container.Reset();

            UpdateSauceDisplay();
        }

        if (removed && shake) transform.DOShakeRotation(.2f,Vector3.forward*90,randomnessMode : ShakeRandomnessMode.Harmonic);
        
    }

    public bool TryAddIngredient(DraggableIngredientContainer container)
    {
        Debug.Log("Tried adding new ingredient : " + ((IngredientBase)(container.item)).name);

        bool successful = false;

        //ingredient
        if (successful|=(container.item is Ingredient && ingredients.Count < 3))
        {
            items[ingredients.Count] = container;
            ingredients.Add((Ingredient)container.item);

            UpdateIngredientsStatsDisplay();
            UpdateCooldownAndRangeDisplay();
        }
        //sauce
        else if (successful |= (container.item is Sauce && sauce == null))
        {
            items[3] = container;
            sauce = (Sauce)container.item;

            UpdateSauceDisplay();
        }

        if (successful) transform.DOPunchScale(Vector3.one * .25f, .25f,9,1.2f); else transform.DOShakePosition(.3f,50,20);

        return successful;
    }

    public bool TryCookSpell(out SpellData spell)
    {
        spell = null;
        if (ingredients.Count != 3) return false;
        //if (sauce == null) return false;

        spell = Crafting.CraftNewSpell(ingredients.ToArray(),sauce == null ? _defaultSauce : sauce);
        GameManager.Instance.playerInventory.Spells.Add(spell);

        return true;
    }

    #endregion

}
