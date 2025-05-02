using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;

    Vector3 originalPos;

    RectTransform _rectTransform ;
    RectTransform rectTransform { get { if (_rectTransform == null) TryGetComponent(out _rectTransform); return _rectTransform; } set => _rectTransform = value; }

    protected virtual void Awake()
    {
        originalPos = rectTransform.anchoredPosition;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
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
        rectTransform.anchoredPosition = originalPos;
    }
}
