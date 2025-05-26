using System.Collections.Generic;
using UnityEngine;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabIngredient;
    [SerializeField] private GameObject _prefabSauce;

    private GetInfoIngredient _infoIngredientUI;
    private GetInfoSauce _infoSauceUI;
    private Ingredient _ingredientChoose;
    private Sauce _sauceChoose;

    public void SetupIngredient()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
        {
            _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
            GameObject go = Instantiate(_prefabIngredient, _ingredientSlot[i]); //Création 
            _infoIngredientUI = go.GetComponent<GetInfoIngredient>();
            _infoIngredientUI.IngredientIcon.sprite = _ingredientChoose.sprite;
            //_infoIngredientUI.IngredientName.text = _ingredientChoose.name;
            //_infoIngredientUI.Effect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
        }
    }

    public void SetupSauce()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
        {
            _sauceChoose = GameManager.Instance.playerInventory.Sauces[i];
            GameObject go = Instantiate(_prefabSauce, _sauceSlot[i]); //Création 
            _infoSauceUI = go.GetComponent<GetInfoSauce>();
            _infoSauceUI.SauceIcon.sprite = _sauceChoose.sprite;
            //_infoSauceUI.IngredientName.text = _sauceChoose.name;
            //_infoSauceUI.Effect.text = Serializer.GetIngredientEffectString(_sauceChoose);
        }
    }
}
