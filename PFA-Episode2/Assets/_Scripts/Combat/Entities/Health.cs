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

    public void ApplyHealth(float heal)
    {
        OnTakeHeal?.Invoke(heal);

        _currentHealth += heal;

        if (_currentHealth >= MaxHealth)
            _currentHealth = MaxHealth;
        else if (_currentHealth <= 0)
            //OnDie?.Invoke();
            print(gameObject.name + " is dead");
    }

    public void ApplyShield(float shield)
    {
        OnTakeShield?.Invoke(shield);

        ShieldAmount += shield;
    }
}
