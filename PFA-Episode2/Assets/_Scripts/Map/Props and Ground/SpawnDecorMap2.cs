using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère l’activation des décors en fonction de la progression de la map.
/// </summary>
public class SpawnDecorMap2 : MonoBehaviour
{
    [SerializeField] private List<GameObject> _mapBorderDecor = new();
    private readonly List<int> _activeIndices = new();

    private void Start()
    {
        SpawnDecor();
    }

    public void SpawnDecor()
    {
        _activeIndices.Clear();

        int pos = PlayerMap.Instance.PositionMap;

        if (pos >= 0) _activeIndices.AddRange(new[] { 0, 1 });
        if (pos >= 2) ReplaceLast(2);
        if (pos >= 3) _activeIndices.Add(3);
        if (pos >= 4) ReplaceLast(4);
        if (pos >= 6) ReplaceLast(5);
        if (pos >= 7) _activeIndices.Add(6);
        if (pos >= 8) RemoveLast();
        if (pos >= 9) { RemoveLast(2); _activeIndices.Add(7); }
        if (pos >= 10) ReplaceLast(8);
        //if (pos >= 14) RemoveLast();

        LoadDecor();
    }

    private void LoadDecor()
    {
        foreach (var go in _mapBorderDecor) { go.SetActive(false); }

        foreach (int index in _activeIndices)
        {
            if (index >= 0 && index < _mapBorderDecor.Count)
                _mapBorderDecor[index].SetActive(true);
        }
    }

    private void ReplaceLast(int newIndex)
    {
        if (_activeIndices.Count > 0)
            _activeIndices[_activeIndices.Count - 1] = newIndex;
        else
            _activeIndices.Add(newIndex);
    }

    private void RemoveLast(int count = 1)
    {
        for (int i = 0; i < count && _activeIndices.Count > 0; i++)
            _activeIndices.RemoveAt(_activeIndices.Count - 1);
    }
}
