using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DraggableSpellContainer : DraggableItemContainer
{
        [Header("SceneReferences")]

    [SerializeField] Button CancelButton;
    [SerializeField] Image image;
    [SerializeField] Image backGroundImage;
    [SerializeField] private GameObject DisabledImage;
    
    [SerializeField] private SpellInfoPopup _descriptionPanel;
    
    int indexInInventory;
    
    public void SetUp(SpellData spell,int indexInInventory )
    {
        this.indexInInventory = indexInInventory;
        
        image.sprite = spell.Sprite;
        backGroundImage.sprite = spell.Sprite;
        
        DisabledImage.SetActive(false);
        
        //description panel
        _descriptionPanel?.Setup(spell);
        _descriptionPanel?.gameObject.SetActive(false);
        EventClicked += () =>
        {
            _descriptionPanel?.gameObject.SetActive(true);
            _descriptionPanel.ClearParent();
            
        };
        EventClickedSomewhereElse += () =>
        {
            _descriptionPanel?.gameObject.SetActive(false);
            _descriptionPanel.AttachToTransform(transform) ;
        };
        

    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        
        //enable description popup
        _descriptionPanel?.gameObject.SetActive(true);
        _descriptionPanel?.AttachToTransform(transform);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        
        //disable description popup
        _descriptionPanel?.gameObject.SetActive(false);
        
        List<RaycastResult> a = new();
        EventSystem.current.RaycastAll(eventData, a);
        if (a.Count > 0 && a[0].gameObject.layer == 8)
        {
            DisplayAsEquipped(a[0].gameObject.transform);
            
            //save spell index to be equipped
            GameManager.Instance.playerInventory.playerEquipedSpellIndex.Add(indexInInventory);
        }
        else
        {
            Reset();
        }
    }

    public void DisplayAsEquipped(Transform ParentSlot)
    {
        //attach to bottom slot
        transform.parent = ParentSlot;
        transform.localPosition = Vector3.zero;
            
        //set up cancel button
        DisabledImage.SetActive(true);
        CancelButton.onClick.AddListener(()=>
        {
            GameManager.Instance.playerInventory.playerEquipedSpellIndex.Remove(indexInInventory);
            Reset();
        });
    }

    public override void Reset()
    {
        CancelButton.onClick.RemoveAllListeners();
        DisabledImage.SetActive(false);
        base.Reset();
    }
}
