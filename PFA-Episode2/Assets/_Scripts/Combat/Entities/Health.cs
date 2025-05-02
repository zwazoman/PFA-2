using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action<float> OnTakeDamage;
    public event Action<float> OnTakeHeal;
    public event Action<float> OnTakeShield;

    public event Action OnDie;

    [HideInInspector] public float MaxHealth;
    [HideInInspector] public float ShieldAmount;

    float _currentHealth;

    public void ApplyDamage(float damage)
    {
        if(ShieldAmount > 0)
        {
            float damageRemain = Mathf.Abs(ShieldAmount - damage);
            ApplyShield(damage);
            damage = damageRemain;
        }

        ApplyHealth(damage);
            
    }

    public void ApplyHealth(float heal)
    {
        OnTakeHeal?.Invoke(heal);

        _currentHealth += heal;

        if (_currentHealth >= MaxHealth)
            _currentHealth = MaxHealth;
        else if (_currentHealth <= 0)
            OnDie?.Invoke();
            print(gameObject.name + " is dead");
    }

    public void ApplyShield(float shield)
    {
        OnTakeShield?.Invoke(shield);

        ShieldAmount += shield;

        if(ShieldAmount < 0)
            ShieldAmount = 0;
    }
}
