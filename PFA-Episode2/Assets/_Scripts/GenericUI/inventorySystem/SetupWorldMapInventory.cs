using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupWorldMapInventory : MonoBehaviour
{
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private GetInfoIngredient _infoIngredientUI;
    private Ingredient _ingredientChoose;
    
    public void SetupIngredient()
    {
        for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
        {
            _ingredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
            GameObject go = Instantiate(_prefabItem, _ingredientSlot[i]); //Création 
            _infoIngredientUI = go.GetComponent<GetInfoIngredient>();
            _infoIngredientUI.IngredientIcon.sprite = _ingredientChoose.sprite;
            //_infoIngredientUI.IngredientName.text = _ingredientChoose.name;
            //_infoIngredientUI.Effect.text = Serializer.GetIngredientEffectString(_ingredientChoose);
        }
    }
}
