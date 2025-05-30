using TMPro;
using UnityEngine;

public class SetupHealMap : MonoBehaviour
{
    [Header("SceneReferences")]

    [SerializeField] private CoolSlider _lifebar;
    [SerializeField] private TMP_Text _hpText;

    private void Start() { SetupLife(); }

    private void SetupLife()
    {
        print(GameManager.Instance.playerInventory.playerHealth.health);
        _lifebar.Value = GameManager.Instance.playerInventory.playerHealth.health;
        _hpText.text = GameManager.Instance.playerInventory.playerHealth.health.ToString();
    }
}
