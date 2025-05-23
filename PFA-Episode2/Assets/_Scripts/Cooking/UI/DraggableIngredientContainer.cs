using System.Collections.Generic;
using TMPro;
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
    [SerializeField] Image _descriptionPanel;
    [SerializeField] Image _descriptionSprite;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] TextMeshProUGUI _descriptionTitle;

    private void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        IngredientBase i = (IngredientBase)item;
        image.sprite = i.sprite;
        backGroundImage.sprite = i.sprite;
        _descriptionTitle.text = i.name;
        
        if (i is Sauce Sauce) { _descriptionText.text = Serializer.GetSauceEffectString(Sauce); _descriptionSprite.sprite = ((Sauce)i).areaOfEffect.sprite; }
        else if (i is Ingredient Ing) { _descriptionText.text = Serializer.GetIngredientEffectString(Ing); _descriptionSprite.sprite = i.sprite; }

        DeleteIcon.SetActive(false);
    }

    public void SetUp(Item i)
    {
        item = i;
        SetUp();
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
        if (a.Count > 0 && a[0].gameObject.TryGetComponent(out CookingPot pot) && pot.TryAddIngredient(this))
        {
            DeleteIcon.SetActive(true);
            CancelButton.onClick.AddListener(() => pot.RemoveIngredient(this));
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
