using System.Collections.Generic;
using UnityEngine;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();
    [SerializeField] private List<Transform> _spellSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private GetInfoItem _infoItemUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public void SetupIngredient()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
        {
            _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
            GameObject go = Instantiate(_prefabItem, _ingredientSlot[i]); //Création 
            _infoItemUI = go.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = _ingredientChoose.sprite;
            //_infoItemUI.IngredientName.text = _ingredientChoose.name;
            //_infoItemUI.Effect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
        }
    }

    public void SetupSauce()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
        {
            _sauceChoose = GameManager.Instance.playerInventory.Sauces[i];
            GameObject go = Instantiate(_prefabItem, _sauceSlot[i]); //Création 
            _infoItemUI = go.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = _sauceChoose.sprite;
            //_infoItemUI.IngredientName.text = _sauceChoose.name;
            //_infoItemUI.SauceEffect.text = Serializer.GetIngredientEffectString(_sauceChoose);
        }
    }

    public void SetupSpell()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Spells.Count; i++)
        {
            SpellData SpellChoose = GameManager.Instance.playerInventory.Spells[i];
            GameObject go = Instantiate(_prefabItem, _spellSlot[i]); //Création 
            _infoItemUI = go.GetComponent<GetInfoItem>();
            _infoItemUI.Icon.sprite = SpellChoose.Sprite;
            //_infoItemUI.IngredientName.text = SpellChoose.name;
            //_infoItemUI.Effect.text = Serializer.GetIngredientEffectString(SpellChoose);
        }
    }
}
