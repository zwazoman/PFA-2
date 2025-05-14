using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

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

        //shield bar
        owner.stats.ShieldUpdateFeeback += OnShieldUpdated;
        _lifebar.MaxValue = _shieldBar.MaxValue = owner.stats.maxHealth;
        _lifebar.MinValue = _shieldBar.MinValue = 0;
        OnHpUpdated(-1, owner.stats.currentHealth);

        //icon
        _image.sprite = _owner.Icon;

        owner.OnDead += () => { OnHpUpdated(-1,0); OnShieldUpdated(0); _image.color = new Color(.5f,.4f,.4f); };

    }

    private void OnShieldUpdated(float newShield)
    {
        _shieldBar.Value = newShield;
        _shieldText.text = newShield > 0 ? newShield.ToString() : "";
    }

    private async UniTask OnHpUpdated(float delta, float newValue)
    {
        _lifebar.Value = newValue;
        _hpText.text = Mathf.Round(newValue).ToString();
        if (delta < 0 && Time.timeSinceLevelLoad > 1) { transform.DOShakePosition(.2f, 20); }
    }
}
