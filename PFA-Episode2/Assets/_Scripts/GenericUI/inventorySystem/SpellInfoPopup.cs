using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfoPopup : MonoBehaviour
{
    public Image SpellIcon;
    public TextMeshProUGUI SpellName;
    public List<TextMeshProUGUI> Effect;
    public Image SpellZoneEffect;
    public Image SpellIconDisable;
    public TextMeshProUGUI Range;
    public TextMeshProUGUI Cooldown;

    public void Setup(SpellData spellData)
    {
        SpellIcon.sprite = spellData.Sprite; //Sprite
        SpellIconDisable.sprite = spellData.Sprite;
        Range.text = spellData.Range.ToString();
        Cooldown.text = spellData.CoolDown.ToString();

        SpellName.text = spellData.Name; //Nom
        for (int index = 0; index < spellData.Effects.Count; index++) //Effets
        {
            Effect[index].gameObject.transform.parent.gameObject.SetActive(true);
            SpellEffect spellEffect = spellData.Effects[index];
            Effect[index].text = Serializer.GetSpellEffectString(spellEffect);
        }

        SpellZoneEffect.sprite = spellData.AreaOfEffect.sprite; //Area
    }
}
