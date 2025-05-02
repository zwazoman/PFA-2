using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishInfoPanel : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private TMP_Text txt_cooldown;
    [SerializeField] private TMP_Text txt_range, txt_DishName;
    [SerializeField] private Image image_dishIcon;

    [SerializeField] private List<InfoHeader> spellEffects;

    const string cd = " turns", r = " tiles",t = " - ";
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
        txt_cooldown.text = spell.CoolDown.ToString() + cd;
        txt_range.text = Mathf.Max(0, spell.Range-SpellCaster.RangeRingThickness).ToString() + t + spell.Range.ToString() + r;
    }

    /*[SerializeField] PremadeSpell test;
    private void Start()
    {
        Setup(test.SpellData);
    }*/
}
