using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableIngredientContainer : DraggableItemContainer
{
    [Header("SceneReferences")]

    [SerializeField] Button CancelButton;
    [SerializeField] Image image;
    [SerializeField] Image backGroundImage;
    [SerializeField] GameObject DeleteIcon;

    private void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        IngredientBase i = (IngredientBase)item;
        image.sprite = i.sprite;
        backGroundImage.sprite = i.sprite;
        DeleteIcon.SetActive(false);
    }

    public void SetUp(Item i)
    {
        item = i;
        SetUp();
    }



    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        List<RaycastResult> a = new();
        EventSystem.current.RaycastAll(eventData, a);
        if (a[0].gameObject.TryGetComponent(out CookingPot pot) && pot.TryAddIngredient(this))
        {
            DeleteIcon.SetActive(true);
            CancelButton.onClick.AddListener( () => pot.RemoveIngredient(this));
            gameObject.SetActive(false);
        }
        else
        {
            Reset();
        }
    }

    public override void Reset()
    {
        CancelButton.onClick.RemoveAllListeners();
        DeleteIcon.SetActive(false);
        base.Reset();
    }
}
