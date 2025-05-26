using UnityEngine;
using UnityEngine.UI;

public class InventoryMap : MonoBehaviour
{
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _desactiveColor;

    public void EnableColor(Image img) { img.color = _activeColor; img.transform.GetChild(1).gameObject.SetActive(true); }
    public void DisableColor(Image img) { img.color = _desactiveColor; img.transform.GetChild(1).gameObject.SetActive(false); }
}
