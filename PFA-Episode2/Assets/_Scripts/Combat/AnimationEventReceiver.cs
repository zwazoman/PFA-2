using System;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] Entity _entity;

    private void Awake()
    {
        try
        {
            if (_entity == null)
                _entity = GetComponentInParent<Entity>();
        }catch (Exception ex)
        {

        }
    }

    public void AttackEvent(/*string connard*/)
    {
        _entity.entitySpellCaster.attackEventCompleted = true;
    }

    public void SoundEvent(string soundName)
    {
        //try
        //{
        //    Sounds sound = (Sounds)(int.Parse(parameters.Split("_")[0]));
        //    Debug.Log("CallSound " + sound);

        //    SFXManager.Instance.PlaySFXClip(sound);

        //}
        //catch (Exception e)
        //{
        //    Debug.LogException(e);
        //}

        try
        {
            SFXManager.Instance.PlaySFXClip(soundName);
        }
        catch (Exception e) { Debug.LogException(e); }
    }
}
