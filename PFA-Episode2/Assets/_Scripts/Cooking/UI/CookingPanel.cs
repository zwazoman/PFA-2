using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class CookingPanel : AnimatedPanel
{
    [Header("Scene References")]
    [SerializeField] GridLayoutGroup _ingredientsParent;
    [SerializeField] Transform _saucesParent;
    [SerializeField] Transform _ustencilsParent;
    [SerializeField] CookingPot _pot;
    [SerializeField] DishInfoPanel _dishInfoPanel;

#if UNITY_EDITOR
    [Header("test")]
    [SerializeField] List<Ingredient> tests = new();
    [SerializeField] List<Sauce> testSauces = new();
    [SerializeField] bool useTestInventory = false;
#endif

    private void Start()
    {

#if UNITY_EDITOR
        if (!useTestInventory)
        {
            LoadPlayerInventory();
            return;
        }

        //@temp --
        Inventory inv = new();
        for(int i = 0; i < 12; i++)
        {
            inv.Ingredients.Add(tests.PickRandom());
        }

        for (int i = 0; i < 12; i++)
        {
            inv.Sauces.Add(testSauces.PickRandom());
        }


        LoadInventory(inv);
        //--
#else
            LoadPlayerInventory();
    
#endif

        
    }

    void OnShown()
    {
        Debug.Log(GarbageCollector.isIncremental);
        GarbageCollector.CollectIncremental(100);
        LoadPlayerInventory();//pas dingue
        
    }

    public void LoadPlayerInventory()
    {
        Debug.Log("about to load player inventory");
        LoadInventory(GameManager.Instance.playerInventory);
    }

    public void LoadInventory(Inventory inv)
    {
        Clear();

        Debug.Log("-- loading inventory --");
        Debug.Log("Sauce count : " + inv.Sauces.Count);
        Debug.Log("Ingredient count : " + inv.Ingredients.Count);
        Debug.Log("Spell count : " + inv.Spells.Count);

        //content height
        float height = _ingredientsParent.cellSize.y + _ingredientsParent.spacing.y;
        if (_ingredientsParent.TryGetComponent(out RectTransform IngredientsGrid))
        {
            IngredientsGrid.sizeDelta = new Vector2(IngredientsGrid.sizeDelta.x, height * ( (inv.Ingredients.Count) / 3));
        }
        if (_saucesParent.transform.parent.parent.TryGetComponent(out RectTransform SauceGrid))
        {
            SauceGrid.sizeDelta = new Vector2(SauceGrid.sizeDelta.x, height * ((inv.Ingredients.Count + inv.Sauces.Count) / 3 ));
        }
        if (_ingredientsParent.transform.parent.parent.TryGetComponent(out RectTransform ScrollbarContent))
        {
            ScrollbarContent.sizeDelta = new Vector2(ScrollbarContent.sizeDelta.x, height * (3 + (inv.Ingredients.Count + inv.Sauces.Count) / 3 * 2));
        }

        //spawn inventory slots prefabs
        GameObject ing = Resources.Load<GameObject>("IngredientSlot");
        
        //ingredients
        for (byte i = 0; i < inv.Ingredients.Count; ++i)
        {
            Instantiate(ing, _ingredientsParent.transform)
                .GetComponentInChildren<DraggableIngredientContainer>()
                .SetUp(inv.Ingredients[i]);
        }



        //sauces
        for (byte i = 0; i < inv.Sauces.Count; ++i)
        {
            Instantiate(ing, _saucesParent.transform)
                .GetComponentInChildren<DraggableIngredientContainer>()
                .SetUp(inv.Sauces[i]);
        }
        Debug.Log("----");
    }
    private void Clear()
    {
        //ingredients
        for (int i = 0; i < _ingredientsParent.transform.childCount; i++)
        {
            Destroy(_ingredientsParent.transform.GetChild(i).gameObject);
        }
    }

    public void tryToCookNewSpell()
    {

        bool result = _pot.TryCookSpell(out SpellData spell);
        
        switch (result)
        {
            case true:
                Hide();
                _dishInfoPanel.Show();
                _dishInfoPanel.Setup(spell);

                //@revoir
                //inventory.Add(spell)
                //GameManager.Instance.playerInventory.Spells.Add(spell);

                break;
            case false:
                transform.DOShakePosition(.35f,50,15);
                break;
        }
    }
}

