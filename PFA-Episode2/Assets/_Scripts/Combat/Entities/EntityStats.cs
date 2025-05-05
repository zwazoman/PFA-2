using System;
using UnityEngine;

public class EntityStats
{
    public event Action OnDie;

    public float maxHealth;
    public float shieldAmount;
    public int maxMovePoints;

    public int currentMovePoints;
    public float _currentHealth;

    public void ApplyDamage(float damage)
    {
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
        _currentHealth += heal;

        if (_currentHealth >= maxHealth)
            _currentHealth = maxHealth;
        else if (_currentHealth <= 0)
            OnDie?.Invoke();
    }

    public void ApplyShield(float shield)
    {
        shieldAmount += shield;

        if(shieldAmount < 0)
            shieldAmount = 0;
    }
}
