using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfoFrame : MonoBehaviour
{
    [SerializeField] Image _image;
    [Header("SceneReferences")]

    [SerializeField] CoolSlider _lifebar;
    [SerializeField] CoolSlider _shieldBar;
    [SerializeField] TMP_Text _hpText;
    [SerializeField] TMP_Text _shieldText;

    Entity _owner;


    public void Setup(Entity owner)
    {
        this._owner = owner;

        //healthbar
        owner.stats.healthFeedbackTasks.Add(OnHpUpdated);

        owner.stats.ShieldUpdateFeeback += (float a) => {
            _shieldBar.Value = owner.stats.shieldAmount;
            _shieldText.text = owner.stats.shieldAmount>0? owner.stats.shieldAmount.ToString() : "";
        };

        _lifebar.MaxValue = _shieldBar.MaxValue = owner.stats.maxHealth;
        _lifebar.MinValue = _shieldBar.MinValue = 0;
        OnHpUpdated(-1, owner.stats.currentHealth);

        //icon
        _image.sprite = _owner.Icon;

        owner.OnDead += () => { OnHpUpdated(-1, owner.stats.currentHealth); _image.color = new Color(.5f,.4f,.4f); };

    }

    private async UniTask OnHpUpdated(float delta, float newValue)
    {
        _lifebar.Value = newValue;
        _hpText.text = Mathf.Round(newValue).ToString();
        if (delta < 0 && Time.timeSinceLevelLoad > 1) { transform.DOShakePosition(.2f, 20); }
    }
}
