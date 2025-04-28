using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Jobs;

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableIngredient : Draggable
{
    Transform originalParent;
    int siblingIndex;

    Rigidbody2D rb;

    const string deathZone = "DeathZone";

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue( TryGetComponent(out rb),"Y'a pas de rigid body là gros fdp de mort");
        rb.simulated = false;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        siblingIndex = transform.GetSiblingIndex(); 

        transform.parent = transform.root;
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        List<RaycastResult> a = new();
        EventSystem.current.RaycastAll(eventData, a);
        Debug.Log(a[0].gameObject, this);

        ResetDrag();
    }

    public override void ResetDrag()
    {
        transform.parent = originalParent;
        transform.SetSiblingIndex(siblingIndex);
        base.ResetDrag();
    }
}
