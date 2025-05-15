using System.Collections.Generic;
using UnityEngine;

public class SpawnDecorMap2 : MonoBehaviour
{
    [SerializeField] private List<GameObject> _mapListDecor = new();
    private Queue<GameObject> _activeDecor = new(); //A save
    private void Start()
    {
        SpawnDecor();
    }

    public void SpawnDecor()
    {
        if (PlayerMap.Instance.PositionMap >= 0) { _activeDecor.Enqueue(_mapListDecor[0]); _activeDecor.Enqueue(_mapListDecor[1]); }
        if (PlayerMap.Instance.PositionMap >= 1) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapListDecor[2]); }
        if (PlayerMap.Instance.PositionMap >= 3) { _activeDecor.Enqueue(_mapListDecor[3]); }
        if (PlayerMap.Instance.PositionMap >= 4) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapListDecor[4]); }
        if (PlayerMap.Instance.PositionMap >= 6) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapListDecor[5]); }
        if (PlayerMap.Instance.PositionMap >= 7) { _activeDecor.Enqueue(_mapListDecor[6]); }
        if (PlayerMap.Instance.PositionMap >= 8) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap >= 9) { _activeDecor.Dequeue(); _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapListDecor[7]); }
        if (PlayerMap.Instance.PositionMap >= 10) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap >= 10) { _activeDecor.Enqueue(_mapListDecor[8]); }
        if (PlayerMap.Instance.PositionMap >= 14) { _activeDecor.Dequeue(); }

        LoadDecor();
    }

    public void LoadDecor()
    {
        foreach (GameObject go in _mapListDecor) { go.SetActive(false); }
        foreach (GameObject go in _activeDecor) { go.SetActive(true); }
    }

    public void test()
    {
        PlayerMap.Instance.PositionMap++;
        SpawnDecor();
    }
}
