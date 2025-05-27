using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();
    [SerializeField] private List<Transform> _spellSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private GameObject test;
    private GetInfoItem _infoItemUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public async void SetupIngredient()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
        {
            _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
            test = Instantiate(_prefabItem, _ingredientSlot[i]); //Création 
            _infoItemUI = test.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = _ingredientChoose.sprite;
            await SpawnTween(test);
            //_infoItemUI.IngredientName.text = _ingredientChoose.name;
            //_infoItemUI.Effect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
        }
    }

    public async UniTask SetupSauce()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
        {
            _sauceChoose = GameManager.Instance.playerInventory.Sauces[i];
            GameObject go = Instantiate(_prefabItem, _sauceSlot[i]); //Création 
            _infoItemUI = go.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = _sauceChoose.sprite;
            await SpawnTween(go);
            //_infoItemUI.IngredientName.text = _sauceChoose.name;
            //_infoItemUI.SauceEffect.text = Serializer.GetIngredientEffectString(_sauceChoose);
        }
    }

    public async UniTask SetupSpell()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
        {
            SpellData SpellChoose = GameManager.Instance.playerInventory.Spells[i];
            GameObject go = Instantiate(_prefabItem, _spellSlot[i]); //Création 
            _infoItemUI = go.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = SpellChoose.Sprite;
            await SpawnTween(go);
            //_infoItemUI.IngredientName.text = SpellChoose.name;
            //_infoItemUI.Effect.text = Serializer.GetIngredientEffectString(SpellChoose);
        }
    }

    private async UniTask SpawnTween(GameObject go)
    {
        await go.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetEase(Ease.InBack);
    }
}
