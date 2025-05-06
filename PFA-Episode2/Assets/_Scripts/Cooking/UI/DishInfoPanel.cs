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
    [SerializeField] private Image image_AOE;

    [SerializeField] private List<InfoHeader> spellEffects;

    
    public void Setup(SpellData spell)
    {
        //effects
        for(byte i = 0; i < 4; i++)
        {
            if(i < spell.Effects.Count)
            {
                spellEffects[i].gameObject.SetActive(true);
                spellEffects[i].UpdateVisual(spell.Effects[i], null); //@Revoir sprite
            }
            else
            spellEffects[i].gameObject.SetActive(false);
        }

        //dish icon
        image_dishIcon.sprite = spell.Sprite;
        txt_DishName.text = spell.Name;

        //area of effect
        image_AOE.sprite = spell.AreaOfEffect.sprite;

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
