using System.Collections.Generic;
using UnityEngine;

public class SpawnGround : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> _spawnPoints = new();
    [SerializeField] private List<GameObject> _spawnSpecialGround = new();
    [SerializeField] private GameObject _spawnGround;
    [SerializeField][Range(0f, 1f)] private float _spawnProbability = 0.5f;
    public List<GameObject> GroundList = new();

    [Header("Others")]
    [SerializeField] private Transform _parent;
    private GameObject _imposibleGround;

    // Seed
    [HideInInspector] public bool UseSeed = false;
    [HideInInspector] public int Seed = 0;

    #region Singleton
    public static SpawnGround Instance;

    private void Awake() { Instance = this; }
    #endregion

    public void StartSpawnRiver() //Dans MapMaker2 et SaveMapGeneration
    {
        if (UseSeed)
        {
            Random.InitState(Seed);
        }
        else
        {
            Seed = Random.Range(0, int.MaxValue);
            Random.InitState(Seed);
            UseSeed = true;
        }

        List<GameObject> spawnSpecialGroundClone = new List<GameObject>(_spawnSpecialGround);

        for (int index = 0; index < _spawnPoints.Count; index++)
        {
            Transform point = _spawnPoints[index];

            List<Node> ListNodeA = MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(index);
            List<Node> ListNodeB = MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(index + 1);
            if (ListNodeA.Count == 0 || ListNodeB.Count == 0)
                continue;

            Node NodeA = ListNodeA[0];
            Node NodeB = ListNodeB[0];

            if (NodeA.Hauteur != NodeB.Hauteur || ListNodeA.Count != 1 || ListNodeB.Count != 1)
            {
                GameObject item = Instantiate(_spawnGround, point.position, point.rotation, _parent);
                item.transform.localScale = new Vector3(4, 4, 4);
                SetupObject(item, point, false, index);
            }
            else
            {
                float chance = Random.value;
                if (chance <= _spawnProbability)
                {
                    int randomIndex = Random.Range(0, spawnSpecialGroundClone.Count);
                    GameObject item = Instantiate(spawnSpecialGroundClone[randomIndex], point.position, point.rotation, _parent);
                    item.transform.localScale = new Vector3(4, 4, 4);
                    SetupObject(item, point, true, index);
                }
                else
                {
                    GameObject item = Instantiate(_spawnGround, point.position, point.rotation, _parent);
                    item.transform.localScale = new Vector3(4, 4, 4);
                    SetupObject(item, point, false, index);
                }
            }
        }
        SpawnRandomItem.Instance.SpawnItemOnGround();
    }

    public void SetupObject(GameObject obj, Transform point, bool IsSpecial, int index)
    {
        obj.transform.position = point.position;
        GroundList.Add(obj);
        if (IsSpecial)
        {
            _imposibleGround = obj;
        }
        if (index < PlayerMap.Instance.PositionMap - 1 || index > PlayerMap.Instance.PositionMap + 3)
        {
            obj.SetActive(false);
        }
    }
}
