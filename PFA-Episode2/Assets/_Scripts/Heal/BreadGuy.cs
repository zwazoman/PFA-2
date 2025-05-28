using Cysharp.Threading.Tasks;
using UnityEngine;

public class BreadGuy : MonoBehaviour
{
    [SerializeField] int HealAmount;

    [SerializeField] Animator _animator;

    bool healed = false;
    bool _activate = false;

    #region AnimationParameters
    string _isSpeaking = "IsSpeaking";
    string _give = "Give";

    #endregion

    private void Start()
    {
        DialogueManager.Instance.SearchDialogue(Random.Range(0, DialogueManager.Instance.TextData.DialogueData.Count));
    }


    public async void StartHealing()
    {
        if (!healed)
        {
            healed = true;
            await _animator.PlayAnimationTrigger(_give);
            GameManager.Instance.playerInventory.playerHealth.health = Mathf.Clamp(GameManager.Instance.playerInventory.playerHealth.health += HealAmount, 0, GameManager.Instance.playerInventory.playerHealth.maxHealth);
            print(GameManager.Instance.playerInventory.playerHealth.health);

            DialogueManager.Instance.SearchDialogue(Random.Range(0, DialogueManager.Instance.TextData.DialogueData.Count));
        }
        else
        {
            await SceneTransitionManager.Instance.GoToScene("WorldMap");
        }
    }
}
