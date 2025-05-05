using System.Collections.Generic;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    private Vector3 _target;
    public float speed = 5f;
    public static PlayerMap Instance;
    private bool hasArrived = false;
    private Vector3Int position;
    private int positionX = -2550;

    private void Awake()
    {
        Instance = this;
        _target = transform.position;
    }

    public void SetupTarget(Vector3 target)
    {
        _target = target;
    }


    private async void Update()
    {
        if (Vector3.Distance(transform.position, _target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
            hasArrived = false;
        }
        else if (!hasArrived)
        {
            print("Arrivé");
            hasArrived = true;
            positionX = positionX + 300;
            int Y = Mathf.RoundToInt(gameObject.transform.localPosition.y) / 10;
            int y = Y * 10;
            position = new Vector3Int(positionX, y, Mathf.RoundToInt(gameObject.transform.localPosition.z));
            foreach(KeyValuePair<Vector3Int, Node> KeyAndValues in MapMaker2.Instance.DicoNode)
            {
                if (KeyAndValues.Key == position)
                {
                    SaveMapGeneration.Instance.SaveMap();
                    if (KeyAndValues.Value.EventName.ToString() == "Start") { break; }
                    await SceneTransitionManager.Instance.GoToScene(KeyAndValues.Value.EventName.ToString());
                }
            }
        }
    }

}
