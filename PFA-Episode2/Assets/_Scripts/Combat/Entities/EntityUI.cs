using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class EntityUI : MonoBehaviour
{
    [Header("SceneReferences")]
    [SerializeField] CoolSlider lifebar;

    Entity owner;

    private void Awake()
    {
        TryGetComponent(out owner);
    }

    private void Start()
    {
        //owner.stats.healthFeedbackTasks += UpdateLifebar;
        owner.stats.healthFeedbackTasks.Add(UpdateLifebar);
        lifebar.MaxValue = owner.stats.maxHealth;
        lifebar.MinValue = 0;
    }

    private async UniTask UpdateLifebar(float delta, float newValue)
    {
        Debug.Log("delta : " + delta.ToString() + " , new value : " + newValue.ToString());
        lifebar.Value = newValue;
    }
}
