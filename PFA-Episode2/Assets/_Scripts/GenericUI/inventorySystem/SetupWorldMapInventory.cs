using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();
    [SerializeField] private List<Transform> _spellSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private Dictionary<Ingredient, GameObject> _ingredientItemDico = new();
    private Dictionary<Sauce, GameObject> _sauceItemDico = new();
    private Dictionary<SpellData, GameObject> _spellItemDico = new();

    public List<GameObject> PanelToDisable = new();

    private GetInfoItem _infoItemUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public static SetupWorldMapInventory Instance;
    private void Awake() { Instance = this; }

    public async void SetupIngredient()
    {
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_ingredientItemDico.Count == 0)
        {
            int indexSpawn = 0;
            for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
            {
                _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
                if(_ingredientItemDico.ContainsKey(_ingredientChoose))
                {
                    GameObject go = IngGetValueFromKey(_ingredientChoose);
                    _infoItemUI = go.GetComponent<GetInfoItem>();
                    _infoItemUI.NumberItem++;
                    _infoItemUI.IngredientDoubleTxt.text = "x " + _infoItemUI.NumberItem.ToString();
                    indexSpawn--;
                }
                else
                {
                    GameObject go = Instantiate(_prefabItem, _ingredientSlot[indexSpawn]); //Création 
                    _infoItemUI = go.GetComponent<GetInfoItem>();
                    _infoItemUI.Icon.sprite = _ingredientChoose.sprite;
                    _infoItemUI.IngredientName.text = _ingredientChoose.name;
                    _infoItemUI.IngredientEffect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
                    _infoItemUI.IngBase = _ingredientChoose;
                    go.transform.GetChild(0).parent = gameObject.transform;
                    _ingredientItemDico.Add(_ingredientChoose, go);
                    await SpawnTween(go);
                }
                indexSpawn++;
            }
        }
    }

    public async void SetupSauce()
    {
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_sauceItemDico.Count == 0)
        {
            int indexSpawn = 0;
            for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
            {
                _sauceChoose = GameManager.Instance.playerInventory.Sauces[i];
                if (_sauceItemDico.ContainsKey(_sauceChoose))
                {
                    GameObject go = SauceGetValueFromKey(_sauceChoose);
                    _infoItemUI = go.GetComponent<GetInfoItem>();
                    _infoItemUI.NumberItem++;
                    _infoItemUI.IngredientDoubleTxt.text = "x " + _infoItemUI.NumberItem.ToString();
                    indexSpawn--;
                }
                else
                {
                    GameObject go = Instantiate(_prefabItem, _sauceSlot[i]); //Création 
                    _infoItemUI = go.GetComponent<GetInfoItem>();
                    _infoItemUI.Icon.sprite = _sauceChoose.sprite;
                    _infoItemUI.SauceName.text = _sauceChoose.name;
                    _infoItemUI.SauceEffect.text = Serializer.GetSauceEffectString(_sauceChoose);
                    _infoItemUI.SauceAoE.sprite = _sauceChoose.areaOfEffect.sprite;
                    _infoItemUI.IngBase = _sauceChoose;
                    go.transform.GetChild(1).parent = gameObject.transform;
                    _sauceItemDico.Add(_sauceChoose, go);
                    await SpawnTween(go);
                }
                indexSpawn++;
            }
        }
    }

    public async void SetupSpell()
    {
        foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); }
        if (_spellItemDico.Count == 0)
        {
            for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
            {
                SpellData SpellChoose = GameManager.Instance.playerInventory.Spells[i];
                GameObject go = Instantiate(_prefabItem, _spellSlot[i]); //Création 
                _infoItemUI = go.GetComponent<GetInfoItem>();
                _infoItemUI.Icon.sprite = SpellChoose.Sprite;
                _spellItemDico.Add(SpellChoose, go);
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

    private GameObject IngGetValueFromKey(IngredientBase ing)
    {
        foreach (var kvp in _ingredientItemDico)
        {
            if (kvp.Key == ing)
                return kvp.Value;
        }
        return null;
    }

    private GameObject SauceGetValueFromKey(IngredientBase ing)
    {
        foreach (var kvp in _sauceItemDico)
        {
            if (kvp.Key == ing)
                return kvp.Value;
        }
        return null;
    }
}
