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
    [SerializeField] TMP_Text _hpText;

    Entity _owner;


    public void Setup(Entity owner)
    {
        this._owner = owner;

        //healthbar
        owner.stats.healthFeedbackTasks.Add(OnHpUpdated);
        _lifebar.MaxValue = owner.stats.maxHealth;
        _lifebar.MinValue = 0;
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
