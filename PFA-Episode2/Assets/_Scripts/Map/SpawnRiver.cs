using System.Collections.Generic;
using UnityEngine;

public class SpawnRiver : MonoBehaviour
{
    [Header("Spawn Settings")]

    [SerializeField] private List<Transform> _spawnPoints = new();
    [SerializeField] private List<GameObject> _spawnSpecialGround = new();
    [SerializeField] private GameObject _spawnGround;
    [SerializeField][Range(0f, 1f)] private float _spawnProbability = 0.5f;
    [SerializeField] private List<GameObject> _groundList = new();

    [Header("Others")]

    [SerializeField] private Transform _parent;
    private GameObject _imposibleGround;

    #region Singleton
    public static SpawnRiver Instance;

    private void Awake() { Instance = this; }
    #endregion

    public void StartSpawnRiver() //Dans MapMaker2
    {
        for (int index = 0; index < _spawnPoints.Count; index++) //On doit trié
        {
            Transform point = _spawnPoints[index];

            List<Node> ListNodeA = MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(index);
            List<Node> ListNodeB = MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(index + 1);
            Node NodeA = ListNodeA[0];
            Node NodeB = ListNodeB[0];
            int NumberOfNodeA = ListNodeA.Count;
            int NumberOfNodeB = ListNodeB.Count;

            if (NodeA.Hauteur != NodeB.Hauteur || NumberOfNodeA != 1 || NumberOfNodeB != 1) //Ground
            {
                GameObject item = Instantiate(_spawnGround, _parent);
                SetupObject(item, point, false);
            }
            else
            {
                float chance = Random.value;
                if (chance <= _spawnProbability)
                {
                    int randomIndex = Random.Range(0, _spawnSpecialGround.Count);
                    GameObject item = Instantiate(_spawnSpecialGround[randomIndex], point.position, point.rotation, _parent);
                    SetupObject(item, point, true);
                }
                else
                {
                    GameObject item = Instantiate(_spawnGround, _parent);
                    SetupObject(item, point, false);
                }

            }
        }
    }

    public void SetupObject(GameObject obj, Transform point, bool IsSpecial)
    {
        obj.transform.position = point.position;
        _groundList.Add(obj);
        if (IsSpecial)
        {
            if (_imposibleGround != null) { _spawnSpecialGround.Add(_imposibleGround); }
            _spawnSpecialGround.Remove(obj);
            _imposibleGround = obj;
        }
    }
}
