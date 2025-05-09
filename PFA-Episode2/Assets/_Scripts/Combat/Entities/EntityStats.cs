using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EntityStats
{
    public Entity owner;

    /// <summary>
    /// float : Damage taken
    /// float : new HP
    /// </summary>
    public event Action<float, float> OnDamageTaken;

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

        await ApplyHealth(damage);

    }

    public async UniTask ApplyHealth(float heal)
    {
        currentHealth += heal;

        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
            await owner.Die();

    }

    public async UniTask ApplyShield(float shield)
    {
        shieldAmount += shield;

        if (shieldAmount < 0)
            shieldAmount = 0;
    }
}
