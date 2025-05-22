using UnityEngine;
using System.Collections.Generic;

public class SpawnRandomItem : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> _spawnItems = new();
    [SerializeField] private List<Transform> _spawnPoints = new();
    [SerializeField][Range(0f, 1f)] private float _spawnProbability = 0.5f;
    [SerializeField] private List<GameObject> _spawnedItems = new();

    [Header("Others")]
    [SerializeField] private Transform _player;

    void Start()
    {
        SpawnAllItemsOnce();
    }

    void SpawnAllItemsOnce()
    {
        if (_spawnItems.Count == 0 || _spawnPoints.Count == 0)
        {
            Debug.LogWarning("Pas d’objets ou de points.");
            return;
        }

        foreach (Transform point in _spawnPoints)
        {
            float chance = Random.value;
            if (chance <= _spawnProbability)
            {
                int randomIndex = Random.Range(0, _spawnItems.Count);
                GameObject item = Instantiate(_spawnItems[randomIndex]);
                item.transform.position = point.position;
                //item.SetActive(false); // Désactivé au départ
                _spawnedItems.Add(item); // Tu dois garder une référence à ces objets
            }
        }
    }


}

