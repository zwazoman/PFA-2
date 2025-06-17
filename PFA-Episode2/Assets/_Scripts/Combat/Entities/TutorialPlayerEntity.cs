using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialPlayerEntity : PlayerEntity
{
    private bool PlayedOnTurnWithTuto = false;

    public override async UniTask CheckPlayerInput()
    {
        if (!PlayedOnTurnWithTuto)
        {
            CombatUiManager.Instance.endButton.gameObject.SetActive(false);
            CombatUiManager.Instance.StopButtonShake();
            foreach (Transform transform in CombatUiManager.Instance.SpellSlots) { transform.gameObject.SetActive(false); }

            await SetupFight.Instance.DialogueSpawn(0); //dialogue
            while (!DialogueManager.Instance.IsEndingDialogue)
            {
                await UniTask.Yield();
            }

            SetupFight.Instance.DesactiveTrigger();
            ApplyWalkables(); // cases jaunes

            bool Moved = false;
            while (!Moved)
            {
                if (Input.GetMouseButtonUp(0) && Tools.CheckMouseRay(out WayPoint point) &&
                    !EventSystem.current.IsPointerOverGameObject(0))
                {
                    await TryMoveTo(point);
                    Moved = true;
                    ClearWalkables();
                    await SetupFight.Instance.DialogueSpawn(1);
                }
                await UniTask.Yield();
            }

            while (!DialogueManager.Instance.IsEndingDialogue)
            {
                await UniTask.Yield();
            }
            SetupFight.Instance.DesactiveTrigger();

            CombatUiManager.Instance.SpellSlots[0].gameObject.SetActive(true); //active le 1er slots
            SetupFight.Instance.SpellSlotHUD.SetActive(true);
            ShowSpellsUI();

            
            bool PlayedSpell = false;
            while (!PlayedSpell)
            {   
                foreach (DraggableSpell draggable in spellsUI)
                {
                    PlayedSpell |= await draggable.CheckForBeginDrag();
                }

                await UniTask.Yield();
            }

            ClearWalkables();
            SetupFight.Instance.DesactiveTrigger();

            CombatUiManager.Instance.endButton.gameObject.SetActive(true);

            CombatUiManager.Instance.ShakeButton();

            while (!endTurnButton.Pressed)
            {
                await UniTask.Yield();
            }
            CombatUiManager.Instance.StopButtonShake();

            await SetupFight.Instance.DialogueSpawn(2);
            while (!DialogueManager.Instance.IsEndingDialogue)
            {
                await UniTask.Yield();
            }

            await UniTask.Delay(250);
            foreach (Transform transform in CombatUiManager.Instance.SpellSlots) { transform.gameObject.SetActive(true); }
        }
        else
            await base.CheckPlayerInput();
        
    }
    
    public override async UniTask PlayTurn()
    {
        if (!PlayedOnTurnWithTuto)
        {
            await EntityBasePlayTurn();
            endTurnButton.Pressed = false;
            
            await CheckPlayerInput();
        
            await EndTurn();
            PlayedOnTurnWithTuto = true;
        }
        else await base.PlayTurn();
    }
}
