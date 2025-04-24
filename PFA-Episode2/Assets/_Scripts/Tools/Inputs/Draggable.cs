using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        print("start drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        print("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        print("EndDrag");
    }
}
