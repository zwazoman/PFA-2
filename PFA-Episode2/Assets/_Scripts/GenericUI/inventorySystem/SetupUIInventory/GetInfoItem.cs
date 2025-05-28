using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetInfoItem : MonoBehaviour
{
    public Image Icon;
    public int NumberItem = 1;
    public TextMeshProUGUI IngredientDoubleTxt;

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

    public void SetPanelOn()
    {
        bool isAlreadyActive = false;

        // Vérifie si le panel actuel est déjà actif
        if (IngBase is Sauce)
        {
            isAlreadyActive = PanelSauce.activeSelf;
        }
        else if (IngBase is Ingredient)
        {
            isAlreadyActive = PanelIng.activeSelf;
        }

        // Ferme tous les panels
        SetPanelOff();

        // Si ce panel était déjà actif, on ne le réactive pas
        if (isAlreadyActive)
            return;

        // Sinon on active ce panel
        if (IngBase is Sauce)
        {
            PanelSauce.SetActive(true);
            SetupWorldMapInventory.Instance.PanelToDisable.Add(PanelSauce);
        }
        else if (IngBase is Ingredient)
        {
            PanelIng.SetActive(true);
            SetupWorldMapInventory.Instance.PanelToDisable.Add(PanelIng);
        }
    }

    private void SetPanelOff()
    {
        foreach (GameObject obj in SetupWorldMapInventory.Instance.PanelToDisable)
        {
            obj.SetActive(false);
        }
        SetupWorldMapInventory.Instance.PanelToDisable.Clear(); // Vide la liste pour éviter les doublons
    }
}
