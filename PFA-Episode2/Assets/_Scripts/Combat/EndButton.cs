using UnityEngine;
using UnityEngine.UI;

public class EndButton : MonoBehaviour
{
    [field : HideInInspector] 
    public PlayerEntity currentEntity
    {
        get
        {
            return currentEntity;
        }
        set
        {
            currentEntity = value;
            Pressed = false;
            return;
        }
    }

    [HideInInspector] public bool Pressed;

    [SerializeField] Button _endButton;

    

    private void Awake()
    {
        if(_endButton ==  null)
            TryGetComponent(out _endButton);
    }

    private void Start()
    {
        _endButton.onClick.AddListener(Press);
    }

    public void Press()
    {
        Pressed = true;
    }

}
