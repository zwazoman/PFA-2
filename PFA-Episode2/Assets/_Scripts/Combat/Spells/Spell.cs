using System;
using Unity.VisualScripting;
using UnityEngine;

public class Spell
{
    public SpellData spellData;

    public event Action OnCooled;

    public bool canUse
    {
        get
        {
            return cooling == 0;
        }
    }

    bool _canUse;

    public int cooling = 0;

    public void StartCooldown()
    {
        cooling = spellData.CoolDown;
    }

    public void TickSpellCooldown(int value = 1)
    {
        cooling -= value;
        if (cooling < 0)
            cooling = 0;
        else if (cooling == 0)
            OnCooled?.Invoke();

    }
}
