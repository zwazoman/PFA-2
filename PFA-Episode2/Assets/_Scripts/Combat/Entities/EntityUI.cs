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
        owner.stats.OnHealthUpdated+=(UpdateLifebar);
        lifebar.MaxValue = owner.stats.maxHealth;
        lifebar.MinValue = 0;
        UpdateLifebar(0,owner.stats.currentHealth);

        //shield bar
        _shieldBar.MaxValue = owner.stats.maxHealth;
        _shieldBar.MinValue = 0;
        owner.stats.ShieldUpdateFeeback += OnShieldUpdated;

        //death handling
        owner.OnDead += () => { UpdateLifebar(-1, 0); OnShieldUpdated(0); };

        //topleft icons
        CombatUiManager.Instance.RegisterEntity(owner);

        //spell preview
        owner.OnPreviewSpell += (float newShield, float newHP, Vector3 direction) =>
        {
            lifebar.PreviewValue(newHP);
            _shieldBar.PreviewValue(newShield);
        };

        owner.OnSpellPreviewCancel += () =>
        {
            lifebar.CancelPreview();
            _shieldBar.CancelPreview();
        };
    }
    private void OnShieldUpdated(float newShield)
    {
        _shieldBar.Value = newShield;
    }

    private void UpdateLifebar(float delta, float newValue)
    {
        //Debug.Log("delta : " + delta.ToString() + " , new value : " + newValue.ToString());
        lifebar.Value = newValue;
    }
}
