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
        //Debug.Log(GarbageCollector.isIncremental);
        GarbageCollector.CollectIncremental(100);
        LoadPlayerInventory();//pas dingue
    }

    public void LoadPlayerInventory()
    {
        //Debug.Log("about to load player inventory");
        LoadInventory(GameManager.Instance.playerInventory);
    }

    public void LoadInventory(Inventory inv)
    {
        Clear();

        // Debug.Log("-- loading inventory --");
        // Debug.Log("Sauce count : " + inv.Sauces.Count);
        // Debug.Log("Ingredient count : " + inv.Ingredients.Count);
        // Debug.Log("Spell count : " + inv.Spells.Count);

        //content height
        float height = _ingredientsParent.cellSize.y + _ingredientsParent.spacing.y;
        if (_ingredientsParent.TryGetComponent(out RectTransform IngredientsGrid))
        {
            IngredientsGrid.sizeDelta = new Vector2(IngredientsGrid.sizeDelta.x, height * Mathf.Ceil( (inv.Ingredients.Count) / 3f));
        }
        if (_saucesParent.transform.parent.parent.TryGetComponent(out RectTransform SauceGrid))
        {
            SauceGrid.sizeDelta = new Vector2(SauceGrid.sizeDelta.x, height * Mathf.Ceil((inv.Ingredients.Count + inv.Sauces.Count) / 3f ));
        }
        if (_ingredientsParent.transform.parent.parent.TryGetComponent(out RectTransform ScrollbarContent))
        {
            ScrollbarContent.sizeDelta = new Vector2(ScrollbarContent.sizeDelta.x, height * Mathf.Ceil(3 + (inv.Ingredients.Count + inv.Sauces.Count) / 3f * 2f));
        }

        //spawn inventory slots prefabs
        GameObject ing = Resources.Load<GameObject>("IngredientSlot");
        
        //ingredients
        for (byte i = 0; i < inv.Ingredients.Count; ++i)
        {
            DraggableIngredientContainer d = 
                Instantiate(ing, _ingredientsParent.transform)
                .GetComponentInChildren<DraggableIngredientContainer>();
            d.SetUp(inv.Ingredients[i]);
            d.EventBeginDrag+=_pot.Grow;
            d.EventEndDrag+=_pot.Shrink;
        }

        //sauces
        for (byte i = 0; i < inv.Sauces.Count; ++i)
        {
            DraggableIngredientContainer d = 
                Instantiate(ing, _saucesParent.transform)
                .GetComponentInChildren<DraggableIngredientContainer>();
            d.SetUp(inv.Sauces[i]);
            d.EventBeginDrag+=_pot.Grow;
            d.EventEndDrag+=_pot.Shrink;
        }
        //Debug.Log("----");
    }
    private void Clear()
    {
        //ingredients
        for (int i = 0; i < _ingredientsParent.transform.childCount; i++)
        {
            Destroy(_ingredientsParent.transform.GetChild(i).gameObject);
        }

        //sauces
        for (int i = 0; i < _saucesParent.transform.childCount; i++)
        {
            Destroy(_saucesParent.transform.GetChild(i).gameObject);
        }
    }

    public async void tryToCookNewSpell()
    {

        bool result = _pot.TryCookSpell(out SpellData spell);
        
        switch (result)
        {
            case true:
                
                Hide();
                
                await Awaitable.WaitForSecondsAsync(.2f);

                
                _dishInfoPanel.Show();
                _dishInfoPanel.Setup(spell);

                break;
            case false:
                transform.DOShakePosition(.35f,50,15);
                break;
        }
    }
}

