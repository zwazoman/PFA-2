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

    public List<GameObject> IngredientItemList = new();
    public List<GameObject> SauceItemSlot = new();
    public List<GameObject> SpellItemSlot = new();

    private GetInfoItem _infoItemUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public static SetupWorldMapInventory Instance;
    private void Awake() { Instance = this; }
    public async void SetupIngredient()
    {
        if (IngredientItemList.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
            {
                _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
                GameObject go = Instantiate(_prefabItem, _ingredientSlot[i]); //Création 
                _infoItemUI = go.GetComponent<GetInfoItem>();
                _infoItemUI.Icon.sprite = _ingredientChoose.sprite;
                _infoItemUI.IngredientName.text = _ingredientChoose.name;
                _infoItemUI.IngredientEffect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
                _infoItemUI.IngBase = _ingredientChoose;
                go.transform.GetChild(0).parent = gameObject.transform;
                IngredientItemList.Add(go);
                await SpawnTween(go);
            }
        }
    }

    public async void SetupSauce()
    {
        if (SauceItemSlot.Count == 0)
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
                go.transform.GetChild(0).parent = gameObject.transform;
                SauceItemSlot.Add(go);
                await SpawnTween(go);
            }
        }
    }

    public async void SetupSpell()
    {
        if (SpellItemSlot.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
            {
                SpellData SpellChoose = GameManager.Instance.playerInventory.Spells[i];
                GameObject go = Instantiate(_prefabItem, _spellSlot[i]); //Création 
                _infoItemUI = go.GetComponent<GetInfoItem>();
                _infoItemUI.Icon.sprite = SpellChoose.Sprite;
                SpellItemSlot.Add(go);
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
