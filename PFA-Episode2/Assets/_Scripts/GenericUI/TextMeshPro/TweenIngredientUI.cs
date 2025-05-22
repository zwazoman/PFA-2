using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// Script qui Tween les panels liées aux ingredients
/// </summary>
public class TweenIngredientUI : MonoBehaviour
{
    [SerializeField] private List<Button> _buttonList;
    [SerializeField] private RectTransform _tmpRect;
    public List<RectTransform> PanelToTween = new();
    public static TweenIngredientUI Instance;

    const float TweenDuration = .25f;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start() //Pour le texte du début
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(_tmpRect.DOScale(0.9f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(_tmpRect.DOAnchorPos(new Vector2(0, 955), TweenDuration).SetEase(Ease.InOutQuad));
        await seq.AsyncWaitForCompletion().AsUniTask();

        await TweenUISpawn();
    }

    public async UniTask TweenUISpawn() //Spawn des 3 cartes
    {
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            RectTransform rect = PanelToTween[i];
            Sequence seq = DOTween.Sequence();
            _buttonList[i].interactable = true;
            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, 0), TweenDuration).SetEase(Ease.OutBack));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, 0), TweenDuration).SetEase(Ease.OutBack));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300,0), TweenDuration).SetEase(Ease.OutBack));
                    break;
            }

            await seq.AsyncWaitForCompletion().AsUniTask();
        }
    }

    public async UniTask TweenUIDespawn() //Dispawn des 3 cartes
    {
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            RectTransform rect = PanelToTween[i];

            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, -2100), TweenDuration).SetEase(Ease.InBack));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, -2100), TweenDuration).SetEase(Ease.InBack));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300, -2100), TweenDuration).SetEase(Ease.InBack));
                    break;
            }
            await seq.AsyncWaitForCompletion().AsUniTask();
        }
    }

    public async UniTask Monte(RectTransform chosenUI) //Carte choisi 
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(chosenUI.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), .35f, 5, 0.6f));
        seq.Join(chosenUI.DOAnchorPos(new Vector2(chosenUI.anchoredPosition.x, 2100), TweenDuration).SetEase(Ease.InBack));

        await seq.AsyncWaitForCompletion().AsUniTask();
        chosenUI.position = new Vector2(chosenUI.anchoredPosition.x, -2100);
    }


}
