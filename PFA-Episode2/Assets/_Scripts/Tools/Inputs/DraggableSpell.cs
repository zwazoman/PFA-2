using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class DraggableSpell : Draggable
{
    [SerializeField] PremadeSpell _spell;

    [SerializeField] public SpellCaster spellCaster;

    WayPoint _currentPoint = null;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (_spell == null) return;
        spellCaster.entity.CurrentPoint.ChangeTileColor(spellCaster.entity.CurrentPoint._walkableMaterial);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        spellCaster.entity.ClearWalkables();
        spellCaster.PreviewSpellRange(_spell.SpellData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        print("drag");
    }

    public async UniTask DragAndDrop()
    {
        while (isDragging)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit, Mathf.Infinity/*, LayerMask.GetMask("Waypoint")*/);

            if (hit.collider != null && Tools.CheckMouseRay(out WayPoint point) && (_currentPoint == null || point != _currentPoint))
            {
                _currentPoint = point;

                spellCaster.StopSpellZonePreview();
                spellCaster.PreviewSpellZone(_spell.SpellData, point);
            }
            else if(!hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint nopoint))
            {
                spellCaster.StopSpellZonePreview();
                _currentPoint = null;
            }
            await UniTask.Yield();
        }

        WayPoint wayPoint = _currentPoint;
        Reset();

        await spellCaster.TryCastSpell(_spell.SpellData, wayPoint);

        spellCaster.entity.ApplyWalkables();

    }

    public override async void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }
}
