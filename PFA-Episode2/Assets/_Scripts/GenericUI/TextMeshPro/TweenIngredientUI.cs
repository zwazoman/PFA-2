using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Script qui Tween les panels liées aux ingredients
/// </summary>
public class TweenIngredientUI : MonoBehaviour
{
    [SerializeField] private RectTransform _tmpRect;
    public List<RectTransform> PanelToTween = new();
    public static TweenIngredientUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start() //Pour le texte du début
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(_tmpRect.DOScale(0.9f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(_tmpRect.DOAnchorPos(new Vector2(0, 955), 0.35f).SetEase(Ease.InOutQuad));
        await seq.AsyncWaitForCompletion();

        await TweenUISpawn();
    }

    public async Task TweenUISpawn() //Spawn des 3 cartes
    {
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            RectTransform rect = PanelToTween[i];
            Sequence seq = DOTween.Sequence();

            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, -180), 0.35f).SetEase(Ease.OutBack));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, -180), 0.35f).SetEase(Ease.OutBack));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300, -180), 0.35f).SetEase(Ease.OutBack));
                    break;
            }

            await seq.AsyncWaitForCompletion();
        }
    }

    public async Task TweenUIDespawn() //Dispawn des 3 cartes
    {
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            Sequence seq = DOTween.Sequence();
            RectTransform rect = PanelToTween[i];

            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, -2100), 0.35f).SetEase(Ease.InBack));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, -2100), 0.35f).SetEase(Ease.InBack));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300, -2100), 0.35f).SetEase(Ease.InBack));
                    break;
            }
            await seq.AsyncWaitForCompletion();
        }
    }

    public async Task Monte(RectTransform chosenUI) //Carte choisi 
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(chosenUI.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 5, 0.6f));
        await Task.Delay(200);
        seq.Join(chosenUI.DOAnchorPos(new Vector2(chosenUI.anchoredPosition.x, 2100), 0.35f).SetEase(Ease.InBack));
        await Task.Delay(400);
        chosenUI.position = new Vector2(chosenUI.anchoredPosition.x, -2100);
    }


}
