using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net.NetworkInformation;

public class ChooseIngredient : MonoBehaviour
{
    [SerializeField] private List<IngredientBase> _listScriptableIngredient = new();
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
    [SerializeField] private List<ColorPanel> _listColor = new();

    private void Start()
    {
        ChooseRandomIngredient();
    }

    private void ChooseRandomIngredient()
    {
        for(int i = 0; i <= 2; i++)
        {
            List<IngredientBase> listIngredient = _listScriptableIngredient;
            int choice = UnityEngine.Random.Range(0, listIngredient.Count - i);
            listIngredient.RemoveAt(choice);
            print(choice);
            SetupInfo(choice, i);
        }
    }

    private void SetupInfo(int choice, int index)
    {
        _listIngredientUI[index].title.text = _listScriptableIngredient[choice].name; //Name
        _listIngredientUI[index].imageLogoRef.sprite = _listScriptableIngredient[choice].sprite; //Sprite

        if(_listScriptableIngredient[choice] is Sauce) //Sauce
        {
            _listIngredientUI[index].familly.text = "Sauce";
            //_listIngredientUI[index].rarityFrame 
            SetupColor(index, 4);
        }
        else //Ingrédient
        {
            Ingredient ing = (Ingredient)_listScriptableIngredient[choice];
            _listIngredientUI[index].familly.text = ing.Family.ToString();
            switch (ing.Family)
            {
                case IngredientsInfo.Family.Starchys:
                    SetupColor(index, 2);
                    print("lait");
                    break;
                case IngredientsInfo.Family.Vegetables:
                    SetupColor(index, 0);
                    print("Légume");
                    break;
                case IngredientsInfo.Family.Dairys:
                    SetupColor(index, 3);
                    print("Féculent");
                    break;
                case IngredientsInfo.Family.Meat:
                    SetupColor(index, 1);
                    print("viande");
                    break;
            }
        }
    }

    private void SetupColor(int index, int colorIndex)
    {
        _listIngredientUI[index].famillyPanelColorLight.color = _listColor[colorIndex].ColorLight;
        _listIngredientUI[index].famillyPanelColorDark.color = _listColor[colorIndex].ColorMid;
        _listIngredientUI[index].famillyButtonColor.color = _listColor[colorIndex].ColorDark;
    }


}
