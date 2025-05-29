using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class SpawnRandomItem : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> _spawnItems = new();
    [SerializeField, Range(0f, 1f)] private float _spawnProbability = 0.5f;
    private List<GameObject> _spawnedItems = new();

    [Header("Others")]
    [SerializeField] private Transform _parent;

    public static SpawnRandomItem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnItemOnGround()
    {
        //foreach (GameObject ground in SpawnRiver.Instance.GroundList)
        //{
        //    if (ground.activeSelf)
        //    {
        //        SpawnZone spawnZone = ground.GetComponent<SpawnZone>();
        //        if (spawnZone == null) continue;

        //        foreach (Vector3 position in spawnZone.PositionForSpawn)
        //        {
        //            if (Random.value > _spawnProbability) continue;

        //            int index = Random.Range(0, _spawnItems.Count);
        //            GameObject item = Instantiate(_spawnItems[index], _parent);
        //            item.transform.position = position;
        //            _spawnedItems.Add(item);
        //        }
        //    }
        //}
    }
}
