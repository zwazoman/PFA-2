<<<<<<< Updated upstream
=======
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
>>>>>>> Stashed changes
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    protected bool isDragging;

    RectTransform _rectTransform ;
    RectTransform rectTransform { get { if (_rectTransform == null) TryGetComponent(out _rectTransform); return _rectTransform; } set => _rectTransform = value; }

    //original set up
    Vector3 originalPos;
    public Transform originalParent;
    int siblingIndex;
    CanvasGroup canvasGroup;

<<<<<<< Updated upstream
=======
    public bool usePositionAboveFinger = true;

    protected Sounds dragSound = Sounds.DragDish;

    //notifiers
    public event Action EventBeginDrag, EventEndDrag,EventClicked,EventClickedSomewhereElse;

    private bool inspected = false;
    
>>>>>>> Stashed changes
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
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        transform.position = Tools.GetPositionAboveFinger();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
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
        else inspected = false;
    }

    async UniTask CheckForOtherClick()
    {
        //Set up the new Pointer Event
        PointerEventData mPointerEventData = new PointerEventData(EventSystem.current);

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        while (!(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) )
        {
            await UniTask.Yield();
        }
        
        EventClickedSomewhereElse?.Invoke();
    }
}
