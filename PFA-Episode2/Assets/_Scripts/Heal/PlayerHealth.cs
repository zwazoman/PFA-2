using System;
using UnityEngine;

public class PlayerHealth
{
    public int maxHealth;
    public int health;

    public PlayerHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }
}
