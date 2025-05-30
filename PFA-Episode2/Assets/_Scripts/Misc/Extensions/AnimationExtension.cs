using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class AnimationExtension
{
    public static async UniTask PlayAnimationTrigger(this Animator animator, string trigger)
    {
        if (animator == null)
            return;

        animator.SetTrigger(trigger);
        await UniTask.Delay(275);
        
        await Awaitable.WaitForSecondsAsync(Mathf.Max(0, GetAnimationLength(trigger,animator)- 0.25f)*.7f);
    }

    public static void PlayAnimationBool(this Animator animator, string boolos)
    {
        if (animator == null)
            return;

        animator.SetBool(boolos, true);
    }

    public static void EndAnimationBool(this Animator animator, string boolos)
    {
        if (animator == null)
            return;

        animator.SetBool(boolos, false);
    }

    public static float GetAnimationLength(string trigger, Animator animator)
    {
        AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

        return clipInfo.clip.length;
    }
}
