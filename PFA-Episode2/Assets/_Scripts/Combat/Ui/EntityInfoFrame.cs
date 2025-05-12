using Cysharp.Threading.Tasks;
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
        owner.stats.healthFeedbackTasks.Add(UpdateLifebar);
        _lifebar.MaxValue = owner.stats.maxHealth;
        _lifebar.MinValue = 0;
        UpdateLifebar(0, owner.stats.currentHealth);

        //icon
        _image.sprite = _owner.Icon;

    }

    private async UniTask UpdateLifebar(float delta, float newValue)
    {
        _lifebar.Value = newValue;
    }
}
