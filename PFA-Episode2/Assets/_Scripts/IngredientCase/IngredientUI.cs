using AYellowpaper.SerializedCollections;
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
    [SerializeField] GameObject _spellZoneGO;

    public void Setup(IngredientBase ing, Sprite iconEffect, Rarity rarity)
    {
        _txt_title.text = ing.name;           //Name
        _img_Icon.sprite = ing.sprite;        //Sprite
        _img_IconEffect.sprite = iconEffect;
        switch (rarity)
        {
            case Rarity.Ordinaire:
                _starsList[0].gameObject.SetActive(true);
                _starsList[1].gameObject.SetActive(false);
                _starsList[2].gameObject.SetActive(false);
                break;

            case Rarity.Savoureux:
                _starsList[0].gameObject.SetActive(true);
                _starsList[1].gameObject.SetActive(true);
                _starsList[2].gameObject.SetActive(false);
                break;

            case Rarity.Divin:
                _starsList[0].gameObject.SetActive(true);
                _starsList[1].gameObject.SetActive(true);
                _starsList[2].gameObject.SetActive(true);
                break;
        }                    //Stars
        if (ing is Sauce s)                                                 //Sauce
        {
            s = (Sauce)ing;
            _txt_effect.text = Serializer.GetSauceEffectString(s);
            _img_SauceArea.sprite = s.areaOfEffect.sprite;
            _img_SauceArea.transform.parent.gameObject.SetActive(true);
        }
        else                                                                                  //Ingrédient
        {
            _spellZoneGO.SetActive(false);
            Ingredient Ingredient = (Ingredient)ing;
            _txt_effect.text = Serializer.GetIngredientEffectString(Ingredient);
            _img_SauceArea.transform.parent.gameObject.SetActive(false);
            _txt_effect.color = new Color(87, 122, 44, 48);
        }
    }

    public void SetupColor()
    {

    }
}
