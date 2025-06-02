using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] TMP_Text _txt_title; //Titre
    [SerializeField] Image _img_Icon; //Sprite
    [SerializeField] Image _img_SauceArea; //Sprite
    [SerializeField] TMP_Text _txt_effect; //Titre
    [SerializeField] Image _img_IconEffect;
    [SerializeField] List<Image> _starsList = new();


    public void Setup(IngredientBase ing,Sprite frameSprite)
    {
        _txt_title.text = ing.name;           //Name
        _img_Icon.sprite = ing.sprite;        //Sprite

        if (ing is Sauce)                                                 //Sauce
        {
            Sauce s = (Sauce)ing;
            _txt_effect.text = Serializer.GetSauceEffectString(s);
            _img_SauceArea.sprite = s.areaOfEffect.sprite;
            _img_SauceArea.transform.parent.gameObject.SetActive(true);
        }
        else                                                                                  //Ingrédient
        {
            Ingredient Ingredient = (Ingredient)ing;
            _txt_effect.text = Serializer.GetIngredientEffectString(Ingredient);
            _img_SauceArea.transform.parent.gameObject.SetActive(false);

        }
    }
}
