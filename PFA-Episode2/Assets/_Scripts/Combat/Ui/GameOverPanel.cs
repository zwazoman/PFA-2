using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : AnimatedPanel
{
    public void GoBack()
    {
        Time.timeScale = 1.0f;
        SceneTransitionManager.Instance.LoadScence("Menu");
    }
}
