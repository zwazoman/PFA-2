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
    
    
    public void SetUp(SpellData spell)
    {
<<<<<<< Updated upstream
        if (Target == null) { Target = gameObject.transform.transform.parent; }

=======
        image.sprite = spell.Sprite;
        backGroundImage.sprite = spell.Sprite;
        
        DisabledImage.SetActive(false);
        
        EventClicked += () =>
        {
            _descriptionPanel?.gameObject.SetActive(true);
            _descriptionPanel.transform.parent = transform.root;
            
        };
        EventClickedSomewhereElse += () =>
        {
            _descriptionPanel?.gameObject.SetActive(true);
            _descriptionPanel.transform.parent = transform;
        };
        
>>>>>>> Stashed changes
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        
        //enable description popup
        _descriptionPanel?.gameObject.SetActive(true);
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
            //attach to bottom slot
            transform.parent = a[0].gameObject.transform;
            transform.localPosition = Vector3.zero;
            
            DisabledImage.SetActive(true);
            CancelButton.onClick.AddListener(Reset );
        }
        else
        {
            Reset();
        }
    }

    public override void Reset()
    {
        CancelButton.onClick.RemoveAllListeners();
        DisabledImage.SetActive(false);
        base.Reset();
    }
}
