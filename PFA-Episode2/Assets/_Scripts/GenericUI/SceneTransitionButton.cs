using UnityEngine;

public class SceneTransitionButton : MonoBehaviour
{
    public async void ChangeScene(string sceneName)
    {
        await SceneTransitionManager.Instance.GoToScene(sceneName);
    }
}
