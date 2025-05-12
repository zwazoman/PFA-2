using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupIngredientUI : MonoBehaviour
{
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
    [SerializeField] private List<ColorPanel> _listColor = new();
    public List<List<IngredientBase>> ListListIngredient = new();
    private bool _firstTime;

    public static SetupIngredientUI Instance;

    private void Awake() { Instance = this; }

    public void SetupInfo(IngredientBase IngredientBase, int index) //Attribue tout l'UI au élément
    {
        _listIngredientUI[index].title.text = IngredientBase.name;                   //Name
        _listIngredientUI[index].imageLogoRef.sprite = IngredientBase.sprite;        //Sprite

        if (IngredientBase is Sauce Sauce)                                                 //Sauce
        {
            _listIngredientUI[index].effectDescription.text = Serializer.GetSauceEffectString(Sauce);
            if (_listIngredientUI[index].familly != null) { _listIngredientUI[index].familly.text = "Sauce"; }
            _listIngredientUI[index].rarityFrame.sprite = IngredientBase.frame;
            _listIngredientUI[index].SpriteZone.SetActive(true);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.sizeDelta = new Vector2(450, 100);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.localPosition = new Vector3(131, -81, _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.position.z);
            SetupColor(index, 4);
        }
        else                                                                                  //Ingrédient
        {
            Ingredient Ingredient = (Ingredient)IngredientBase;
            _listIngredientUI[index].effectDescription.text = Serializer.GetIngredientEffectString(Ingredient);
            if (_listIngredientUI[index].familly != null) { _listIngredientUI[index].familly.text = Ingredient.Family.ToString(); }
            _listIngredientUI[index].SpriteZone.SetActive(false);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.sizeDelta = new Vector2(655, 100);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.localPosition = new Vector3(234,-81, _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.position.z);

            switch (Ingredient.Family)
            {
                case IngredientsInfo.Family.Starchys:

                    SetupColor(index, 2);
                    break;
                case IngredientsInfo.Family.Vegetables:

                    SetupColor(index, 0);
                    break;
                case IngredientsInfo.Family.Dairys:

                    SetupColor(index, 3);
                    break;
                case IngredientsInfo.Family.Meat:

                    SetupColor(index, 1);
                    break;
            }
            _listIngredientUI[index].rarityFrame.sprite = Ingredient.frame;
        }
    }

    /// <summary>
    /// Attribue la couleur selon la famille d'ingredient
    /// </summary>
    /// <param name="index"></param>
    /// <param name="colorIndex"></param>
    private void SetupColor(int index, int colorIndex)
    {
        _listIngredientUI[index].famillyPanelColorLight.color = _listColor[colorIndex].ColorLight;
        if (_listIngredientUI[index].famillyPanelColorMed != null) { _listIngredientUI[index].famillyPanelColorMed.color = _listColor[colorIndex].ColorMid; }
        foreach (Image img in _listIngredientUI[index].famillyPanelColorDark)
        {
            img.color = _listColor[colorIndex].ColorDark;
        }
    }

    public async void Next(int index)
    {
        foreach(IngredientBase ing in ListListIngredient[index])
        {
            if (ing is Sauce Sauce) { GameManager.Instance.playerInventory.Sauces.Add(Sauce); }
            else if (ing is Ingredient Ingredient) { GameManager.Instance.playerInventory.Ingredients.Add(Ingredient); }
        }
        if (SaveMapGeneration.Instance.PositionMap == 0 && !_firstTime)
        {
            _firstTime = true;
            ChooseIngredient.Instance.ResetIngredient();
        }
        else
        {
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
    }
}
