using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableSpell : Draggable
{
    [HideInInspector] public Spell spell;
    [HideInInspector] public SpellCaster spellCaster;

    WayPoint _currentPoint = null;
    List<WayPoint> _rangePoints = new();

    [Header("scene references")]
    [SerializeField] private Image image;

    public void SetUp(Spell spell,Entity player)
    {
        if(spell.spellData.Sprite != null)
            image.sprite = spell.spellData.Sprite;

        this.spell = spell;
        spell.OnCooled += EnableSpell;

        spellCaster = player.entitySpellCaster;
    }


    protected override void Awake()
    {
        base.Awake();
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

    void EnableSpell()
    {
        //activer le spell de merde
    }

    void DisableSpell()
    {
        //d�sacctiver le spell de merde
    }
}
