using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BreadGuy : MonoBehaviour
{
    [SerializeField] int HealAmount;
    [SerializeField] private GameObject _dialogue;
    [SerializeField] Animator _animator;

    bool healed;
    public bool StartDialogue;

    #region AnimationParameters
    string _isSpeaking = "IsSpeaking";
    string _give = "Give";

    #endregion
    private async void Start()
    {
       await MoveMouse.Instance.Moving();
       await StartHealing();
    }

    public async UniTask StartHealing()
    {
        if (!healed)
        {
            await _dialogue.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            StartDialogue = true;
            DialogueManager.Instance.GetRandomSequenceDialogue();
            healed = true;
            await _animator.PlayAnimationTrigger(_give);
            GameManager.Instance.playerInventory.playerHealth.health = Mathf.Clamp(GameManager.Instance.playerInventory.playerHealth.health += HealAmount, 0, GameManager.Instance.playerInventory.playerHealth.maxHealth);
        }
    }
}
