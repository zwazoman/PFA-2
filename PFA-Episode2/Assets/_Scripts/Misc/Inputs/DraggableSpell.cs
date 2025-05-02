using Cysharp.Threading.Tasks;
using UnityEngine;

public class DraggableSpell : Draggable
{
    [HideInInspector] public SpellData spell;

    [HideInInspector] public SpellCaster spellCaster;

    WayPoint _currentPoint = null;

    public async UniTask BeginDrag()
    {
        if (isDragging)
        {
            spellCaster.entity.ClearWalkables();
            spellCaster.PreviewSpellRange(spell);
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
                spellCaster.PreviewSpellZone(spell, point);
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

        await spellCaster.TryCastSpell(spell, wayPoint);

        spellCaster.entity.ApplyWalkables();

        print(spellCaster.gameObject.name);
    }
}
