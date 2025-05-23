using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    private Vector3 _target;
    public float speed;
    public static PlayerMap Instance;
    public int PositionMap = 0;

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
        PositionMap = clickedNode.Position;
        clickedNode = null;
        return tmp.transform.position;
        
    }

    private async void LoadNextScene()
    {
        _target = await SetupTarget(); //Attendre que _target soit def
        await MoveTo(_target); //Attends qu'il soit arriv�

        //---------------- Charge une nouvelle sc�ne -------------------------

        int positionX = Mathf.RoundToInt(gameObject.transform.localPosition.x) / 10;
        positionX = positionX * 10;

        int Y = Mathf.RoundToInt(gameObject.transform.localPosition.y) / 10;
        int y = Y * 10;

        Vector3Int position = new Vector3Int(positionX, y, Mathf.RoundToInt(gameObject.transform.localPosition.z));

        foreach (KeyValuePair<Vector3Int, Node> KeyAndValues in MapMaker2.Instance.DicoNode)
        {
            if (KeyAndValues.Key == position)
            {
                SaveMapGeneration.Instance.SaveMap();
                if (KeyAndValues.Value.EventName.ToString() == "Start") { break; }
                await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.EventName.ToString());
            }
        }
    }

    async UniTask MoveTo(Vector3 targetPos) //Faut pas qu'il se lance au start
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            await UniTask.Yield();
        }
    }
}
