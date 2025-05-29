using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;

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

    [SerializeField][Range(0, 1)] private float _probaSauce;

    [SerializeField][Range(0, 1)] private float _probaCommonRef;
    [SerializeField][Range(0, 1)] private float _probaSavoureuxRef;
    [SerializeField][Range(0, 1)] private float _probaDivinRef;

    private float _probaCommon;
    private float _probaSavoureux;
    private float _probaDivin;

    [SerializeField][Range(-1, 1)] private float _biaisCommon;
    [SerializeField][Range(-1, 1)] private float _biaisSavoureux;
    [SerializeField][Range(-1, 1)] private float _biaisDivin;
    private int _moyenneGameTirage = 24;

    [Header("Others")]
    [SerializeField] private bool _sceneCombat;
    private int _maxIngredientRef;
    private bool _sauceChoose;
    public List<IngredientBase> IngredientBaseChooseBySac { get; private set; } = new();
    private List<IngredientBase> _completeListIngredientChoose = new();

    public static ChooseIngredient Instance;

    private void Awake() { Instance = this; }

    private void Start()
    {
        _probaCommon = _probaCommonRef;
        _probaSavoureux = _probaSavoureuxRef;
        _probaDivin = _probaDivinRef;

        ChooseRandomIngredient();
    }

    private void ChooseRandomIngredient()
    {
        float TempoProbaSauce = _probaSauce;

        for (int sacIndex = 0; sacIndex <= 2; sacIndex++)
        {
            if (_sceneCombat == true) { _maxIngredientRef = 0; }
            else { _maxIngredientRef = 2; }
            for (int tagIngredient = 0; tagIngredient <= _maxIngredientRef; tagIngredient++)
            {
                if (!_sauceChoose) { IsSauce(); }
                if (tagIngredient == 2 && _sauceChoose) { IngredientBaseChooseBySac.Add(ReturnSauceChoose()); }
                else { IngredientBaseChooseBySac.Add(ReturnIngredientChoose()); }
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

        float total = _probaCommon + _probaSavoureux + _probaDivin;
        float result = Random.Range(1, total + 1);
        GameManager.Instance.playerInventory.TotalTirageIngredient++;
        if (result <= _probaDivin && DivinIng.Count != 0) //Divin
        {
            //_probaDivin = 0;
            Ingredient ing = DivinIng[Random.Range(0, DivinIng.Count)];
            DivinIng.Remove(ing);
            SetupValueIngredient();
            return ing;
        }
        else if (result <= _probaDivin + _probaSavoureux && SavoureuxIng.Count != 0) //Savoureux
        {
            //_probaSavoureux = 0;
            Ingredient ing = SavoureuxIng[Random.Range(0, SavoureuxIng.Count)];
            SavoureuxIng.Remove(ing);
            SetupValueIngredient();
            return ing;
        }
        else //Common
        {
            Ingredient ing = CommonIng[Random.Range(0, CommonIng.Count)];
            SetupValueIngredient();
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
        float total = _probaCommon + _probaSavoureux + _probaDivin;
        float result = Random.Range(1, total + 1);
        GameManager.Instance.playerInventory.TotalTirageIngredient++;
        if (result <= _probaDivin && DivinSauce.Count != 0) //Divin
        {
            //_probaDivin = 0;
            Sauce sauce = DivinSauce[Random.Range(0, DivinSauce.Count)];
            DivinSauce.Remove(sauce);
            SetupValueIngredient();
            return sauce;
        }
        else if (result <= _probaDivin + _probaSavoureux && SavoureuxSauce.Count != 0)  //Savoureux
        {
            //_probaSavoureux = 0;
            Sauce sauce = SavoureuxSauce[Random.Range(0, SavoureuxSauce.Count)];
            SavoureuxSauce.Remove(sauce);
            SetupValueIngredient();
            return sauce;
        }
        else //Common
        {
            Sauce sauce = CommonSauce[Random.Range(0, CommonSauce.Count)];
            SetupValueIngredient();
            return sauce;
        }
    }
    private void IsSauce()
    {
        float numberChoose = Random.value;
        if (numberChoose <= _probaSauce) { _sauceChoose = true; }
    }
    public async Task ResetIngredient()
    {
        IngredientBaseChooseBySac.Clear();
        _completeListIngredientChoose.Clear();
        ChooseRandomIngredient();
        await TweenIngredientUI.Instance.TweenUISpawn();
    }
    private void SetupValueIngredient()
    {
        _probaCommon = FormuleRandom(_probaCommonRef, _biaisCommon, _moyenneGameTirage, GameManager.Instance.playerInventory.TotalTirageIngredient);
        _probaSavoureux = FormuleRandom(_probaSavoureuxRef, _biaisSavoureux, _moyenneGameTirage, GameManager.Instance.playerInventory.TotalTirageIngredient);
        _probaDivin = FormuleRandom(_probaDivinRef, _biaisDivin, _moyenneGameTirage, GameManager.Instance.playerInventory.TotalTirageIngredient);
    }
    private float FormuleRandom(float probaRef, float biais, int nombreTotalTirage, int tirageActuel)
    {
        float test = biais * (2 * (float)tirageActuel / (float)nombreTotalTirage - 1) + probaRef;
        return test;
    }
    //#if UNITY_EDITOR
    //    public void GenerateLists()
    //    {
    //        _listIngredientCommon.Clear();
    //        _listIngredientSavoureux.Clear();
    //        _listIngredientDivin.Clear();

    //        _listSauceCommon.Clear();
    //        _listSauceSavoureux.Clear();
    //        _listSauceDivin.Clear();

    //        string[] files = Directory.GetFiles("Assets/_Data/Ingredients/ingredients", "*.asset", SearchOption.TopDirectoryOnly);
    //        foreach (string path in files)
    //        {
    //            Ingredient asset = (Ingredient)AssetDatabase.LoadAssetAtPath(path, typeof(Ingredient));
    //            switch (asset.rarity)
    //            {
    //                case Rarity.Ordinaire:
    //                    _listIngredientCommon.Add(asset);
    //                    break;
    //                case Rarity.Savoureux:
    //                    _listIngredientSavoureux.Add(asset);
    //                    break;
    //                case Rarity.Divin:
    //                    _listIngredientDivin.Add(asset);
    //                    break;
    //            }
    //        }

    //        files = Directory.GetFiles("Assets/_Data/Ingredients/Sauce", "*.asset", SearchOption.TopDirectoryOnly);
    //        foreach (string path in files)
    //        {
    //            Sauce asset = (Sauce)AssetDatabase.LoadAssetAtPath(path, typeof(Sauce));
    //            switch (asset.rarity)
    //            {
    //                case Rarity.Ordinaire:
    //                    _listSauceCommon.Add(asset);
    //                    break;
    //                case Rarity.Savoureux:
    //                    _listSauceSavoureux.Add(asset);
    //                    break;
    //                case Rarity.Divin:
    //                    _listSauceDivin.Add(asset);
    //                    break;
    //            }
    //        }

    //    }
    //#endif
    //}

    //#if UNITY_EDITOR
    //[CustomEditor(typeof(ChooseIngredient))]
    //class ChooseIngredientEditor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        if (GUILayout.Button("générer les listes connard"))
    //        {
    //            ((ChooseIngredient)target).GenerateLists();
    //        }
    //    }
    //}
    //#endif
}
