using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DraggableSpell : Draggable
{
    [SerializeField] PremadeSpell _spell;

    [SerializeField] SpellCaster _spellCaster;

    WayPoint _currentPoint = null;

    protected override void Awake()
    {
        base.Awake();
        print(_spell);
    }

    private void Start()
    {
        if (_spell == null) return;
        _spellCaster.entity.CurrentPoint.ChangeTileColor(_spellCaster.entity.CurrentPoint._walkableMaterial);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        _spellCaster.PreviewSpellRange(_spell.SpellData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        print("drag");
    }

    private void Update()
    {
        if (isDragging)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit, Mathf.Infinity/*, LayerMask.GetMask("Waypoint")*/);

            if (hit.collider != null && hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint point) && (_currentPoint == null || point != _currentPoint))
            {
                print("change de current point");

                _currentPoint = point;

                _spellCaster.StopSpellZonePreview();
                _spellCaster.PreviewSpellZone(_spell.SpellData, point);
            }
            else if(!hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint nopoint))
            {
                _spellCaster.StopSpellZonePreview();
                _currentPoint = null;
            }
        }
    }

    public override async void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        WayPoint point = _currentPoint;
        ResetDrag();

        await _spellCaster.TryCastSpell(_spell.SpellData, point);
    }
}
