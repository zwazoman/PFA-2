using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] TMP_Text txt_title; //Titre
    [SerializeField] Image img_Icon; //Sprite
    [SerializeField] Image img_SauceArea; //Sprite

    //public Image frame; //Cadre

    [SerializeField] TMP_Text txt_effect; //Titre


    public void Setup(IngredientBase ing)
    {
        txt_title.text = ing.name;                   //Name
        img_Icon.sprite = ing.sprite;        //Sprite

        //frame.sprite = GameManager.Instance.staticData.itemFramesPerRarity[ing.rarity];

        if (ing is Sauce)                                                 //Sauce
        {
            Sauce s = (Sauce)ing;
            txt_title.text = Serializer.GetSauceEffectString(s);
            img_SauceArea.sprite = s.areaOfEffect.sprite;
            img_SauceArea.transform.parent.gameObject.SetActive(true);
        }
        else                                                                                  //Ingrédient
        {
            Ingredient Ingredient = (Ingredient)ing;
            txt_title.text = Serializer.GetIngredientEffectString(Ingredient);
            img_SauceArea.transform.parent.gameObject.SetActive(false);

        }
    }
}
