using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;

    Vector3 originalPos;

    RectTransform rectTransform ;
    RectTransform RectTransform { get { if (rectTransform == null) TryGetComponent(out rectTransform); return rectTransform; } set => rectTransform = value; }

    protected virtual void Awake()
    {
        originalPos = RectTransform.anchoredPosition;
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
        RectTransform.anchoredPosition = originalPos;
    }
}
