using TMPro;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txt;
    void Update()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        _txt.text = Application.targetFrameRate.ToString();
    }
}
