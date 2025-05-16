using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script qui Setup le decor de la map
/// </summary>
public class SpawnDecorMap2 : MonoBehaviour
{
    [SerializeField] private List<GameObject> _mapBorderDecor = new(); //5 = haut
    [SerializeField] private List<GameObject> _mapListSecondary = new();
    private Queue<GameObject> _activeDecor = new();
    private void Start()
    {
        ActiveSecondaryDecor();
        SpawnDecor();
    }

    public void ActiveSecondaryDecor()
    {
        if (!MapMaker2.Instance.DicoNode.ContainsKey(new Vector3Int(250,0,0))) { _mapListSecondary[1].SetActive(true); }
        else if (!MapMaker2.Instance.DicoNode.ContainsKey(new Vector3Int(-250, 0, 0))) { _mapListSecondary[0].SetActive(true); }
        if (!MapMaker2.Instance.DicoNode.ContainsKey(new Vector3Int(1750, 0, 0))) { _mapListSecondary[2].SetActive(true); }
        if ( !MapMaker2.Instance.DicoNode.ContainsKey(new Vector3Int(3750, 0, 0))) { _mapListSecondary[3].SetActive(true); }
        if (!MapMaker2.Instance.DicoNode.ContainsKey(new Vector3Int(4250, 0, 0))) { _mapListSecondary[5].SetActive(true); }
        else { _mapListSecondary[4].SetActive(false); }
    }

    public void SpawnDecor()
    {
        if (PlayerMap.Instance.PositionMap >= 0) { _activeDecor.Enqueue(_mapBorderDecor[0]); _activeDecor.Enqueue(_mapBorderDecor[1]); }
        if (PlayerMap.Instance.PositionMap >= 1) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapBorderDecor[2]); }
        if (PlayerMap.Instance.PositionMap >= 3) { _activeDecor.Enqueue(_mapBorderDecor[3]); }
        if (PlayerMap.Instance.PositionMap >= 4) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapBorderDecor[4]); }
        if (PlayerMap.Instance.PositionMap >= 6) { _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapBorderDecor[5]); }
        if (PlayerMap.Instance.PositionMap >= 7) { _activeDecor.Enqueue(_mapBorderDecor[6]); }
        if (PlayerMap.Instance.PositionMap >= 8) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap >= 9) { _activeDecor.Dequeue(); _activeDecor.Dequeue(); _activeDecor.Enqueue(_mapBorderDecor[7]); }
        if (PlayerMap.Instance.PositionMap >= 10) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap >= 10) { _activeDecor.Enqueue(_mapBorderDecor[8]); }
        if (PlayerMap.Instance.PositionMap >= 14) { _activeDecor.Dequeue(); }

        LoadDecor();
    }

    public void LoadDecor()
    {
        foreach (GameObject go in _mapBorderDecor) { go.SetActive(false); }
        foreach (GameObject go in _activeDecor) { go.SetActive(true); }
    }
}
