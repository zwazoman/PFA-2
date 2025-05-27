using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();
    [SerializeField] private List<Transform> _spellSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private List<GameObject> _ingredientItemList = new();
    private List<GameObject> _sauceItemList = new();
    private List<GameObject> _spellItemList = new();

    public List<GameObject> PanelToDisable = new();

    private GetInfoItem _infoItemUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public static SetupWorldMapInventory Instance;
    private void Awake() { Instance = this; }

    public async void SetupIngredient()
    {
        List<Ingredient> IngList = new();
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_ingredientItemList.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
            {
                _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];

                GameObject go = Instantiate(_prefabItem, _ingredientSlot[i]); //Création 
                _infoItemUI.Icon.sprite = _ingredientChoose.sprite;
                _infoItemUI.IngredientName.text = _ingredientChoose.name;
                _infoItemUI.IngredientEffect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
                _infoItemUI.IngBase = _ingredientChoose;
                go.transform.GetChild(0).parent = gameObject.transform;
                _ingredientItemList.Add(go);
                IngList.Add(_ingredientChoose);
                await SpawnTween(go);

            }
        }
    }

    public async void SetupSauce()
    {
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_sauceItemList.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
            {
                _sauceChoose = GameManager.Instance.playerInventory.Sauces[i];
                GameObject go = Instantiate(_prefabItem, _sauceSlot[i]); //Création 
                _infoItemUI = go.GetComponent<GetInfoItem>();
                _infoItemUI.Icon.sprite = _sauceChoose.sprite;
                _infoItemUI.SauceName.text = _sauceChoose.name;
                _infoItemUI.SauceEffect.text = Serializer.GetSauceEffectString(_sauceChoose);
                _infoItemUI.SauceAoE.sprite = _sauceChoose.areaOfEffect.sprite;
                _infoItemUI.IngBase = _sauceChoose;
                go.transform.GetChild(1).parent = gameObject.transform;
                _sauceItemList.Add(go);
                await SpawnTween(go);
            }
        }
    }

    public async void SetupSpell()
    {
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_spellItemList.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
            {
                SpellData SpellChoose = GameManager.Instance.playerInventory.Spells[i];
                GameObject go = Instantiate(_prefabItem, _spellSlot[i]); //Création 
                _infoItemUI = go.GetComponent<GetInfoItem>();
                _infoItemUI.Icon.sprite = SpellChoose.Sprite;
                _spellItemList.Add(go);
                go.transform.GetChild(0).parent = gameObject.transform;
                await SpawnTween(go);
                //_infoItemUI.IngredientName.text = SpellChoose.name;
                //_infoItemUI.Effect.text = Serializer.GetIngredientEffectString(SpellChoose);
            }
        }
    }

    private async UniTask SpawnTween(GameObject go)
    {
        await go.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetEase(Ease.InOutBack);
    }
}
