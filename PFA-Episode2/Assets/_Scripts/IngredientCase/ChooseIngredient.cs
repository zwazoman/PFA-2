using UnityEngine;
using System.Collections.Generic;

public class ChooseIngredient : MonoBehaviour
{
    [SerializeField] private List<IngredientBase> _listScriptableIngredient = new();
    [SerializeField] private List<IngredientUI> _listIngredientUI = new();

    private void Start()
    {
        ChooseRandomIngredient();
    }

    private void ChooseRandomIngredient()
    {
        List <IngredientBase> listIngredient = _listScriptableIngredient;
        int choice = Random.Range(0, listIngredient.Count - 1);
        listIngredient.RemoveAt(choice);
    }

    private void SetupInfo()
    {

    }
}
