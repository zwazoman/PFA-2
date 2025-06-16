using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DamagePreviewPopup : AnimatedPanel
{

    [SerializeField] private TMP_Text _text;
    public void UpdateVisuals(int currentHealth, int currentShield, int shieldDiff, int healthDiff)
    {
        if (healthDiff != 0)
        {
            _text.text = $"{currentHealth}<color=#ff8080>({healthDiff})</color> <sprite name=heart>";
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
                _text.text +=$" {currentShield}<color=#ff8080>({shieldDiff}) </color><sprite name=shield> ";
            }
            else
            {
                _text.text +=$" {currentShield} <sprite name=shield> ";
            }
        }
        else if (shieldDiff != 0)
        {
            string color = shieldDiff<0? "#ff8080" : "#8080ff";
            _text.text +=$" {currentShield}<color=#ff8080>({shieldDiff}) </color><sprite name=shield> ";
        }

        

    }
}
