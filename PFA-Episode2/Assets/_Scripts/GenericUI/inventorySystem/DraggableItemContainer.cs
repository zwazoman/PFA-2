using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemContainer : Draggable
{
    [field: SerializeField]
    public Object item; //{get; protected set;}



    protected override void Awake()
    {
        base.Awake();
        
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void Reset()
    {
        base.Reset();
    }
}
