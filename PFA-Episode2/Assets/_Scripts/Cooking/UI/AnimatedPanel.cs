using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class AnimatedPanel : MonoBehaviour
{
    const float TransitionTime = .2f;

    const string MethodNameOnShown = "OnShown";
    const string MethodNameOnHidden = "OnHidden";

    [Header("Animated Panel")]
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] private float _scaleJumpOnShow = .2f;

    protected virtual void Awake()
    {
        TryGetComponent(out _canvasGroup);
    }

    public void Show()
    {
        _canvasGroup.DOFade(1, TransitionTime);
        transform.DOPunchScale(Vector3.one * _scaleJumpOnShow, TransitionTime*2);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        SendMessage(MethodNameOnShown);
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, TransitionTime);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        SendMessage(MethodNameOnHidden);
    }

}
