using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EntityStats
{
    public Entity owner;

    /// <summary>
    /// float : health delta
    /// float : new HP
    /// </summary>
    public List<HealthUpdateFeeback> healthFeedbackTasks = new();

    public float maxHealth;
    public int maxMovePoints;

    public float shieldAmount;
    public int currentMovePoints;
    public float currentHealth;



    public async UniTask ApplyDamage(float damage)
    {
        Debug.Log("apply damage");

        if (shieldAmount > 0)
        {
            float damageRemain = Mathf.Abs(shieldAmount - damage);
            await ApplyShield(damage);
            damage = damageRemain;
        }

        await ApplyHealth(-damage);

    }

    public async UniTask ApplyHealth(float delta)
    {
        currentHealth += delta;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //feedbacks
        UniTask[] tasks = new UniTask[healthFeedbackTasks.Count];
        for (int i = 0; i < healthFeedbackTasks.Count; i++)
        {
            HealthUpdateFeeback task = healthFeedbackTasks[i];
            tasks[i] = task(delta, currentHealth);
        }
        await UniTask.WhenAll(tasks);

        //healthFeedbackTasks?.Invoke(delta, currentHealth);

        if (currentHealth <= 0)
        await owner.Die();

    }

    public async UniTask ApplyShield(float delta)
    {
        shieldAmount += delta;

        if (shieldAmount < 0)
            shieldAmount = 0;
    }
}

public delegate UniTask HealthUpdateFeeback(float delta, float newHealth);
