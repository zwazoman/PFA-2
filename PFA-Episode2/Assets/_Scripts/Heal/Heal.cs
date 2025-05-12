using UnityEngine;

public class Heal : MonoBehaviour
{
   public async void Healing()
    {
        print("Tié soigné toi");
        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
