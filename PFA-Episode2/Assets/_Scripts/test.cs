using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txt;

    private void Start()
    {
        _txt.text = PlayerPrefs.GetInt("Nombre total de tirage sur la run").ToString();
    }
}
