using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] TMP_Text txt_button;
    [SerializeField] RectTransform _buttonTransform;
    [SerializeField] Image _sauceAreaImage;
    [SerializeField] TMP_Text _txt_cooldown;
    [SerializeField] CookingPotVisuals Visuals3D;

    [Header("Asset References")]
    [SerializeField] Sauce _defaultSauce;

    //aimant vers draggable
    private Vector3 _basePosition;
    private bool _isBig = false;
    private RectTransform rTransform;

    private bool buttonIsShaking;
    
    #region Display
        
    #region buttonShake

    //todo : faire une classe bouttonShake ou quoi, c'est dégueu ça
    
    public async UniTask ShakeButton()
    {
        buttonIsShaking = true;
        while (buttonIsShaking)
        {
            _buttonTransform.DOShakeRotation(1, Vector3.forward * 5, 10);
            await _buttonTransform.DOShakePosition(1,Vector3.one*8,2,randomness:10f,randomnessMode:ShakeRandomnessMode.Harmonic).ToUniTask();
        }
    }
    public void StopButtonShake()
    {
        buttonIsShaking = false;
    }
    
    #endregion
    
    public void Grow()
    {
        _isBig = true;
        transform.DOScale(1.1f, .2f).SetEase(Ease.OutBack);
    }

    public void Shrink()
    {
        _isBig = false;
        transform.DOMove(_basePosition,.35f).SetEase(Ease.OutBack);
        transform.DOScale(1f, .35f).SetEase(Ease.OutBack);
    }

    private void Update()
    {
        if (_isBig)
        {
            Vector3 potToFInger =  (Vector2)Input.mousePosition - (Vector2)_basePosition - Vector2.up * (rTransform.sizeDelta.y * .1f);
            
            float alpha = Mathf.Clamp01(potToFInger.magnitude / 800 * 1080f / Screen.height);
            alpha = - alpha * (1f - alpha)*2;
            potToFInger = potToFInger.normalized * (75 * 1080f) / Screen.height * (alpha);
                
            Vector3 vel = new();
            transform.position = Vector3.SmoothDamp(transform.position, _basePosition - potToFInger + Vector3.up * (rTransform.sizeDelta.y * .1f),ref vel,.02f);
        }
    }

    private void UpdateCooldownAndRangeDisplay()
    {
        byte cd = 0;
        foreach(Ingredient ing in ingredients)
        {
            cd += ing.CoolDownIncrease;
        }
        _txt_cooldown.text = Serializer.GetCoolDownString((byte)(cd+1));
    }

    void UpdateIngredientsStatsDisplay()
    {
        ingredientInfo0.UpdateVisual(ingredients.Count >= 1 ? ingredients[0] : null);
        ingredientInfo1.UpdateVisual(ingredients.Count >= 2 ? ingredients[1] : null);
        ingredientInfo2.UpdateVisual(ingredients.Count >= 3 ? ingredients[2] : null);

        Visuals3D.UpdateIngredientsList(ingredients);

        txt_button.text = ingredients.Count < 3 ? $"Ingredients :\n{ingredients.Count}/3" : "Cook !";
        if (ingredients.Count == 3) ShakeButton(); else StopButtonShake();
        
        _buttonTransform.DOPunchScale(Vector3.one * .1f, .2f);

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

    private async void Start()
    {
        TryGetComponent(out rTransform);
        UpdateDisplay();

        await UniTask.NextFrame();
        _basePosition = transform.position;
        
    }

    #endregion

    #region logic

    public void RemoveAllIngredients()
    {
        //Debug.unityLogger.Log("== Removing all ingredients ==");
        //Debug.Log("Item count : " + items.Length);
        
        //string s = "";
        //foreach (DraggableIngredientContainer item in items.ToList()) s+= (item?.item.name?? "null") + " ";
        //Debug.Log("Item list : " + s);
        
        foreach (DraggableIngredientContainer item in items.ToList()) 
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
            
            items[items.ToList().IndexOf((DraggableItemContainer)container)] = null;
            ingredients.Remove((Ingredient)container.item);
            container.Reset();

            UpdateIngredientsStatsDisplay();
            UpdateCooldownAndRangeDisplay();
            
        }
        //sauce
        else if (removed |=  (container.item is Sauce && sauce != null))
        {
            sauce = null;
            container.Reset();
            items[3] = null;
            UpdateSauceDisplay();
        }

        if (removed && shake) transform.DOShakeRotation(.2f,Vector3.forward*90,randomnessMode : ShakeRandomnessMode.Harmonic);

        string s = "";
        foreach (Ingredient ingredient in ingredients) s+= ingredient.name+ " ";
        Debug.Log("Removed ingredient. new list : " + s);
    }

    public bool TryAddIngredient(DraggableIngredientContainer container)
    {
        Debug.Log("Tried adding new ingredient : " + ((IngredientBase)(container.item)).name);

        bool successful = false;

        //ingredient
        if (successful|=(container.item is Ingredient && ingredients.Count < 3))
        {
            int i = 0;
            while (i<3 && items[i]!=null) i++;
            items[i] = container;
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

        if (successful)
        {
            SFXManager.Instance.PlaySFXClip(Sounds.Sizzle);

            string s = "";
            foreach (Ingredient ingredient in ingredients) s+= ingredient.name+ " ";
            Debug.Log("Added ingredient. new list : " + s);
            
            //transform.DOPunchScale(Vector3.one * .5f, .2f,9,1.2f);
        } else transform.root.GetChild(0).DOShakePosition(.3f,50,16);

        return successful;
    }

    public bool TryCookSpell(out SpellData spell)
    {
        spell = null;
        if (ingredients.Count != 3) return false;
        //if (sauce == null) return false;

        spell = Crafting.CraftNewSpell(ingredients.ToArray(),sauce == null ? _defaultSauce : sauce);
        GameManager.Instance.playerInventory.Spells.Add(spell);

        SFXManager.Instance.PlaySFXClip(Sounds.Sizzle);
        SFXManager.Instance.PlaySFXClip(Sounds.UiTwinkle);
        Visuals3D.PlayCookedDishAnim(spell);

        return true;
    }

    #endregion

}
