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
                Debug.Log("Attente de fin du 1er dialogue");
            }

            ApplyWalkables(); // cases jaunes
            //dialogue tuto movement

            bool Moved = false;
            while (!Moved)
            {
                Debug.Log("ahhh");
                //si il clique sur une case de mouvement
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
                Debug.Log("Attente de fin du 1er dialogue");
            }
            CombatUiManager.Instance.SpellSlots[0].gameObject.SetActive(true); //active le 1er slots
            SetupFight.Instance.SpellSlotHUD.SetActive(true);
            ShowSpellsUI();

            
            bool PlayedSpell = false;
            while (!PlayedSpell)
            {
                //si il joue un spell
                
                foreach (DraggableSpell draggable in spellsUI)
                {
                    PlayedSpell |= await draggable.CheckForBeginDrag();
                    //PlayedSpell = true;
                }

                await UniTask.Yield();
                Debug.Log("waiting for player to use a spell. " + PlayedSpell);
            }
            ClearWalkables();
            await SetupFight.Instance.DialogueSpawn(2);

            CombatUiManager.Instance.endButton.gameObject.SetActive(true);
            //HideSpellsUI();
            //dialogue tuto end turn

            //jiggle end button when no more action is possible
            CombatUiManager.Instance.ShakeButton();
            while (!endTurnButton.Pressed)
            {
                await UniTask.Yield();
            }
            CombatUiManager.Instance.StopButtonShake();
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
