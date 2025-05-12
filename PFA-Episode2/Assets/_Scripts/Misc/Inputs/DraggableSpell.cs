using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableSpell : Draggable
{
    [HideInInspector] public SpellData spell;
    [HideInInspector] public SpellCaster spellCaster;
    [SerializeField] PremadeSpell _premadeSpell;

    WayPoint _currentPoint = null;
    List<WayPoint> _rangePoints = new();

    [Header("scene references")]
    [SerializeField] private Image image;

    public void SetUp(SpellData spell,Entity player)
    {
        if(spell.Sprite != null)
            image.sprite = spell.Sprite;

        this.spell = spell;
        spellCaster = player.entitySpellCaster;
    }


    protected override void Awake()
    {
        base.Awake();

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
                //Debug.Log("null spell : " + spell == null);
                //Debug.Log("spell name : " + spell.Name);
                zonePoints = spellCaster.PreviewSpellZone(spell, point, _rangePoints);
            }
            else if (point == null)
            {
                spellCaster.StopSpellZonePreview(_rangePoints, ref zonePoints);
                _currentPoint = null;
            }
            await UniTask.Yield();
            Debug.Log("Dragging");
        }

        WayPoint wayPoint = _currentPoint;
        Reset();

        await spellCaster.TryCastSpell(spell, wayPoint, _rangePoints, zonePoints);

        spellCaster.entity.ApplyWalkables();
    }
}
