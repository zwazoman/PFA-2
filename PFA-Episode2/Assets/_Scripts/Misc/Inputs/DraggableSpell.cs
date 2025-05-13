using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraggableSpell : Draggable
{
    [HideInInspector] public Spell spell;
    [HideInInspector] public SpellCaster spellCaster;

    WayPoint _currentPoint = null;
    List<WayPoint> _rangePoints = new();

    [Header("scene references")]
    [SerializeField] private Image _spellImage;
    [SerializeField] private Image _cooldownImage;

    [SerializeField] private TMP_Text _cooldownText;

    bool _canUse = true;

    public void SetUp(Spell spell,Entity player)
    {
        if(spell.spellData.Sprite != null)
        { 
            _spellImage.sprite = _cooldownImage.sprite = spell.spellData.Sprite; 
        }

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
        if (isDragging && _canUse)
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

        if(await spellCaster.TryCastSpell(spell, wayPoint, _rangePoints, zonePoints))
            DisableSpell();

        spellCaster.entity.ApplyWalkables();
    }

    void EnableSpell()
    {
        _canUse = true;

        _spellImage.enabled = true;

        _cooldownImage.enabled = false;
        _cooldownText.enabled = false;
    }

    public void TickCooldownText()
    {
        _cooldownText.text = spell.cooling.ToString();
    }

    void DisableSpell()
    {
        _canUse = false;

        _cooldownImage.enabled = true;
        _cooldownText.enabled = true;
        TickCooldownText();

        _spellImage.enabled = false;
    }
}
