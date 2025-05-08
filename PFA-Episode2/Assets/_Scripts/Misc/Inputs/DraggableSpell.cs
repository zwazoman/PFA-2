using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class DraggableSpell : Draggable
{
    [HideInInspector] public SpellData spell;

    [HideInInspector] public SpellCaster spellCaster;

    [SerializeField] PremadeSpell _premadeSpell;

    WayPoint _currentPoint = null;

    List<WayPoint> _rangePoints = new();

    protected override void Awake()
    {
        if (_premadeSpell != null)
            spell = _premadeSpell.SpellData;
    }

    public async UniTask BeginDrag()
    {
        if (isDragging)
        {
            spellCaster.entity.ClearWalkables();
            _rangePoints = spellCaster.PreviewSpellRange(spell);
            await DragAndDrop();
        }
    }

    public async UniTask DragAndDrop()
    {
        List<WayPoint> zonePoints = new();

        while (isDragging)
        {
            if (Tools.CheckMouseRay(out WayPoint point) && point != null && (_currentPoint == null || point != _currentPoint))
            {
                _currentPoint = point;

                spellCaster.StopSpellZonePreview(_rangePoints, ref zonePoints);
                spellCaster.PreviewSpellZone(spell, point, _rangePoints);
            }
            else if (point == null)
            {
                spellCaster.StopSpellZonePreview(_rangePoints, ref zonePoints);
                _currentPoint = null;
            }
            await UniTask.Yield();
        }

        WayPoint wayPoint = _currentPoint;
        Reset();

        await spellCaster.TryCastSpell(spell, wayPoint, _rangePoints, zonePoints);

        spellCaster.entity.ApplyWalkables();
    }
}
