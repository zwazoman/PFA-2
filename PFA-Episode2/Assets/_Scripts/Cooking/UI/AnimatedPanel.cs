using DG.Tweening;
using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class AnimatedPanel : MonoBehaviour
{
    const float TransitionTime = .2f;

    [Header("Animated Panel")]
    [SerializeField] CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        TryGetComponent(out _canvasGroup);
    }

    public void Show()
    {
        _canvasGroup.DOFade(1, TransitionTime);
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, TransitionTime);
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

}
