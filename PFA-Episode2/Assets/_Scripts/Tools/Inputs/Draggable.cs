using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;

    Vector3 originalPos;

    protected virtual void Awake()
    {
        originalPos = transform.position;
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
        transform.position = originalPos;
    }
}
