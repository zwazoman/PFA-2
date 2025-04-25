using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DraggableSpell : Draggable
{
    SpellData _spelldata;

    SpellCaster _spellCaster;

    public async UniTask DragAndDrop()
    {
        WayPoint _currentPoint = null;

        while (isDragging)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit);

            if (hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint point) && point != _currentPoint)
            {
                _currentPoint = point;
                _spellCaster.StopSpellZonePreview();
                _spellCaster.PreviewSpellZone(_spelldata, point);
            }

            await UniTask.Yield();
        }

        await _spellCaster.TryCastSpell(_spelldata, _currentPoint);
    }
}
