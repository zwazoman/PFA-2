using Cysharp.Threading.Tasks;
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
    [SerializeField] private float _scaleJumpOnShow = 0f;

    //protected virtual void Awake()
    //{
    //    TryGetComponent(out _canvasGroup);
    //}

    public async void Show()
    {
        print("Show");
        _canvasGroup.gameObject.SetActive(true);
        print(_canvasGroup.gameObject.name);
        transform.DOPunchScale(Vector3.one * _scaleJumpOnShow, TransitionTime * 2, 5);
        await _canvasGroup.DOFade(1, TransitionTime).AsyncWaitForCompletion().AsUniTask();

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        SendMessage(MethodNameOnShown,SendMessageOptions.DontRequireReceiver);
        
    }

    public async void Hide()
    {
        await _canvasGroup.DOFade(0, TransitionTime).AsyncWaitForCompletion().AsUniTask();
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        SendMessage(MethodNameOnHidden, SendMessageOptions.DontRequireReceiver);
        _canvasGroup.gameObject.SetActive(false);

    }

}
