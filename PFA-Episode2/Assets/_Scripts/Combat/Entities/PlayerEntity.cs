using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(SpellCaster))]
public class PlayerEntity : Entity
{
    [HideInInspector] public List<WayPoint> walkables = new();
    
    [HideInInspector] public List<DraggableSpell> spellsUI = new();

    [HideInInspector] public EndButton endTurnButton;

    protected override void Awake()
    {
        base.Awake();
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
        ShowSpellsUI();

        await CheckPlayerInput();
        
        await EndTurn();
    }

    public override async UniTask EndTurn()
    {
        await base.EndTurn();

        endTurnButton.Pressed = false;
    }

    public async UniTask CheckPlayerInput()
    {
        while (!endTurnButton.Pressed)
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

    void ShowSpellsUI()
    {
        CombatUiManager.Instance.playerSpellGroup.Show();
    }

    void HideSpellsUI()
    {
        CombatUiManager.Instance.playerSpellGroup.Hide();
    }

}
