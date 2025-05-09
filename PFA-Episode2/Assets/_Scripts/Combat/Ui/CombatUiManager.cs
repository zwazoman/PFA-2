using UnityEngine;
using System.Collections.Generic;

public class CombatUiManager : MonoBehaviour
{
    #region Singleton
    private static CombatUiManager instance;

    public static CombatUiManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Combat Ui Manager");
                instance = go.AddComponent<CombatUiManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] public AnimatedPanel playerHUD;
    [SerializeField] public EndButton endButton;

    [SerializeField] public List<Transform> SpellSlots = new();
}
