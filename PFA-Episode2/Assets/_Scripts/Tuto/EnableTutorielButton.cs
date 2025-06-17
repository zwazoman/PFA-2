using UnityEngine;

public class EnableTutorielButton : MonoBehaviour
{
    [SerializeField] private GameObject _tutoBtn;
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstLaunch") == 0) { _tutoBtn.SetActive(true); }
        else { _tutoBtn.SetActive(false); }
    }
}
