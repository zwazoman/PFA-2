using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    [SerializeField] public List<Transform> SpellSlots = new(); //Il contient les items
    [SerializeField] HorizontalOrVerticalLayoutGroup _entityFramesGroup;
    [SerializeField] EntityInfoFrame _playerFrame;

    [Header("Asset References")]
    [SerializeField] EntityInfoFrame _ennemyInfoFramePrefab;

    #region ButtonShake

    private bool buttonIsShaking = false;
    public async void ShakeButton()
    {
        buttonIsShaking = true;
        while (buttonIsShaking)
        {
            endButton.transform.DOShakeRotation(1, Vector3.forward * 5, 10);
            await endButton.transform.DOShakePosition(1,Vector3.one*8,2,randomness:10f,randomnessMode:ShakeRandomnessMode.Harmonic).ToUniTask();
        }
    }
    public void StopButtonShake()
    {
        buttonIsShaking = false;
    }
    
    #endregion
    
    public void RegisterEntity(Entity e)
    {
        switch (e)
        {
            case AIEntity:
            {
                EntityInfoFrame frame = (EntityInfoFrame)Instantiate(_ennemyInfoFramePrefab, _entityFramesGroup.transform);
                frame.Setup(e);
                frame.transform.SetAsFirstSibling();
                break;
            }
            case PlayerEntity:
                _playerFrame.Setup(e);
                break;
        }
    }
}
