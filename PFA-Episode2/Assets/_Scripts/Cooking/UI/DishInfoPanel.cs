using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishInfoPanel : AnimatedPanel
{
    [Header("Scene References")]
    [SerializeField] private TMP_Text txt_cooldown;
    [SerializeField] private TMP_Text txt_range, txt_DishName;
    [SerializeField] private Image image_dishIcon;

    [SerializeField] private List<InfoHeader> spellEffects;

    
    public void Setup(SpellData spell)
    {
        //effects
        for(byte i = 0; i < 4; i++)
        {
            spellEffects[i].UpdateVisual(i < spell.Effects.Count ? spell.Effects[i] : null,null); //@Revoir sprite
        }

        //dish icon
        image_dishIcon.sprite = spell.Sprite;
        txt_DishName.text = spell.Name;

        //area of effect
        //mes couilles

        //turns and range
        txt_cooldown.text = Serializer.GetCoolDownString(spell.CoolDown);
        txt_range.text = Serializer.GetRangeString(spell.Range);
    }

    /*[SerializeField] PremadeSpell test;
    private void Start()
    {
        Setup(test.SpellData);
    }*/
}
