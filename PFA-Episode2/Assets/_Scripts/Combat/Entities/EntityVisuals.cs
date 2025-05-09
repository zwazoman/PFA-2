using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class EntityVisuals : MonoBehaviour
{
    Entity owner;

    [SerializeField] Transform VisualsRoot;

    private void Awake()
    {
        TryGetComponent(out owner);
    }

    private void Start()
    {
        owner.stats.healthFeedbackTasks.Add(OnHealthUpdated);

    }

    async UniTask OnHealthUpdated(float delta, float newValue)
    {
        if(delta < 0) //damage
        {
            await VisualsRoot.DOShakePosition(.5f, .2f).AsyncWaitForCompletion().AsUniTask();
        }
        else //heal
        {
            await VisualsRoot.DOPunchScale(Vector3.one*1.2f, .5f,5).AsyncWaitForCompletion().AsUniTask(); ;
        }
    }
}
