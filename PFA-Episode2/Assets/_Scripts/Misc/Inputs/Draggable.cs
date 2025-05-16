using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;

    RectTransform _rectTransform ;
    RectTransform rectTransform { get { if (_rectTransform == null) TryGetComponent(out _rectTransform); return _rectTransform; } set => _rectTransform = value; }

    //original set up
    Vector3 originalPos;
    Transform originalParent;
    int siblingIndex;
    CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        TryGetComponent(out canvasGroup);
        originalPos = rectTransform.anchoredPosition;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        originalParent = transform.parent;
        Debug.Log("Original parent : " + originalParent);
        siblingIndex = transform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        transform.parent = transform.root;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public virtual void Reset()
    {
        Debug.Log("Reset");
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        transform.parent = originalParent;
        transform.SetSiblingIndex(siblingIndex);
        rectTransform.anchoredPosition = originalPos;
    }
}
