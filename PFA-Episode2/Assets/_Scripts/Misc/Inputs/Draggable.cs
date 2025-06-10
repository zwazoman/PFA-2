
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    protected bool isDragging;

    RectTransform _rectTransform;
    RectTransform rectTransform { get { if (_rectTransform == null) TryGetComponent(out _rectTransform); return _rectTransform; } set => _rectTransform = value; }

    //original set up
    Vector3 originalPos;
    public Transform originalParent;
    int siblingIndex;
    CanvasGroup canvasGroup;
    
    public bool usePositionAboveFinger = true;

    protected Sounds dragSound = Sounds.DragDish;

    //notifiers
    public event Action EventBeginDrag, EventEndDrag,EventClicked,EventClickedSomewhereElse;

    private bool inspected = false;
    

    protected virtual void Awake()
    {
        TryGetComponent(out canvasGroup);
        originalPos = rectTransform.anchoredPosition;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        originalParent = transform.parent;

        siblingIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        transform.parent = transform.root;

        SFXManager.Instance.PlaySFXClip(dragSound);
        EventBeginDrag?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        transform.position = usePositionAboveFinger ? Tools.GetPositionAboveFinger() : Input.mousePosition;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        SFXManager.Instance.PlaySFXClip(dragSound);
        EventEndDrag?.Invoke();
    }

    public virtual void Reset()
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        transform.parent = originalParent;
        transform.SetSiblingIndex(siblingIndex);
        rectTransform.anchoredPosition = originalPos;
    }
    
    
    //click events
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!inspected)
        {
            inspected = true;
            EventClicked?.Invoke();
            _ = CheckForOtherClick();
        }
    }

    async UniTask CheckForOtherClick()
    {
        //Set up the new Pointer Event
        PointerEventData mPointerEventData = new PointerEventData(EventSystem.current);

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        do
        {
            await UniTask.Yield();
        } while (!(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended));

        inspected = false;
        EventClickedSomewhereElse?.Invoke();
    }
}
