using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

[RequireComponent(typeof(SpellCaster))]
public class PlayerEntity : Entity
{
    [HideInInspector] public List<WayPoint> walkables = new();

    [SerializeField] List<DraggableSpell> spellsUI = new();

    [SerializeField] EndButton _endTurnButton;

    SpellCaster _spellCaster;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _spellCaster);
    }

    protected override void Start()
    {
        base.Start();

        CombatManager.Instance.PlayerEntities.Add(this);
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        ApplyWalkables();

        await CheckPlayerInput();
        
        await EndTurn();
    }

    public override async UniTask EndTurn()
    {
        await base.EndTurn();

        _endTurnButton.Pressed = false;
    }

    public async UniTask CheckPlayerInput()
    {
        while (!_endTurnButton.Pressed)
        {
            foreach (DraggableSpell draggable in spellsUI)
            {
                await draggable.BeginDrag();
            }

            if (Input.GetMouseButtonDown(0) && Tools.CheckMouseRay(out WayPoint point))
            {
                await TryMoveTo(point);
            }

            await UniTask.Yield();
        }
    }
}
