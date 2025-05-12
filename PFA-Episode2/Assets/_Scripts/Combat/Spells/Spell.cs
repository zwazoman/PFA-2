using Unity.VisualScripting;
using UnityEngine;

public class Spell
{
    public SpellData spellData;

    public bool canUse
    {
        get
        {
            return cooling == 0;
        }
    }

    bool _canUse;

    int cooling = 0;

    public void StartCooldown()
    {
        cooling = spellData.CoolDown;
    }

    public void TickSpellCooldown(int value = 1)
    {
        cooling -= value;
    }
}
