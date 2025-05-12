using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Threading.Tasks;


public class ChooseIngredient : MonoBehaviour
{
    [Header("Sauce")]

    [SerializeField] private List<Sauce> _listSauceCommon = new();
    [SerializeField] private List<Sauce> _listSauceSavoureux = new();
    [SerializeField] private List<Sauce> _listSauceDivin = new();

    [Header("Ingredient")]

    [SerializeField] private List<Ingredient> _listIngredientCommon = new();
    [SerializeField] private List<Ingredient> _listIngredientSavoureux = new();
    [SerializeField] private List<Ingredient> _listIngredientDivin = new();

    [Header("Probability")]

    [SerializeField][Range(0, 100)] private int _probaSauce;
    [SerializeField][Range(0, 100)] private int _probaCommon;
    [SerializeField][Range(0, 100)] private int _probaSavoureux;
    [SerializeField][Range(0, 100)] private int _probaDivin;

    public List<IngredientBase> IngredientBaseChooseBySac { get; private set; } = new();
    private List<IngredientBase> _completeListIngredientChoose = new();

    public static ChooseIngredient Instance;

    private void Awake() { Instance = this; }

    private void Start()
    {
        ChooseRandomIngredient();
    }

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
                    IngredientBaseChooseBySac.Add(ReturnSauceChoose());
                }
                else
                {
                    IngredientBaseChooseBySac.Add(ReturnIngredientChoose());
                }
            }
            foreach (IngredientBase ing in IngredientBaseChooseBySac) { _completeListIngredientChoose.Add(ing); }
            List<IngredientBase> tempo = new();
            tempo.AddRange(IngredientBaseChooseBySac);
            SetupIngredientUI.Instance.ListListIngredient.Add(tempo);
            IngredientBaseChooseBySac.Clear();

            //_probaSavoureux = TempoProbaSavoureux;
            //_probaDivin = TempoProbaDivin;
            _probaSauce = TempoProbaSauce;
        }
        for (int i = 0; i != _completeListIngredientChoose.Count; i++)
        {
            SetupIngredientUI.Instance.SetupInfo(_completeListIngredientChoose[i], i);
        }

    }

    /// <summary>
    /// Retourne un ingrédient au hasard selon les proba des ingrédients
    /// </summary>
    /// <returns>Ingrédient (scriptable Object)</returns>
    private Ingredient ReturnIngredientChoose()
    {
        List<Ingredient> CommonIng = _listIngredientCommon;
        List<Ingredient> SavoureuxIng = _listIngredientSavoureux;
        List<Ingredient> DivinIng = _listIngredientDivin;

        int total = _probaCommon + _probaSavoureux + _probaDivin;
        int result = Random.Range(1, total + 1);

        if (result <= _probaDivin && DivinIng.Count != 0) //Divin
        {
            //_probaDivin = 0;
            Ingredient ing = DivinIng[Random.Range(0, DivinIng.Count)];
            DivinIng.Remove(ing);
            return ing;
        }
        else if (result <= _probaDivin + _probaSavoureux && SavoureuxIng.Count != 0) //Savoureux
        {
            //_probaSavoureux = 0;
            Ingredient ing = SavoureuxIng[Random.Range(0, SavoureuxIng.Count)];
            SavoureuxIng.Remove(ing);
            return ing;
        }
        else //Common
        {
            Ingredient ing = CommonIng[Random.Range(0, CommonIng.Count)];
            return ing;
        }
    }

    /// <summary>
    /// Retourne une sauce au hasard selon les proba des sauces
    /// </summary>
    /// <returns>Sauce (scriptable Object)</returns>
    private Sauce ReturnSauceChoose()
    {
        List<Sauce> CommonSauce = _listSauceCommon;
        List<Sauce> SavoureuxSauce = _listSauceSavoureux;
        List<Sauce> DivinSauce = _listSauceDivin;

        _probaSauce = 0;
        int total = _probaCommon + _probaSavoureux + _probaDivin;
        int result = Random.Range(1, total + 1);

        if (result <= _probaDivin && DivinSauce.Count != 0) //Divin
        {
            //_probaDivin = 0;
            Sauce sauce = DivinSauce[Random.Range(0, DivinSauce.Count - 1)];
            DivinSauce.Remove(sauce);
            return sauce;
        }
        else if (result <= _probaDivin + _probaSavoureux && SavoureuxSauce.Count != 0)  //Savoureux
        {
            //_probaSavoureux = 0;
            Sauce sauce = SavoureuxSauce[Random.Range(0, SavoureuxSauce.Count - 1)];
            SavoureuxSauce.Remove(sauce);
            return sauce;
        }
        else //Common
        {
            Sauce sauce = CommonSauce[Random.Range(0, CommonSauce.Count - 1)];
            return sauce;

        }
    }

    private bool IsSauce()
    {
        int numberChoose = Random.Range(0, 101);
        if (numberChoose > _probaSauce) { return false; }
        else { return true; }
    }

    public async Task ResetIngredient()
    {
        IngredientBaseChooseBySac.Clear();
        _completeListIngredientChoose.Clear();
        ChooseRandomIngredient();
        await TweenIngredientUI.Instance.TweenUISpawn();
    }
}
