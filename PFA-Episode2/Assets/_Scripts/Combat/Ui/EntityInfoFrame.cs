using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EntityInfoFrame : MonoBehaviour
{
    [SerializeField] Image _image;
    [Header("SceneReferences")]

    [SerializeField] CoolSlider _lifebar;

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

        owner.OnDead += () => { _lifebar.Value = 0; _image.color = Color.gray; };


    }

    private async UniTask OnHpUpdated(float delta, float newValue)
    {
        _lifebar.Value = newValue;
        if (delta < 0) { transform.DOShakePosition(.2f, 20); }
    }
}
