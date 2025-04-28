using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(SpellCaster))]
public class PlayerEntity : Entity
{
    [HideInInspector] public List<WayPoint> walkables = new();

    [SerializeField] Button _endTurnButton;

    List<DraggableSpell> spellsUI = new();

    SpellCaster _spellCaster;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _spellCaster);
        CombatManager.Instance.PlayerEntities.Add(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        await CheckPlayerInput();
    }

    public async UniTask CheckPlayerInput()
    {
        print("check player inpu");
        while (true /*bouton endTurn cliqué*/)
        {
            foreach (DraggableSpell draggable in spellsUI)
            {
                await draggable.DragAndDrop();
            }

            if (Input.GetMouseButtonDown(0) && Tools.CheckMouseRay(out WayPoint point))
            {
                print("point");
                await TryMoveTo(point);
            }

            await UniTask.Yield();
        }
    }


    public override async UniTask EndTurn()
    {
        await base.EndTurn();
    }
}
