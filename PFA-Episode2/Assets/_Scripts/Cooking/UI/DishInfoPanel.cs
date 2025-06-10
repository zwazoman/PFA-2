using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
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
    [SerializeField] private GameObject btn_keepCooking;
    [SerializeField] AudioSource _boillingSource;

    [SerializeField] private List<InfoHeader> spellEffects;

    public SerializedDictionary<SpellEffectType, IngredientUISerialize> itemIconPerSpellEffect = new();
    public Sprite noEffectSprite;
    public void Setup(SpellData spell)
    {
        _boillingSource.Pause();

        //effects
        for(byte i = 0; i < 4; i++)
        {
            if(i < spell.Effects.Count)
            {
                //spellEffects[i].gameObject.SetActive(true);
                spellEffects[i].UpdateVisual(spell.Effects[i], itemIconPerSpellEffect[spell.Effects[i].effectType]?.IconEffectSprite??noEffectSprite); 
            }
            else
                spellEffects[i].UpdateVisual("...",noEffectSprite); 
        }

        //dish icon
        image_dishIcon.sprite = spell.Sprite;
        txt_DishName.text = spell.Name;

        //area of effect
        image_AOE.sprite = spell.AreaOfEffect.sprite;

        //turns and range
        txt_cooldown.text = Serializer.GetCoolDownString(spell.CoolDown);
        txt_range.text = Serializer.GetRangeString(spell.Range);
        
        //keep cooking
        btn_keepCooking.SetActive(GameManager.Instance.playerInventory.Ingredients.Count>=3);
    }

    /*[SerializeField] PremadeSpell test;
    private void Start()
    {
        Setup(test.SpellData);
    }*/
}
