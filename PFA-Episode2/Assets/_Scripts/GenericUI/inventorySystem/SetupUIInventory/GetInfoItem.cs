using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetInfoItem : MonoBehaviour
{
    public Image Icon;
    [HideInInspector] public IngredientBase IngBase;

    [Header("Ingredient")]
    public GameObject PanelIng;
    public TextMeshProUGUI IngredientName;
    public TextMeshProUGUI IngredientEffect;

    [Header("Sauce")]
    public GameObject PanelSauce;
    public TextMeshProUGUI SauceName;
    public TextMeshProUGUI SauceEffect;
    public Image SauceAoE;

    [Header("Spell")]
    public TextMeshProUGUI SpellName;
    public TextMeshProUGUI SpellEffect;
    public Image SpellAoE;

    private List<GameObject> _listPanel = new();

    public void SetPanelOn()
    {
        if (IngBase is Sauce)
        {
            PanelSauce.SetActive(true);
        }
        else if (IngBase is Ingredient)
        {
            PanelIng.SetActive(true);
        }
    }

    public void SetPanelOff()
    {
        //?
    }
}
