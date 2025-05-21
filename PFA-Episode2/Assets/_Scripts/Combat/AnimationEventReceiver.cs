using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] Entity _entity;

    private void Awake()
    {
        if ( _entity == null )
            _entity = GetComponentInParent<Entity>();
    }

    public void Attack()
    {
        _entity.entitySpellCaster.attackEventCompleted = true;
    }
}
