using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject OptionPanel;
    public GameObject MainPanel;
    public Slider soundSlider;
    public Slider grappleSensitivitySlider;

    
    public void PauseGame()
    {
        Time.timeScale = 0;
        PausePanel.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
    }


    public void ShowOptions()
    {
        OptionPanel.SetActive(true);
    }

    public void HideOptions()
    {
        OptionPanel.SetActive(false);
    }

}
