using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupIngredientUI : MonoBehaviour
{
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
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
            _listIngredientUI[index].rarityFrame.sprite = GameManager.Instance.staticData.itemFramesPerRarity[IngredientBase.rarity];
            _listIngredientUI[index].famillyPanelColorLight.sprite = GameManager.Instance.staticData.framesPerRarity[IngredientBase.rarity];
            _listIngredientUI[index].SpriteZone.SetActive(true);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.sizeDelta = new Vector2(450, 100);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.localPosition = new Vector3(131, -81, _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.position.z);
            _listIngredientUI[index].SpriteZone.GetComponent<Image>().sprite = Sauce.areaOfEffect.sprite;
        }
        else                                                                                  //Ingrédient
        {
            Ingredient Ingredient = (Ingredient)IngredientBase;
            _listIngredientUI[index].effectDescription.text = Serializer.GetIngredientEffectString(Ingredient);
            if (_listIngredientUI[index].familly != null) { _listIngredientUI[index].familly.text = Ingredient.Family.ToString(); }
            _listIngredientUI[index].SpriteZone.SetActive(false);
            _listIngredientUI[index].famillyPanelColorLight.sprite = GameManager.Instance.staticData.framesPerRarity[IngredientBase.rarity];
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.sizeDelta = new Vector2(655, 100);
            _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.localPosition = new Vector3(234,-81, _listIngredientUI[index].famillyPanelColorDark[0].rectTransform.position.z);
            _listIngredientUI[index].rarityFrame.sprite = GameManager.Instance.staticData.itemFramesPerRarity[IngredientBase.rarity];
            if(Ingredient.Family == IngredientsInfo.Family.Dairys ) //Si laitier
            {
                _listIngredientUI[index].Porte.SetActive(true);
                _listIngredientUI[index].PorteTxt.text = Ingredient.RangeIncrease.ToString();
            }
        }
    }

    public async void Next(int index)
    {
        foreach (IngredientBase ing in ListListIngredient[index])
        {
            if (ing is Sauce Sauce) { GameManager.Instance.playerInventory.Sauces.Add(Sauce); }
            else if (ing is Ingredient Ingredient) { GameManager.Instance.playerInventory.Ingredients.Add(Ingredient); }
        }
        if (SaveMapGeneration.Instance.PositionMap == 0 && !_firstTime)
        {
            _firstTime = true;
            ListListIngredient.Clear();
            await TweenIngredientUI.Instance.Monte(TweenIngredientUI.Instance.PanelToTween[index]);
            await TweenIngredientUI.Instance.TweenUIDespawn();
            await ChooseIngredient.Instance.ResetIngredient();
        }
        else
        {
            await TweenIngredientUI.Instance.Monte(TweenIngredientUI.Instance.PanelToTween[index]);
            await TweenIngredientUI.Instance.TweenUIDespawn();
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
    }
}
