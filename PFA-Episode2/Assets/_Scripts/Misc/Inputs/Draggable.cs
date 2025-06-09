using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    protected Sounds dragSound = Sounds.ButtonPress;

    //notifiers
    public event Action EventBeginDrag, EventEndDrag;

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
}
