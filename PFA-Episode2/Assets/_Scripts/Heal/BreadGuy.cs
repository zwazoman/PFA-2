using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreadGuy : MonoBehaviour
{
    [SerializeField] int HealAmount;

    [SerializeField] Animator _animator;

    bool healed = false;

    #region AnimationParameters
    string _isSpeaking = "IsSpeaking";
    string _give = "Give";

    #endregion

    private void Start()
    {
        //dialog ?
    }


    public async void StartHealing()
    {
        if (healed) return;
        healed = true;
        await _animator.PlayAnimationTrigger(_give);
        GameManager.Instance.playerInventory.playerHealth.health = Mathf.Clamp(GameManager.Instance.playerInventory.playerHealth.health += HealAmount, 0, GameManager.Instance.playerInventory.playerHealth.maxHealth);
        print(GameManager.Instance.playerInventory.playerHealth.health);

        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
