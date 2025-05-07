using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EntityStats
{
    public event Action OnDie;

    public float maxHealth;
    public float shieldAmount;
    public int maxMovePoints;

    public int currentMovePoints;
    public float currentHealth;

    public void ApplyDamage(float damage)
    {
        Debug.Log("apply damage");

        if(shieldAmount > 0)
        {
            float damageRemain = Mathf.Abs(shieldAmount - damage);
            ApplyShield(damage);
            damage = damageRemain;
        }

        ApplyHealth(damage);
            
    }

    public void ApplyHealth(float heal)
    {
        currentHealth += heal;

        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
            OnDie?.Invoke();
    }

    public void ApplyShield(float shield)
    {
        shieldAmount += shield;

        if(shieldAmount < 0)
            shieldAmount = 0;
    }
}
