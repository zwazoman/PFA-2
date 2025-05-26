using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetInfoItem : MonoBehaviour
{
    public Image Icon;

    [Header ("Ingredient")]
    public TextMeshProUGUI IngredientName;
    public TextMeshProUGUI IngredientEffect;

    [Header("Sauce")]
    public TextMeshProUGUI SauceName;
    public TextMeshProUGUI SauceEffect;
    public Image SauceAoE;

    [Header("Spell")]
    public TextMeshProUGUI SpellName;
    public TextMeshProUGUI SpellEffect;
    public Image SpellAoE;
}
