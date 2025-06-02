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
    [SerializeField] private PlayerUIHandler _playerUIHandler;

    [SerializeField] int maxMovePoints;


    protected override void Awake()
    {
        base.Awake();

        stats.maxMovePoints = maxMovePoints;
        team = Team.Player;
    }

    protected override void Start()
    {
        base.Start();

        stats.Setup(GameManager.Instance.playerInventory.playerHealth.maxHealth,GameManager.Instance.playerInventory.playerHealth.health);
        
        CombatManager.Instance.RegisterEntity(this);

        _playerUIHandler.SetUp();

        foreach (DraggableSpell spell in spellsUI)
        {
            spells.Add(spell.spell);
        }

        CombatManager.Instance.OnWin += () => GameManager.Instance.playerInventory.playerHealth.health = Mathf.RoundToInt(stats.currentHealth);
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
                //await point.Lift(.6f, .5f,0);
                await TryMoveTo(point);
            }

            await UniTask.Yield();
        }
    }

    public void ShowSpellsUI()
    {
        CombatUiManager.Instance.playerHUD.Show();

        foreach (DraggableSpell spell in spellsUI)
        {
            spell.TickCooldownUI();
        }
    }

    public void HideSpellsUI()
    {
        CombatUiManager.Instance.playerHUD.Hide();
    }
}
