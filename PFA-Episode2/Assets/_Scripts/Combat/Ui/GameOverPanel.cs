using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : AnimatedPanel
{
    public void OnShown()
    {
        Time.timeScale = 0;
    }
    public async void GoBack()
    {
        Time.timeScale = 1.0f;
        await SceneTransitionManager.Instance.GoToScene("MainMenu");
    }
}
