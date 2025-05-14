using Cysharp.Threading.Tasks;
using UnityEngine;

public class EntityUI : MonoBehaviour
{
    [Header("SceneReferences")]
    [SerializeField] CoolSlider lifebar;
    [SerializeField] CoolSlider _shieldBar;

    Entity owner;


    public void Setup(Entity owner)
    {
        this.owner = owner;
        
        //healthbar
        owner.stats.healthFeedbackTasks.Add(UpdateLifebar);
        lifebar.MaxValue = owner.stats.maxHealth;
        lifebar.MinValue = 0;
        UpdateLifebar(0,owner.stats.currentHealth);
        
        //shield bar
        owner.stats.ShieldUpdateFeeback += OnShieldUpdated;

        //death handling
        owner.OnDead += () => { UpdateLifebar(-1, 0); OnShieldUpdated(0); };


        //topleft icons
        CombatUiManager.Instance.RegisterEntity(owner);
    }
    private void OnShieldUpdated(float newShield)
    {
        _shieldBar.Value = newShield;
    }

    private async UniTask UpdateLifebar(float delta, float newValue)
    {
        //Debug.Log("delta : " + delta.ToString() + " , new value : " + newValue.ToString());
        lifebar.Value = newValue;
    }
}
