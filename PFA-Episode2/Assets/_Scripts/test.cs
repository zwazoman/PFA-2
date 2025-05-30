using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txt;
    void Update()
    {
        Application.targetFrameRate = 120;
       _txt.text = Application.targetFrameRate.ToString();
    }
}
