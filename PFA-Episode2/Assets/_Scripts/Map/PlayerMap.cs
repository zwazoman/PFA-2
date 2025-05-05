using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    private Vector3 _target;
    public float speed = 5f;
    public static PlayerMap Instance;
    private bool hasArrived = false;
    private Vector3Int position;

    [HideInInspector] public Node clickedNode;

    private void Awake()
    {
        Instance = this;
        _target = transform.position;
    }

    private void Start()
    {
        LoadNextScene();
    }

    public async UniTask<Vector3> SetupTarget() 
    { 
        while(clickedNode == null)
        {
            await UniTask.Yield();
        }
        Node tmp = clickedNode;
        clickedNode = null;
        return tmp.transform.position;
        
    }

    private async void LoadNextScene()
    {
        _target = await SetupTarget(); //Attendre que _target soit def
        await MoveTo(_target, speed); //Attends qu'il soit arrivé

        //---------------- Charge une nouvelle scène -------------------------

        int positionX = Mathf.RoundToInt(gameObject.transform.localPosition.x) / 10;
        positionX = positionX * 10;

        int Y = Mathf.RoundToInt(gameObject.transform.localPosition.y) / 10;
        int y = Y * 10;

        position = new Vector3Int(positionX, y, Mathf.RoundToInt(gameObject.transform.localPosition.z));

        foreach (KeyValuePair<Vector3Int, Node> KeyAndValues in MapMaker2.Instance.DicoNode)
        {
            print(positionX);
            if (KeyAndValues.Key == position)
            {
                SaveMapGeneration.Instance.SaveMap();

                if (KeyAndValues.Value.EventName.ToString() == "Start") { break; }

                await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.EventName.ToString());
            }
        }
    }

    async UniTask MoveTo(Vector3 targetPos, float moveSpeed = 8) //Faut pas qu'il se lance au start
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed);
            await UniTask.Yield();
        }
    }
}
