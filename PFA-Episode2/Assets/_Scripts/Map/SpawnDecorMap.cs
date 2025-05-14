using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script qui doit faire spawn le décor petit à petit (2-3 cases à l'avance)
/// </summary>
public class SpawnDecorMap : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPoint = new();
    [SerializeField] private List<GameObject> _prefabList = new(); // 0 = I, 1 ES, 2 GP, 3 DP, 4 TDOS
    [SerializeField] private GameObject _boss;
    [SerializeField] private GameObject _start;
    [SerializeField] private GameObject _droit;
    [SerializeField] private GameObject _intersection;
    [SerializeField] private Transform _parent;
    private int _indexSpawn = 0;
    private Queue<GameObject> _activeDecor = new(); // A SAVE

    private void Start()
    {
        _activeDecor.Enqueue(_start);
        _activeDecor.Enqueue(_droit);
    }

    public void SetupDecor()//appelé quand Start mais il faut être sur que tout soit fini (liste) sinon erreur   // ATTENTION IL PEUT Y AVOIR 2 SPAWN DE PREFAB DANS LE MEME TOUR
    {
        //Les 1er sont actif au début
        if (PlayerMap.Instance.PositionMap == 0)
        {
            _activeDecor.Enqueue(_start);
            _activeDecor.Enqueue(_droit);
            _activeDecor.Enqueue(_intersection);
        }
        if (PlayerMap.Instance.PositionMap == 2) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap == 3) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap == 5) { _activeDecor.Dequeue(); }
        if (PlayerMap.Instance.PositionMap == MapMaker2.Instance.MapRange - 5) { _activeDecor.Enqueue(_boss); }
        if (PlayerMap.Instance.PositionMap == MapMaker2.Instance.MapRange - 3) { _activeDecor.Clear(); _activeDecor.Enqueue(_boss); } //Au cas ou, normalement useless
        foreach (Node node in MapMaker2.Instance.AllNodeGood) //Coeur de la foret
        {
            if(MapBuildingTools.Instance.ReturnNodeFromNodePosition(node.Position, 1).Intersection && MapBuildingTools.Instance.ReturnNodeFromNodePosition(node.Position, 1).Hauteur != 3)
            {
                GameObject go = Instantiate(_prefabList[0], _parent);
                go.transform.position = _spawnPoint[_indexSpawn].position; //le placer
                _activeDecor.Enqueue(go);
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
                    else if (MapBuildingTools.Instance.ReturnListOfNodeFromNodePosition(node.Position).Count >= 2)
                    {
                        /*ES*/
                    }
                    else
                    {
                        if (node.Hauteur < 3)
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
        _indexSpawn++;
        LoadDecor();
    }

    public void LoadDecor()
    {
        foreach(GameObject go in _activeDecor)
        {
            go.SetActive(true);
        }
    }
}
