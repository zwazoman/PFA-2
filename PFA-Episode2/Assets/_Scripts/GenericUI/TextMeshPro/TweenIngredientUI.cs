using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// Script qui Tween les panels li�es aux ingredients
/// </summary>
public class TweenIngredientUI : MonoBehaviour
{
    [SerializeField] private List<Button> _buttonList;
    [SerializeField] private RectTransform _tmpRect;
    public List<RectTransform> PanelToTween = new();
    public static TweenIngredientUI Instance;
    [SerializeField] private int _targetX = 1300;
    [SerializeField] private int _targetY = 0;
    [SerializeField] private int _targetYTxt = 955;

    const float TweenDuration = .25f;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start() //Pour le texte du d�but
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(_tmpRect.DOScale(0.9f, 0.35f).SetEase(Ease.OutBack));
        seq.Join(_tmpRect.DOAnchorPos(new Vector2(0, _targetYTxt), TweenDuration).SetEase(Ease.InOutQuad));
        await seq.AsyncWaitForCompletion().AsUniTask();
        await TweenUISpawn();
    }

    public async UniTask TweenUISpawn() //Spawn des 3 cartes
    {
        foreach (Button btn in _buttonList) { btn.interactable = false; }
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            RectTransform rect = PanelToTween[i];
            switch (i)
            {
                case 0:
                    SFXManager.Instance.PlaySFXClip(Sounds.CardEnter);
                    await rect.DOAnchorPos(new Vector2(-_targetX, _targetY), TweenDuration).SetEase(Ease.OutBack);
                    break;
                case 1:
                    SFXManager.Instance.PlaySFXClip(Sounds.CardEnter);
                    await rect.DOAnchorPos(new Vector2(0, _targetY), TweenDuration).SetEase(Ease.OutBack);
                    break;
                case 2:
                    SFXManager.Instance.PlaySFXClip(Sounds.CardEnter);
                    await rect.DOAnchorPos(new Vector2(_targetX, _targetY), TweenDuration).SetEase(Ease.OutBack);
                    break;
            }
        }
        foreach (Button btn in _buttonList) { btn.interactable = true; }
    }

    public async UniTask TweenUIDespawn() //Dispawn des 3 cartes
    {
        foreach (Button btn in _buttonList) { btn.interactable = false; }
        for (int i = 0; i < PanelToTween.Count; i++)
        {
            RectTransform rect = PanelToTween[i];

            switch (i)
            {
                case 0:
                    await rect.DOAnchorPos(new Vector2(-_targetX, -2100), TweenDuration).SetEase(Ease.InBack);
                    SFXManager.Instance.PlaySFXClip(Sounds.CardExit);
                    break;
                case 1:
                    SFXManager.Instance.PlaySFXClip(Sounds.CardExit);
                    await rect.DOAnchorPos(new Vector2(0, -2100), TweenDuration).SetEase(Ease.InBack);
                    break;
                case 2:
                    SFXManager.Instance.PlaySFXClip(Sounds.CardExit);
                    await rect.DOAnchorPos(new Vector2(_targetX, -2100), TweenDuration).SetEase(Ease.InBack);
                    break;
            }
        }
        foreach (Button btn in _buttonList) { btn.interactable = true; }
    }

    public async UniTask Monte(RectTransform chosenUI) //Carte choisi 
    {
        foreach(Button btn in _buttonList) { btn.interactable = false; }
        SFXManager.Instance.PlaySFXClip(Sounds.UiTwinkle);
        await chosenUI.DOPunchScale(Vector3.one*.2f, .25f, 5, 0.6f);
        await chosenUI.DOAnchorPos(new Vector2(chosenUI.anchoredPosition.x, 2100), TweenDuration).SetEase(Ease.InBack);
        chosenUI.position = new Vector2(chosenUI.anchoredPosition.x, -2100);
        foreach (Button btn in _buttonList) { btn.interactable = true; }
    }


}
