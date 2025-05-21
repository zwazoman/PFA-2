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
        await Awaitable.WaitForSecondsAsync(GetAnimationLength(trigger,animator));
        animator.SetTrigger("Idle");
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
        //float time = 0;
        //RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        //for (int i = 0; i < ac.animationClips.Length; i++) 
        //{
        //    Debug.Log(ac.animationClips[i].name);
        //    if (ac.animationClips[i].name == animator.GetCurrentAnimatorClipInfo(0).)
        //    {
        //        time = ac.animationClips[i].length;
        //        Debug.Log(time);
        //    }
        //}

        AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        Debug.Log(clipInfo.clip.name);

        return clipInfo.clip.length;
    }
}
