using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class PlayerMap : MonoBehaviour
{
    private Vector3 _target;
    public float speed;
    public static PlayerMap Instance;
    public int PositionMap = 0;
    public int Y;
    [SerializeField] private Animator _animator;

    [HideInInspector] public Node clickedNode;

    //camera
    [SerializeField] Transform _camera;
    [SerializeField] Vector3 camOffset = new Vector3(-3.75f, 1.6f, 0);

#if UNITY_EDITOR
    private void OnValidate()
    {
        _camera.transform.position = transform.position + camOffset;
    }
#endif

    private void Awake()
    {
        Instance = this;
        _target = transform.position;
    }

    private void Start()
    {
        GameManager.Instance.playerInventory.Save(GameManager.Instance.playerInventory.NameSave);
        GameManager.Instance.CalculateProgress();
        LoadNextScene();
        Vector3 p = transform.position + camOffset;
        p.z = 100;
        _camera.transform.position = p;

        if (PositionMap == 0)
        {
            SaveMapGeneration.Instance.SaveMap();
        }
    }

    public async UniTask<Vector3> SetupTarget() 
    { 
        while(clickedNode == null)
        {
            await UniTask.Yield();
        }
        Node tmp = clickedNode;
        PositionMap = clickedNode.Position;
        clickedNode = null;
        return tmp.transform.position;     
    }

    private async void LoadNextScene()
    {
        _target = await SetupTarget(); //Attendre que _target soit def
        await MoveTo(_target); //Attends qu'il soit arriv�

        //---------------- Charge une nouvelle sc�ne -------------------------
        int positionX = Mathf.RoundToInt((gameObject.transform.localPosition.x) / 10);
        positionX = positionX * 10;

        int y = Mathf.RoundToInt((gameObject.transform.localPosition.y) / 10);
        Y = y * 10;

        Vector3Int position = new Vector3Int(positionX, Y, Mathf.RoundToInt(gameObject.transform.localPosition.z));

        foreach (KeyValuePair<Vector3Int, Node> KeyAndValues in MapMaker2.Instance.DicoNode)
        {
            if (KeyAndValues.Key == position)
            {
                GameManager.Instance.PlayerPosMap.playerPosition = Vector3Int.RoundToInt(transform.localPosition);
                GameManager.Instance.PlayerPosMap.PositionMap = PositionMap;
                GameManager.Instance.PlayerPosMap.Y = Y;

                GameManager.Instance.playerInventory.Save(GameManager.Instance.playerInventory.NameSave);
                if (KeyAndValues.Value.EventName.ToString() == "Start") { break; }
                if (KeyAndValues.Value.EventName.ToString() == "Combat")
                {
                    await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.CombatScene);
                }
                else if (KeyAndValues.Value.EventName.ToString() == "Boss")
                {
                    await SceneTransitionManager.Instance.GoToScene("Forest_Combat_Boss");
                }
                else if (KeyAndValues.Value.EventName.ToString() == "Tuto")
                {
                    await SceneTransitionManager.Instance.GoToScene("Forest_Combat_Tuto");
                }
                else { await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.EventName.ToString()); }
            }
        }
    }

    async UniTask MoveTo(Vector3 targetPos) //Faut pas qu'il se lance au start
    {
        _animator.PlayAnimationBool("Move");
        Vector3 p = targetPos + camOffset;
        p.z = 100;
        _camera.transform.DOMove(p, 1f / speed).SetEase(Ease.InOutSine);
        await transform.DOMove(targetPos,1f/speed).SetEase(Ease.InOutCubic).AsyncWaitForCompletion().AsUniTask();
        _animator.EndAnimationBool("Move");
    }
}
