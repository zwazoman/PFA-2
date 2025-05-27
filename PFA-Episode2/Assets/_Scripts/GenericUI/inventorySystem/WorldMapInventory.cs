using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapInventory : MonoBehaviour
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _desactiveColor;
    [SerializeField] private List<GameObject> _goEnable = new (); //Btn & Inventory
    [SerializeField] private List<GameObject> _goDisable = new ();
    [SerializeField] private List<Image> _disableButton = new();

    public void EnablePanel()
    {
        foreach(GameObject go in _goEnable) { go.SetActive(true); }
        gameObject.GetComponent<Image>().color = _activeColor;
        DisablePanel();
    }

    private void DisablePanel()
    {
        foreach (GameObject go in _goDisable) { go.SetActive(false); }
        foreach (Image img in _disableButton) { img.color = _desactiveColor; }
    }
}
