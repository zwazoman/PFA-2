using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreadGuy : MonoBehaviour
{
    [SerializeField] int HealAmount;

    [SerializeField] Animator _animator;

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
        await _animator.PlayAnimationTrigger(_give);
        Mathf.Clamp(GameManager.Instance.playerInventory.playerHealth.health += HealAmount, 0, GameManager.Instance.playerInventory.playerHealth.maxHealth);
        _animator.PlayAnimationBool(_isSpeaking);
        await UniTask.Delay(2000);
        _animator.EndAnimationBool(_isSpeaking);

        await SceneTransitionManager.Instance.GoToScene("WorldMap");
    }
}
