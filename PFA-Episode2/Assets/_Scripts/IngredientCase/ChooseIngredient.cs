using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Net.NetworkInformation;

public class ChooseIngredient : MonoBehaviour
{
    [Header("Sauce")]

    [SerializeField] private List<Sauce> _listSauceCommon = new();
    [SerializeField] private List<Sauce> _listSauceSavoureux = new();
    [SerializeField] private List<Sauce> _listSauceDivin = new();

    [Header("Ingredient")]

    [SerializeField] private List<Ingredient> _listIngreidentCommon = new();
    [SerializeField] private List<Ingredient> _listIngredientSavoureux = new();
    [SerializeField] private List<Ingredient> _listIngredientDivin = new();

    [Header("Probability")]

    [SerializeField] [Range(0, 100)] private int _probaSauce;
    [SerializeField] [Range(0, 100)] private int _probaCommon;
    [SerializeField] [Range(0, 100)] private int _probaSavoureux;
    [SerializeField] [Range(0, 100)] private int _probaDivin;

    public List<IngredientBase> IngredientBaseChoose { get; private set; } = new();

    public static ChooseIngredient Instance;

    private void Awake() { Instance = this; }

    private void Start() { ChooseRandomIngredient(); }

    private void ChooseRandomIngredient()
    {
        int TempoProbaSavoureux = _probaSavoureux;
        int TempoProbaDivin = _probaDivin;
        int TempoProbaSauce = _probaSauce;

        for (int sacIndex = 0; sacIndex <= 2; sacIndex++)
        {
            for (int tagIngredient = 0; tagIngredient <= 2; tagIngredient++)
            {
                if (IsSauce())
                {
                    IngredientBaseChoose.Add(ReturnSauceChoose());
                }
                else
                {
                    IngredientBaseChoose.Add(ReturnIngredientChoose());
                }
            }
            //_probaSavoureux = TempoProbaSavoureux;
            //_probaDivin = TempoProbaDivin;
            _probaSauce = TempoProbaSauce;
        }
        int i = 0;
        foreach (IngredientBase ing in IngredientBaseChoose)
        {
            SetupIngredientUI.Instance.SetupInfo(ing, i);
            i++;
        }

    }

    /// <summary>
    /// Retourne un ingrédient au hasard selon les proba des ingrédients
    /// </summary>
    /// <returns>Ingrédient (scriptable Object)</returns>
    private Ingredient ReturnIngredientChoose()
    {
        int total = _probaCommon + _probaSavoureux + _probaDivin;
        int result = Random.Range(1, total + 1);

        print(result);
        if (result <= _probaDivin) //Divin
        {
            //_probaDivin = 0;
            Ingredient ing = _listIngredientDivin[Random.Range(0, _listIngredientDivin.Count - 1)];
            _listIngredientDivin.Remove(ing);
            return ing;
        }
        else if (result <= _probaDivin + _probaSavoureux)  //Savoureux
        {
            //_probaSavoureux = 0;
            Ingredient ing = _listIngredientSavoureux[Random.Range(0, _listIngredientSavoureux.Count - 1)];
            _listIngredientSavoureux.Remove(ing);
            return ing;
        }
        else //Common
        {
            Ingredient ing = _listIngreidentCommon[Random.Range(0, _listIngreidentCommon.Count - 1)];
            return ing;

        }
    }

    /// <summary>
    /// Retourne une sauce au hasard selon les proba des sauces
    /// </summary>
    /// <returns>Sauce (scriptable Object)</returns>
    private Sauce ReturnSauceChoose()
    {
        _probaSauce = 0;
        int total = _probaCommon + _probaSavoureux + _probaDivin;
        int result = Random.Range(1, total + 1);

        if (result <= _probaDivin && _listSauceDivin.Count != 0) //Divin
        {
            //_probaDivin = 0;
            Sauce sauce = _listSauceDivin[Random.Range(0, _listSauceDivin.Count - 1)];
            _listSauceDivin.Remove(sauce);
            return sauce;
        }
        else if (result <= _probaDivin + _probaSavoureux && _listSauceSavoureux.Count != 0)  //Savoureux
        {
            //_probaSavoureux = 0;
            Sauce sauce = _listSauceSavoureux[Random.Range(0, _listSauceSavoureux.Count - 1)];
            _listSauceSavoureux.Remove(sauce);
            return sauce;
        }
        else //Common
        {
            Sauce sauce = _listSauceCommon[Random.Range(0, _listSauceCommon.Count - 1)];
            return sauce;

        }
    }

    private bool IsSauce()
    {
        int numberChoose = Random.Range(0, 101);
        if (numberChoose > _probaSauce) { return false; }
        else { return true; } 
    }
}
