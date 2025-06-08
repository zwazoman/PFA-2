using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Serialization;

//controle la camera pendant les combats.
public class CameraBehaviour : MonoBehaviour
{

    Vector3 basePosition;
    private Vector3 TargetPosition;
    private Vector3 yOffset;
    [SerializeField] Transform _repereWorldCenter;
    [SerializeField] Camera _cam;

    [SerializeField] private float _offsetMagnitude = 1.3f,_dampingDuration = .1f;

    private Vector3 vel;
    float zoomVel = 0;

    [Header("Spell drag")]
    [SerializeField] private float _spellDragMagnitude=2.5f;

    private Vector2 _dragStart;
    //Singleton
    public static CameraBehaviour Instance => _instance;
    private static CameraBehaviour _instance;

    private bool screenIsShaking = false;

    private float _baseOrthoSize;
    
    [Header("zooming")]
    [SerializeField] float _zoomDampingDuration = .15f;
    private void Awake()
    {
        //singleton
        if (_instance == null) _instance = this;
        else
        {
            Debug.LogError("A singleton Instance Already Exists.");
            Destroy(this);
        }
    }

    private bool _isDraggingSpell;
    
    public void OnSpellDrag()
    {
        _isDraggingSpell = true;
        _dragStart = Input.mousePosition;
    }

    public void OnSpellReleased()
    {
        _isDraggingSpell = false;
    }

    private async void Start()
    {
        _repereWorldCenter.transform.parent = null;
        basePosition = transform.position;
        TargetPosition = basePosition;
        
        CombatManager.Instance.OnNewTurn += OnNewTurn;

        await Awaitable.NextFrameAsync();

        foreach(Entity e in CombatManager.Instance.PlayerEntities)
        {
            if(e is PlayerEntity )
            {
                e.stats.OnHealthUpdated+=(OnPlayerHit);
            }
        }
    }

    private void Update()
    {
        if (screenIsShaking) return;
        
        //centrer la cam sur les entités
        Bounds bounds = new();
        foreach (Entity e in CombatManager.Instance.Entities)
        {
            bounds.Encapsulate(e.transform.position);
        }
        Vector3 offset = (bounds.center - _repereWorldCenter.position).Flatten();
        TargetPosition = basePosition + offset * _offsetMagnitude;

        //ajouter draggables offset
        if (_isDraggingSpell)
        {
            Vector2 dir = (((Vector2)Input.mousePosition-new Vector2(Screen.width,Screen.height)*.5f) / new Vector2(Screen.height,Screen.height)  ) * _spellDragMagnitude;
            TargetPosition += transform.right * dir.x + transform.up * dir.y;
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref vel, _dampingDuration);
        
        //orthosize
        _cam.orthographicSize =
            Mathf.SmoothDamp(_cam.orthographicSize , _baseOrthoSize - (_isDraggingSpell? .5f : 0), ref zoomVel, _zoomDampingDuration);
    }

    private void OnPlayerHit(float delta,float newValue )
    {
        if(delta < 0 && Time.timeSinceLevelLoad > 1)
        {
            screenIsShaking = true;
            transform.DOShakePosition(.3f, .6f).onComplete = () => screenIsShaking = false;
        }
    }


    void OnNewTurn(Entity e)
    {
        if(e is PlayerEntity)
        {
            yOffset = Vector3.zero;
            //transform.DOMove(TargetPosition, .8f).SetEase(Ease.InOutSine);
            _baseOrthoSize = 3.6f;
            //_cam.DOOrthoSize(3.6f,.8f);
        }
        else
        {
            yOffset =  Vector3.up*1f;

            _baseOrthoSize = 4;
            
            //transform.DOMove(TargetPosition + Vector3.up*1f, .8f).SetEase(Ease.InOutSine);
            //_cam.DOOrthoSize(4,.8f);
        }
    }

}
