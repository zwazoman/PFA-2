using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpellCaster))]
public class PlayerEntity : Entity
{
    [HideInInspector] public List<WayPoint> walkables = new();
    [HideInInspector] public EndButton endTurnButton;

    [SerializeField] public List<DraggableSpell> spellsUI = new();

    [SerializeField] int maxHealth;
    [SerializeField] int maxMovePoints;


    protected override void Awake()
    {
        base.Awake();

        //edit les valeurs du entityspell avec celles de l'inventaire

        stats.maxMovePoints = maxMovePoints;
    }

    protected override void Start()
    {
        base.Start();
        CombatManager.Instance.RegisterEntity(this);
        stats.Setup(maxHealth);

        foreach(DraggableSpell spell in spellsUI)
        {
            spells.Add(spell.spell);
        }
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        endTurnButton.Pressed = false;
        ApplyWalkables();
        ShowSpellsUI();

        await CheckPlayerInput();
        
        await EndTurn();
    }

    public override async UniTask EndTurn()
    {
        await base.EndTurn();

        endTurnButton.Pressed = false;
        HideSpellsUI();
    }

    public async UniTask CheckPlayerInput()
    {
        while (!endTurnButton.Pressed)
        {
            foreach (DraggableSpell draggable in spellsUI)
            {
                await draggable.BeginDrag();
            }

            if (Input.GetMouseButtonUp(0) && Tools.CheckMouseRay(out WayPoint point) && !EventSystem.current.IsPointerOverGameObject(0))
            {
                await TryMoveTo(point);
            }

            await UniTask.Yield();
        }
    }

    void ShowSpellsUI()
    {
        CombatUiManager.Instance.playerHUD.Show();

        foreach (DraggableSpell spell in spellsUI)
        {
            spell.TickCooldownText();
        }
    }

    void HideSpellsUI()
    {
        CombatUiManager.Instance.playerHUD.Hide();
    }

}
