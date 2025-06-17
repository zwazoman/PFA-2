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

    protected async UniTask EntityBasePlayTurn()
    {
        await base.PlayTurn();
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

    public virtual async UniTask CheckPlayerInput()
    {
        CombatUiManager.Instance.StopButtonShake();
        
        bool canPlay = true;
        while (!endTurnButton.Pressed)
        {
            bool hasMoreActions = false;
            
            //si il joue un spell
            if(canPlay)
                foreach (DraggableSpell draggable in spellsUI)
                {
                    hasMoreActions |= draggable.canUse;
                    await draggable.CheckForBeginDrag();
                }

            //si il clique sur une case de mouvement
            hasMoreActions |= stats.currentMovePoints > 0;
            if (canPlay && Input.GetMouseButtonUp(0) && Tools.CheckMouseRay(out WayPoint point) && !EventSystem.current.IsPointerOverGameObject(0))
            {
                await TryMoveTo(point);
            }
            
            //quand le joueur n'a plus d'actions
            if (!hasMoreActions && canPlay)
            {
                canPlay = false;
                //jiggle end button when no more action is possible
                CombatUiManager.Instance.ShakeButton();
            }
            
            await UniTask.Yield();
        }
        
        CombatUiManager.Instance.StopButtonShake();

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
