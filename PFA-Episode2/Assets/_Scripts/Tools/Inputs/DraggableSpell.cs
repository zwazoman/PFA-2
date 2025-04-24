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
        while (isDragging)
        {
            WayPoint _currentPoint = null;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out hit);

            if (hit.collider.gameObject.TryGetComponent<WayPoint>(out WayPoint point) && point != _currentPoint)
            {
                _currentPoint = point;
                _spellCaster.StopSpellCastPreview();
                _spellCaster.PreviewSpellCast(_spelldata, point);
            }

            await UniTask.Yield();

            await _spellCaster.CastSpell(_spelldata, _currentPoint);
        }
    }
}
