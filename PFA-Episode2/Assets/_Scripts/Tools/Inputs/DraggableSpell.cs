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

    public async UniTask BeginDrag()
    {
        if (isDragging)
        {
            spellCaster.entity.ClearWalkables();
            spellCaster.PreviewSpellRange(_spell.SpellData);
            await DragAndDrop();
        }
    }

    public async UniTask DragAndDrop()
    {
        while (isDragging)
        {
            if (Tools.CheckMouseRay(out WayPoint point) && point != null && (_currentPoint == null || point != _currentPoint))
            {
                _currentPoint = point;

                spellCaster.StopSpellZonePreview();
                spellCaster.PreviewSpellZone(_spell.SpellData, point);
            }
            else if (point == null)
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

        print(spellCaster.gameObject.name);
    }
}
