using UnityEngine;

public class Heal : MonoBehaviour
{
   public async void Healing()
    {
        print("Ti� soign� toi");
        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
