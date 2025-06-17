using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks.Triggers;

public class SetupIngredientUI : MonoBehaviour
{
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();
    public List<List<IngredientBase>> ListListIngredient = new();
    public SerializedDictionary<SauceEffectType, IngredientUISerialize> itemIconPerSauceEffect = new();
    public SerializedDictionary<IngredientEffectType, IngredientUISerialize> itemIconPerIngredientEffect = new();
    [SerializeField] private List<Button> _listButton = new();
    [SerializeField] private GameObject _parent;
    [SerializeField] private TextMeshProUGUI _numberRollTxt;
    public int NumberRoll = 1;

    public static SetupIngredientUI Instance;

    private void Awake() { Instance = this; }

    public void SetupTxt()
    {
        _numberRollTxt.text = "x " + NumberRoll.ToString();
    }

    public void SetupInfo(IngredientBase IngredientBase, int index) //Attribue tout l'UI au �l�ment
    {
        try
        {
            if (IngredientBase is Sauce sauce) { _listIngredientUI[index].Setup(IngredientBase, itemIconPerSauceEffect[sauce.effect].IconEffectSprite, IngredientBase.rarity, itemIconPerSauceEffect[sauce.effect].EffectTypeColor); }
            else if (IngredientBase is Ingredient ing) { _listIngredientUI[index].Setup(IngredientBase, itemIconPerIngredientEffect[ing.EffectType].IconEffectSprite, IngredientBase.rarity, itemIconPerIngredientEffect[ing.EffectType].EffectTypeColor); }
        }
        catch (Exception ){  }
    }

    public async void Next(int index)
    {
        foreach (Button btn in _listButton) { btn.interactable = false; }
        foreach (IngredientBase ing in ListListIngredient[index])
        {
            GameManager.Instance.playerInventory.AddIngredient(ing);
        }
        if (NumberRoll > 1)
        {
            ListListIngredient.Clear();
            NumberRoll--;
            _numberRollTxt.text = "x " + NumberRoll.ToString();
            TweenCard();
            await TweenIngredientUI.Instance.Monte(TweenIngredientUI.Instance.PanelToTween[index]);
            await TweenIngredientUI.Instance.TweenUIDespawn();
            await ChooseIngredient.Instance.ResetIngredient();
            //foreach (Button btn in _listButton) { btn.interactable = true; }
        }
        else
        {
            foreach (Button btn in _listButton) { btn.interactable = false; }
            NumberRoll--;
            _numberRollTxt.text = "x " + NumberRoll.ToString();
            TweenCard();
            await TweenIngredientUI.Instance.Monte(TweenIngredientUI.Instance.PanelToTween[index]);
            await TweenIngredientUI.Instance.TweenUIDespawn();
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
    }

    public async UniTask TweenCard()
    {
        await _parent.transform.DOScale(new Vector3(0.6f,0.6f,0.6f), 0.2f).SetEase(Ease.OutCubic);
        await _parent.transform.DOScale(new Vector3(1, 1, 1), 0.2f).SetEase(Ease.InCubic);
    }
}
