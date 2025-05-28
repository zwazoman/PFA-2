using UnityEngine;
using System.Collections.Generic;

public class SpawnRandomItem : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> _spawnItems = new();
    [SerializeField] private List<Transform> _spawnPoints = new();
    [SerializeField][Range(0f, 1f)] private float _spawnProbability = 0.5f;
    private List<GameObject> _spawnedItems = new();

    [Header("Others")]
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _player;

    void SpawnItemOnGround()
    {
        if (_spawnItems.Count == 0 || _spawnPoints.Count == 0)
        {
            Debug.LogWarning("Pas d’objets ou de points.");
            return;
        }
    }


}

