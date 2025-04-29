using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemContainer : Draggable
{
    [field: SerializeField]
    public Object item {get; private set;}

    Transform originalParent;
    int siblingIndex;

    CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out canvasGroup);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        originalParent = transform.parent;
        siblingIndex = transform.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;

        transform.parent = transform.root;
        
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
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        transform.parent = originalParent;
        transform.SetSiblingIndex(siblingIndex);
        base.Reset();
    }
}
