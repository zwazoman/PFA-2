using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfoPopup : MonoBehaviour
{
    public TextMeshProUGUI SpellName;
    public List<TextMeshProUGUI> Effect;
    public Image SpellZoneEffect;
    public TextMeshProUGUI Range;
    public TextMeshProUGUI Cooldown;

    [SerializeField]private RectTransform _rectTransform;
    
    private Vector2 _basePose;

    
    public void ClearParent()
    {
       
        _rectTransform.SetParent(transform.root);
    }

    public void AttachToTransform(Transform t)
    {
        _rectTransform.SetParent(t);
        _rectTransform.anchoredPosition =  _basePose;
    }
    
    public void Setup(SpellData spellData)
    {
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
        _basePose = _rectTransform.anchoredPosition;
    }
}
