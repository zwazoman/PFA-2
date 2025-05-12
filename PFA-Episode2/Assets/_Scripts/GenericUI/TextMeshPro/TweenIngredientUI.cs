using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;


public class TweenIngredientUI : MonoBehaviour
{
    [SerializeField] private RectTransform _tmpRect;
    [SerializeField] private List<RectTransform> _panelToTween = new();
    [SerializeField] private HorizontalLayoutGroup _layout;
    public static TweenIngredientUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        //_layout.enabled = true;
        Sequence seq = DOTween.Sequence();
        seq.Join(_tmpRect.DOScale(0.9f, 0.7f).SetEase(Ease.OutBack));
        seq.Join(_tmpRect.DOAnchorPos(new Vector2(0, 955), 0.5f).SetEase(Ease.InOutQuad));
        await seq.AsyncWaitForCompletion();

        await TweenUISpawn();
    }

    public async Task TweenUISpawn()
    {
        for (int i = 0; i < _panelToTween.Count; i++)
        {
            RectTransform rect = _panelToTween[i];

            Sequence seq = DOTween.Sequence();
            //seq.Join(rect.DOScale(0.9f, 0.2f).SetEase(Ease.OutBack));

            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, -180), 0.5f).SetEase(Ease.OutQuad));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, -180), 0.5f).SetEase(Ease.OutQuad));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300, -180), 0.5f).SetEase(Ease.OutQuad));
                    break;
            }

            await seq.AsyncWaitForCompletion();
        }
    }

    public async Task TweenUIDespawn()
    {
        for (int i = 0; i < _panelToTween.Count; i++)
        {
            RectTransform rect = _panelToTween[i];

            Sequence seq = DOTween.Sequence();
            ///seq.Join(rect.DOScale(0f, 0.3f).SetEase(Ease.InBack));

            switch (i)
            {
                case 0:
                    seq.Join(rect.DOAnchorPos(new Vector2(-1300, -2100), 0.4f).SetEase(Ease.InCubic));
                    break;
                case 1:
                    seq.Join(rect.DOAnchorPos(new Vector2(0, -2100), 0.4f).SetEase(Ease.InCubic));
                    break;
                case 2:
                    seq.Join(rect.DOAnchorPos(new Vector2(1300, -2100), 0.4f).SetEase(Ease.InCubic));
                    break;
            }

            ///seq.Join(rect.DOScale(0.9f, 0.2f).SetEase(Ease.OutBack));
            await seq.AsyncWaitForCompletion();
        }
    }


}
