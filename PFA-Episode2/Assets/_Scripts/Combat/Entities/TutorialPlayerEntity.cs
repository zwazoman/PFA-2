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
            CombatUiManager.Instance.StopButtonShake();


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
                    //dialogue tuto spells

                }

                await UniTask.Yield();
            }

            ShowSpellsUI();
            bool PlayedSpell = false;
            CombatUiManager.Instance.endButton.gameObject.SetActive(false);
            while (!PlayedSpell)
            {
                //si il joue un spell
                
                foreach (DraggableSpell draggable in spellsUI)
                {
                    PlayedSpell |= await draggable.BeginDrag();
                    //PlayedSpell = true;
                }

                await UniTask.Yield();
                Debug.Log("waiting for player to use a spell. " + PlayedSpell);
            }

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
            ApplyWalkables();
            
            
            await CheckPlayerInput();
        
            await EndTurn();
            PlayedOnTurnWithTuto = true;
        }else await base.PlayTurn();
    }
}
