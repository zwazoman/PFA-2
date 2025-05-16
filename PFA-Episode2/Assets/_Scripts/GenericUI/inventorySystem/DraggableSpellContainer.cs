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

    private bool _faudraRemove;


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
            print("gg");
            backGroundImage.gameObject.SetActive(true);
            gameObject.transform.SetParent(a[0].gameObject.transform);
            gameObject.transform.localPosition = Vector3.zero;
            GameManager.Instance.playerEquipedSpell.Add(_infoVariant.ActualSpell);
            _faudraRemove = true;
        }
        else
        {
            Reset();
        }
    }

    public override void Reset()
    {
        if(_faudraRemove) { GameManager.Instance.playerEquipedSpell.Remove(_infoVariant.ActualSpell); _faudraRemove = false; }
        CancelButton.onClick.RemoveAllListeners();
        backGroundImage.gameObject.SetActive(false);
        base.Reset();
    }
}
