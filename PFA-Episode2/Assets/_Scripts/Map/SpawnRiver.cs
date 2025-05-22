using System.Collections.Generic;
using UnityEngine;

public class SpawnRiver : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoints = new();
    [SerializeField] private Transform _parent;
    [SerializeField] private GameObject _spawnGround = new();
    [SerializeField] private List<GameObject> _spawnSpecialGround = new();
    private GameObject _imposibleGround;
    [SerializeField] private List<GameObject> _groundList = new();
    [SerializeField][Range(0f, 1f)] private float _spawnProbability = 0.5f;

    public void StartSpawnRiver()
    {
        if (_spawnSpecialGround.Count == 0)
        {
            print("Pas d’objets ou de points.");
            return;
        }

        foreach (Transform point in _spawnPoints) //On doit trié
        {
            float chance = Random.value;
            if (chance <= _spawnProbability)
            {
                int randomIndex = Random.Range(0, _spawnSpecialGround.Count);
                GameObject item = Instantiate(_spawnSpecialGround[randomIndex], _parent);
                item.transform.position = point.position;
                _groundList.Add(item);
            }
        }
    }
}
