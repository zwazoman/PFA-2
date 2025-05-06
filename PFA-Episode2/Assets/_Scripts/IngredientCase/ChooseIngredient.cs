using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChooseIngredient : MonoBehaviour
{
    [SerializeField] private List<IngredientBase> _listScriptableIngredient = new();
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
    [SerializeField] private List<ColorPanel> _listColor = new();
    [SerializeField] private List<IngredientBase> _listIngredient = new();

    private void Start()
    {
        ChooseRandomIngredient();
    }

    private void ChooseRandomIngredient()
    {
        _listIngredient = _listScriptableIngredient;
        for(int i = 0; i <= 2; i++) //Pour les interfaces
        {
            int choice = Random.Range(0, _listIngredient.Count - i);
            _listIngredient.RemoveAt(choice);
            SetupInfo(choice, i);
        }
    }

    private void SetupInfo(int choice, int index) //Attribue tout l'UI au élément
    {
        _listIngredientUI[index].title.text = _listIngredient[choice].name;                   //Name
        _listIngredientUI[index].imageLogoRef.sprite = _listIngredient[choice].sprite;        //Sprite

        if (_listIngredient[choice] is Sauce)                                                 //Sauce
        {
            Sauce ing = (Sauce)_listIngredient[choice];
            _listIngredientUI[index].effectDescription.text = Serializer.GetSauceEffectString(ing);

            if (_listIngredientUI[index].familly != null) { _listIngredientUI[index].familly.text = "Sauce"; }
            _listIngredientUI[index].rarityFrame.sprite = _listIngredient[choice].frame;
            SetupColor(index, 4);
        }
        else                                                                                  //Ingrédient
        {
            Ingredient ing = (Ingredient)_listIngredient[choice];
            _listIngredientUI[index].effectDescription.text = Serializer.GetIngredientEffectString(ing);
            if (_listIngredientUI[index].familly != null) { _listIngredientUI[index].familly.text = ing.Family.ToString(); }

            switch (ing.Family)
            {
                case IngredientsInfo.Family.Starchys:

                    SetupColor(index, 2);
                    _listIngredientUI[index].rarityFrame.sprite = _listIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Vegetables:

                    SetupColor(index, 0);
                    _listIngredientUI[index].rarityFrame.sprite = _listIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Dairys:

                    SetupColor(index, 3);
                    _listIngredientUI[index].rarityFrame.sprite = _listIngredient[choice].frame;
                    break;
                case IngredientsInfo.Family.Meat:

                    SetupColor(index, 1);
                    _listIngredientUI[index].rarityFrame.sprite = _listIngredient[choice].frame;
                    break;
            }
        }
    }

    private void SetupColor(int index, int colorIndex)
    {
        _listIngredientUI[index].famillyPanelColorLight.color = _listColor[colorIndex].ColorLight;
        if (_listIngredientUI[index].famillyPanelColorMed != null) { _listIngredientUI[index].famillyPanelColorMed.color = _listColor[colorIndex].ColorMid; }
        foreach(Image img in _listIngredientUI[index].famillyPanelColorDark)
        {
            img.color = _listColor[colorIndex].ColorDark;
        }
    }
    public async void Next()
    {
        //ing
        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
