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
            return cooling <= 0;
        }
    }

    bool _canUse;

    public int cooling = 0;

    public bool isDamaging = true;

    public void StartCooldown()
    {
        cooling = spellData.CoolDown;
    }

    public void TickSpellCooldown(int value = 1)
    {
        cooling = Mathf.Max(0,cooling - value);
        Debug.Log("Cooled : " + spellData.Name + " " + cooling.ToString()) ;
        if (cooling == 0)
            //cooling = spellData.CoolDown;
            
            OnCooled?.Invoke();

    }
}
