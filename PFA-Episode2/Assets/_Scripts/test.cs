using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txt;
    void Update()
    {
       _txt.text = Screen.currentResolution.refreshRateRatio.ToString();
    }
}
