using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private TMP_Text _cooldownText;

    bool _canUse = true;

    public void SetUp(Spell spell,Entity player)
    {
        if(spell.spellData.Sprite != null)
        { 
            _spellImage.sprite = _cooldownImage.sprite = _cooldownOverlay.sprite = spell.spellData.Sprite; 
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
            spellCaster.castingEntity.ClearWalkables();
            _rangePoints = spellCaster.PreviewSpellRange(spell);
            await DragAndDrop();
        }
    }

    public async UniTask DragAndDrop()
    {
        SpellCastData castData = new();
        castData.zonePoints = new();

        while (isDragging)
        {
            //on dragging on new tile
            if (Tools.CheckMouseRay(out WayPoint point) && point != null && (_currentPoint == null || point != _currentPoint))
            {
                _currentPoint = point;

                spellCaster.StopSpellZonePreview(_rangePoints, ref castData);
                castData = spellCaster.PreviewSpellZone(spell, point, _rangePoints);
            }
            else if (point == null && (_currentPoint != null))//dragging outside of board
            {
                spellCaster.StopSpellZonePreview(_rangePoints, ref castData);
                _currentPoint = null;
            }
            await UniTask.Yield();
        }

        WayPoint wayPoint = _currentPoint;
        Reset();

        if(await spellCaster.TryCastSpell(spell, wayPoint, _rangePoints, castData))
            DisableSpell();

        spellCaster.castingEntity.ApplyWalkables();
    }

    void EnableSpell()
    {
        _canUse = true;
        enabled = true;

        _spellImage.enabled = true;

        _cooldownImage.gameObject.SetActive( false);
        //_cooldownText.enabled = false;
    }

    public void TickCooldownUI()
    {
        _cooldownOverlay.DOFillAmount((float)spell.cooling / (float)spell.spellData.CoolDown,0.2f).SetEase(Ease.OutBack);
        _cooldownText.text = (spell.spellData.CoolDown-spell.cooling).ToString() + "/" + spell.spellData.CoolDown.ToString();
        //_cooldownText.text = spell.cooling.ToString();
    }

    void DisableSpell()
    {
        _canUse = false;
        enabled = false;

        _cooldownImage.gameObject.SetActive(true);
        //_cooldownText.enabled = true;
        TickCooldownUI();

        _spellImage.enabled = false;
    }
}
