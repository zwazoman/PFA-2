using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class AnimationExtension
{
    public static async UniTask PlayAnimationTrigger(this Animator animator, string trigger)
    {
        animator.SetTrigger(trigger);
        await Awaitable.WaitForSecondsAsync(GetAnimationLength(trigger,animator));
        animator.SetTrigger("Idle");
    }

    public static void PlayAnimationBool(this Animator animator, string boolos)
    {
        animator.SetBool(boolos, true);
    }

    public static void EndAnimationBool(this Animator animator, string boolos)
    {
        animator.SetBool(boolos, false);
    }

    public static float GetAnimationLength(string trigger, Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
}
