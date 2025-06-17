using DG.Tweening;
using TMPro;
using UnityEngine;

public class FeedbackNumber : MonoBehaviour
{
    [SerializeField]private TMP_Text txt;
    private const float amplitude = .5f;
    private const float duration = 1.5f;
    public void PopupNumber(float number)
    {
        gameObject.SetActive(true);
        txt.text = number.ToString();

        Vector3 basePose = transform.position;
        transform.position = transform.position-Vector3.up*.5f*amplitude;
        transform.DOMoveY(transform.position.y + amplitude , duration).onComplete = () => transform.position = basePose;
        
        Sequence s = DOTween.Sequence();
        txt.alpha = 0;
        s.Append(txt.DOFade(1, duration*.4f));
        s.Append(txt.DOFade(0, duration*.6f));
        
        
        
    }
    
}
