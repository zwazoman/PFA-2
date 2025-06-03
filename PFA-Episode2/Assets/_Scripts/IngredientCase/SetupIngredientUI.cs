using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupIngredientUI : MonoBehaviour
{
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
    public List<List<IngredientBase>> ListListIngredient = new();
    public SerializedDictionary<SauceEffectType, IngredientUISerialize> itemIconPerSauceEffect = new();
    public SerializedDictionary<IngredientEffectType, IngredientUISerialize> itemIconPerIngredientEffect = new();
    [SerializeField] private List<Button> _listButton = new();

    private bool _firstTime;

    public static SetupIngredientUI Instance;

    private void Awake() { Instance = this; }

    public void SetupInfo(IngredientBase IngredientBase, int index) //Attribue tout l'UI au élément
    {
        try
        {
            if (IngredientBase is Sauce sauce) { _listIngredientUI[index].Setup(IngredientBase, itemIconPerSauceEffect[sauce.effect].IconEffectSprite, IngredientBase.rarity, itemIconPerSauceEffect[sauce.effect].EffectTypeColor); }
            else if (IngredientBase is Ingredient ing) { _listIngredientUI[index].Setup(IngredientBase, itemIconPerIngredientEffect[ing.EffectType].IconEffectSprite, IngredientBase.rarity, itemIconPerIngredientEffect[ing.EffectType].EffectTypeColor); }
        }
        catch (Exception e){ Debug.LogException(e); }
    }

    public async void Next(int index)
    {
        foreach (Button btn in _listButton) { btn.interactable = false; }
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
            //foreach (Button btn in _listButton) { btn.interactable = true; }
        }
        else
        {
            foreach (Button btn in _listButton) { btn.interactable = false; }
            await TweenIngredientUI.Instance.Monte(TweenIngredientUI.Instance.PanelToTween[index]);
            await TweenIngredientUI.Instance.TweenUIDespawn();
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
    }
}
