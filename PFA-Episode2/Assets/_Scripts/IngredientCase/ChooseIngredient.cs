using UnityEngine;
using System.Collections.Generic;

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
        List<IngredientBase> listIngredient = _listScriptableIngredient;
        for(int i = 0; i <= 2; i++) //Pour les interfaces
        {
            int choice = Random.Range(0, listIngredient.Count - i);
            listIngredient.RemoveAt(choice);
            SetupInfo(choice, i);
        }
    }

    private void SetupInfo(int choice, int index) //Attribue tout l'UI au élément
    {
        _listIngredientUI[index].title.text = _listScriptableIngredient[choice].name;                   //Name
        _listIngredientUI[index].imageLogoRef.sprite = _listScriptableIngredient[choice].sprite;        //Sprite

        if (_listScriptableIngredient[choice] is Sauce)                                                 //Sauce
        {
            Sauce ing = (Sauce)_listScriptableIngredient[choice];
            _listIngredientUI[index].effectDescription.text = Serializer.GetSauceEffectString(ing);

            _listIngredientUI[index].familly.text = "Sauce";
            _listIngredientUI[index].rarityFrame.sprite = _listScriptableIngredient[choice].frame;
            SetupColor(index, 4);
        }
        else                                                                                            //Ingrédient
        {
            Ingredient ing = (Ingredient)_listScriptableIngredient[choice];
            _listIngredientUI[index].effectDescription.text = Serializer.GetIngredientEffectString(ing);
            _listIngredientUI[index].familly.text = ing.Family.ToString();

            switch (ing.Family)
            {
                case IngredientsInfo.Family.Starchys:

                    SetupColor(index, 2);
                    _listIngredientUI[index].rarityFrame.sprite = _listScriptableIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Vegetables:

                    SetupColor(index, 0);
                    _listIngredientUI[index].rarityFrame.sprite = _listScriptableIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Dairys:

                    SetupColor(index, 3);
                    _listIngredientUI[index].rarityFrame.sprite = _listScriptableIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Meat:

                    SetupColor(index, 1);
                    _listIngredientUI[index].rarityFrame.sprite = _listScriptableIngredient[choice].frame;
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
