using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[Serializable]
public class EntityStats
{
    public Entity owner;

    /// <summary>
    /// float : health delta
    /// float : new HP
    /// </summary>
    public List<HealthUpdateFeeback> healthFeedbackTasks = new();
    
    /// <summary>
    /// float : 
    /// </summary>
    public event Action<float> ShieldUpdateFeeback;


    /// <summary>
    /// float : health delta
    /// float : new HP
    /// </summary>
    public event Action<float,float> OnHealthUpdated;

    public float maxHealth;
    public int maxMovePoints;

    public float shieldAmount;
    public int currentMovePoints;
    public float currentHealth;

    public void Setup(float maxHP,float startHP)
    {
        this.maxHealth = maxHP;
        currentHealth = startHP;

        ApplyHealth(0);
        owner.gameObject.GetComponent<EntityUI>().Setup(owner);
    }

    public async UniTask ApplyDamage(float damage)
    {

        float newShield = Mathf.Max(0, shieldAmount - damage);
        float tankedDamage = Mathf.Abs( newShield - shieldAmount);
        damage = Mathf.Max(damage - tankedDamage,0);

        await ApplyShield(- tankedDamage);
        await ApplyHealth(-damage);
    }

    public async UniTask ApplyHealth(float delta)
    {
        currentHealth += delta;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthUpdated?.Invoke(delta, currentHealth);

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

        ShieldUpdateFeeback?.Invoke(shieldAmount);
    }
}

public delegate UniTask HealthUpdateFeeback(float delta, float newHealth);
