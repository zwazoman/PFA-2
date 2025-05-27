using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

/// <summary>
/// Script qui setup l'UI dans l'inventaire du joueur (Ingredient et Sauce)
/// </summary>
public class SetupWorldMapInventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    [SerializeField] private List<Transform> _ingredientSlot = new();
    [SerializeField] private List<Transform> _sauceSlot = new();

    [Header("Prefab")]
    [SerializeField] private GameObject _prefabItem;

    private readonly Dictionary<Ingredient, GameObject> _ingredientItemDico = new();
    private readonly Dictionary<Sauce, GameObject> _sauceItemDico = new();

    public List<GameObject> PanelToDisable = new(); //Panel info des ingredients

    public static SetupWorldMapInventory Instance;
    private void Awake() { Instance = this; }

    public async void SetupIngredient()
    {
        if (_ingredientItemDico.Count != 0) { return; }

        int indexSpawn = 0;
        for (int i = 0; i < GameManager.Instance.playerInventory.Ingredients.Count; i++)
        {
            Ingredient IngredientChoose = GameManager.Instance.playerInventory.Ingredients[i];
            if (_ingredientItemDico.ContainsKey(IngredientChoose)) //Si j'ai deja cree un ingredient de ce type
            {
                GameObject go = GetValueFromKey(_ingredientItemDico,IngredientChoose);
                GetInfoItem InfoItemUI = go.GetComponent<GetInfoItem>();
                InfoItemUI.NumberItem++;
                InfoItemUI.IngredientDoubleTxt.text = "x " + InfoItemUI.NumberItem.ToString(); //Nombre de double
                indexSpawn--; //On evite de sauter une case dans l'inventaire
            }
            else
            {
                GameObject go = Instantiate(_prefabItem, _ingredientSlot[indexSpawn]); //Création 
                GetInfoItem InfoItemUI = go.GetComponent<GetInfoItem>();
                InfoItemUI.Icon.sprite = IngredientChoose.sprite;
                InfoItemUI.IngredientName.text = IngredientChoose.name;
                InfoItemUI.IngredientEffect.text = Serializer.GetIngredientEffectString(IngredientChoose); //effet
                InfoItemUI.IngBase = IngredientChoose;
                go.transform.GetChild(0).SetParent(gameObject.transform); //On place l'enfant
                go.transform.localScale = Vector3.zero;
                _ingredientItemDico.Add(IngredientChoose, go);
            }
            indexSpawn++;
        }
        List<GameObject> list = new();
        foreach (GameObject go in _ingredientItemDico.Values) { list.Add(go); }
        await SpawnTween(list);
    }

    public async void SetupSauce()
    {
        if (_sauceItemDico.Count != 0) { return; }

        int indexSpawn = 0;
        for (int i = 0; i < GameManager.Instance.playerInventory.Sauces.Count; i++)
        {
            Sauce SauceChoose = GameManager.Instance.playerInventory.Sauces[i];
            if (_sauceItemDico.ContainsKey(SauceChoose))
            {
                GameObject go = GetValueFromKey(_sauceItemDico,SauceChoose);
                GetInfoItem InfoItemUI = go.GetComponent<GetInfoItem>();
                InfoItemUI.NumberItem++;
                InfoItemUI.IngredientDoubleTxt.text = "x " + InfoItemUI.NumberItem.ToString();
                indexSpawn--;
            }
            else
            {
                GameObject go = Instantiate(_prefabItem, _sauceSlot[indexSpawn]); //Création 
                GetInfoItem InfoItemUI = go.GetComponent<GetInfoItem>();
                InfoItemUI.Icon.sprite = SauceChoose.sprite;
                InfoItemUI.SauceName.text = SauceChoose.name;
                InfoItemUI.SauceEffect.text = Serializer.GetSauceEffectString(SauceChoose);
                InfoItemUI.SauceAoE.sprite = SauceChoose.areaOfEffect.sprite;
                InfoItemUI.IngBase = SauceChoose;
                go.transform.GetChild(1).SetParent(gameObject.transform);
                go.transform.localScale = Vector3.zero; 
                _sauceItemDico.Add(SauceChoose, go);
            }
            indexSpawn++;
        }
        List<GameObject> list = new();
        foreach (GameObject go in _sauceItemDico.Values) { list.Add(go); }
        await SpawnTween(list);
    }

    private async UniTask SpawnTween(List<GameObject> list)
    {
        foreach (GameObject go in list) { await go.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetEase(Ease.InOutBack); }
    }

    #region Tools
    public void Clean() { foreach (GameObject obj in PanelToDisable) { obj.SetActive(false); } }

    private GameObject GetValueFromKey<T>(Dictionary<T, GameObject> dico, IngredientBase key) where T : IngredientBase
    {
        foreach (var kvp in dico)
        {
            if (kvp.Key == key)
                return kvp.Value;
        }
        return null;
    }
    #endregion
}
