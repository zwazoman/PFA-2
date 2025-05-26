using System;
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
        try
        {
            _listIngredientUI[index].Setup(IngredientBase);
        }
        catch (Exception e){ Debug.LogException(e); }
    }

    public async void Next(int index)
    {
        foreach (IngredientBase ing in ListListIngredient[index])
        {
            if (ing is Sauce Sauce) { GameManager.Instance.playerInventory.Sauces.Add(Sauce); }
            else if (ing is Ingredient Ingredient) { GameManager.Instance.playerInventory.Ingredients.Add(Ingredient); }
        }
        if (PlayerMap.Instance.PositionMap == 1 && !_firstTime)
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
