using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSpellContainer : DraggableItemContainer
{
    [Header("SceneReferences")]

    [SerializeField] Button CancelButton;
    [SerializeField] Image backGroundImage;
    [SerializeField] GameObject DeleteIcon;
    [SerializeField] Image _descriptionPanel;
    [SerializeField] GetInfoInVariant _infoVariant;
    public Transform Target;
    [SerializeField] Transform _enfant;

    public bool _faudraRemove;

    private void Start()
    {
        if (Target == null) { Target = gameObject.transform.transform.parent; }

    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _descriptionPanel.gameObject.SetActive(true);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _descriptionPanel.gameObject.SetActive(false);
        List<RaycastResult> a = new();
        EventSystem.current.RaycastAll(eventData, a);
        if (a.Count > 0 && a[0].gameObject.layer == 8)
        {
            backGroundImage.gameObject.SetActive(true);
            gameObject.transform.SetParent(a[0].gameObject.transform);
            gameObject.transform.localPosition = Vector3.zero;
            _enfant.transform.SetParent(Target);
            _enfant.transform.localPosition = Vector3.zero;
            GameManager.Instance.playerInventory.playerEquipedSpellIndex.Add(_infoVariant.IndexInPlayerSpell);
            _faudraRemove = true;
        }
        else
        {
            Reset();
        }
    }

    public override void Reset()
    {
        _enfant.parent = gameObject.transform;
        _enfant.localPosition = Vector3.zero;
        if(_faudraRemove) { GameManager.Instance.playerInventory.playerEquipedSpellIndex.Remove(_infoVariant.IndexInPlayerSpell); _faudraRemove = false; }
        CancelButton.onClick.RemoveAllListeners();
        backGroundImage.gameObject.SetActive(false);
        base.Reset();
    }
}
