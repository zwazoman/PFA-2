using UnityEngine;

public class StartGameCombat : MonoBehaviour
{
    public async void StartGame() { await CombatManager.Instance.StartGame(); }
}
