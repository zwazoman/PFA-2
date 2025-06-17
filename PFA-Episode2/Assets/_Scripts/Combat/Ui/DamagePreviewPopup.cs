using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePreviewPopup : MonoBehaviour
{

    [SerializeField] private TMP_Text _text;
    [SerializeField] CanvasGroup _canvasGroup;

    public void Show()
    {
        _canvasGroup.DOFade(1,.15f);
    }
    public void Hide()
    {
        _canvasGroup.DOFade(0,.15f);
    }
    
    public void UpdateVisuals(int currentHealth, int currentShield, int healthDiff, int shieldDiff )
    {
        if (healthDiff != 0)
        {
            _text.text = $"{currentHealth}<color=#ff8080>{healthDiff}</color> <sprite name=heart>";
        }
        else
        {
            _text.text = $"{currentHealth} <sprite name=heart>";
        }

        if (currentShield != 0 )
        {
            
            if (shieldDiff != 0)
            {
                string color = shieldDiff<0? "#ff8080" : "#8080ff";
                _text.text +=$"  {currentShield}<color=#ff8080>{shieldDiff} </color><sprite name=shield> ";
            }
            else
            {
                _text.text +=$"  {currentShield} <sprite name=shield> ";
            }
        }
        else if (shieldDiff != 0)
        {
            string color = shieldDiff<0? "#ff8080" : "#8080ff";
            _text.text += $" {currentShield}<color=#ff8080>"+(shieldDiff>0?"+":"")+$"{shieldDiff} </color><sprite name=shield> ";
        }

        

    }
}
