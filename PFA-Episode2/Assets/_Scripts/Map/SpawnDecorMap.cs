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
        if (PlayerMap.Instance.PositionMap == 0) { /*C'est Start*/ /*C'est Droit*/}
        if (PlayerMap.Instance.PositionMap == 1) { /*C'est Intersection et c'est plus Start*/}
        if (PlayerMap.Instance.PositionMap == 3) { /*c'est plus droit*/}
        if (PlayerMap.Instance.PositionMap == 5) { /*c'est plus Intersection*/}
        if (PlayerMap.Instance.PositionMap == MapMaker2.Instance.MapRange - 5) { /*C'est le boss*/ }
        if (PlayerMap.Instance.PositionMap == MapMaker2.Instance.MapRange - 3) { /* Désactive tout sauf boss*/ }
        foreach (Node node in MapMaker2.Instance.AllNodeGood)
        {
            if(MapBuildingTools.Instance.ReturnNodeFromNodePosition(node.Position, 1).Intersection && MapBuildingTools.Instance.ReturnNodeFromNodePosition(node.Position, 1).Hauteur != 3)
            {
                /*Intersection*/
            }
            if(PlayerMap.Instance.PositionMap == node.Position)
            {
                if(node.Intersection)
                {
                    if (MapBuildingTools.Instance.ReturnNodeFromNodePosition(node.Position, 2).Hauteur == node.Hauteur) //Si son bool est true et que 2 node plus loin est à la même hauteur
                    {
                        /* tout droit */
                    }
                    else
                    {
                        if(node.Hauteur < 3) //il manque ES
                        {
                            /*DP*/
                        }
                        else
                        {
                            /* DG*/
                        }
                    }
                }
            }
        }
    }
}
