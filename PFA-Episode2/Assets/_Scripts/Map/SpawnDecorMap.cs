using System.Collections.Generic;
using UnityEngine;

public class SpawnDecorMap : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoint = new();
    [SerializeField] private List<GameObject> _prefabList = new(); // 0 = I, 1 ES, 2 GP, 3 DP, 4 TDOS
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _start;
    [SerializeField] private GameObject _droit;
    private Queue<int> _caseToMoveMapDecor = new();
    private Queue<GameObject> _activeDecor = new();

    private void Start()
    {
        _caseToMoveMapDecor.Enqueue(1);
        _caseToMoveMapDecor.Enqueue(3);
        _activeDecor.Enqueue(_start);
        _activeDecor.Enqueue(_droit);
    }

    public void SetupDecor()
    {
        if(PlayerMap.Instance.PositionMap == _caseToMoveMapDecor.Dequeue()) //Je vais utiliser Intersection dans les nodes pour savoir si I doit spawn ou si c'est on revieng (gd gp)
        {
            //Un changement
        }
        foreach(Node node in MapMaker2.Instance.AllNodeGood)
        {
            if(node.Position == 0) { /*C'est Start*/}
        }
    }
}
