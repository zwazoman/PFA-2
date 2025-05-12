using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UI;

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

    [Header("Scene references")]
    [SerializeField] public AnimatedPanel playerHUD;
    [SerializeField] public EndButton endButton;

    [SerializeField] public List<Transform> SpellSlots = new();
    [SerializeField] HorizontalOrVerticalLayoutGroup _entityFramesGroup;
    [SerializeField] EntityInfoFrame _playerFrame;

    [Header("Asset References")]
    [SerializeField] EntityInfoFrame _ennemyInfoFramePrefab;

    public void RegisterEntity(Entity e)
    {
        if(e is EnemyEntity)
        {
            EntityInfoFrame frame = (EntityInfoFrame)Instantiate(_ennemyInfoFramePrefab, _entityFramesGroup.transform);
            frame.Setup(e);
            frame.transform.SetAsFirstSibling();
        }
        else if(e is PlayerEntity)
        {
            _playerFrame.Setup(e);
        }

        
        

    }
}
