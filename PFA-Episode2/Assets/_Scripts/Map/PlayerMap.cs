using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    private Vector3 _target;
    public float speed;
    public static PlayerMap Instance;
    public int PositionMap = 0;
    public int Y;

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
        await MoveTo(_target); //Attends qu'il soit arrivé

        //---------------- Charge une nouvelle scène -------------------------

        int positionX = Mathf.RoundToInt((gameObject.transform.localPosition.x) / 10);
        positionX = positionX * 10;

        int y = Mathf.RoundToInt((gameObject.transform.localPosition.y) / 10);
        Y = y * 10;

        Vector3Int position = new Vector3Int(positionX, Y, Mathf.RoundToInt(gameObject.transform.localPosition.z));

        foreach (KeyValuePair<Vector3Int, Node> KeyAndValues in MapMaker2.Instance.DicoNode)
        {
            print("Ma position : " + position);
            print(KeyAndValues);
            if (KeyAndValues.Key == position)
            {
                print("true");
                SaveMapGeneration.Instance.SaveMap();
                GameManager.Instance.playerInventory.Save(GameManager.Instance.playerInventory.NameSave);

                if (KeyAndValues.Value.EventName.ToString() == "Start") { break; }
                if (KeyAndValues.Value.EventName.ToString() == "Combat")
                {
                    await SceneTransitionManager.Instance.GoToScene(GameManager.Instance.ReturnSceneCombat().ToString());
                }
                else { await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.EventName.ToString()); }
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
