using System.Collections.Generic;
using UnityEngine;
using static NodeTypes;

/// <summary>
/// Script qui construit la carte du jeu
/// </summary>
[RequireComponent(typeof(MapAttributeEvent))]
[RequireComponent(typeof(MapBuildingTools))]
public class MapMaker2 : MonoBehaviour
{
    #region Variables
    public static MapMaker2 Instance;

    [Header("Map Adjusting")]
    [SerializeField][Range(4, 15)][Tooltip("Le nombre de node minimum entre le Node de d�part et le boss")] public int MapRange;
    [SerializeField][Tooltip("Distance � laquelle le node va spawn sur l'axe X")] private int _distanceSpawnX = 200;
    [SerializeField][Tooltip("Distance � laquelle le node va spawn sur l'axe Y")] private int _distanceSpawnY = 0;
    [SerializeField][Tooltip("Position en X � laquelle le 1er Node spawn (Le mieux : -1045)")] private int _firstNodePosition = -1045;

    [Header("Probality")]
    [SerializeField][Tooltip("Probabiltit� � chaque node d'avoir une intersection (0 = impossible)")][Range(0, 100)] private int _probaIntersection = 3;
    [SerializeField][Tooltip("Probabiltit� d'avoir un tout droit lors d'un croisement")][Range(0, 100)] private int _probaToutDroitCroisement = 5;

    private int _distanceSpawnYModifiable = 0;
    private int _probaIntersectionModifiable;
    private int _toutdroit = 3;

    [Header("Other ne pas toucher sauf code")]
    [field: SerializeField] public Node _nodePrefab { get; private set; }
    [SerializeField] private Node _parentNode;
    public Node CurrentNode { get; private set; }

    /// <summary>
    /// Queue de node cr�e au d�but du jeu (environ 40)
    /// </summary>
    private Queue<Node> _nodeList = new();
    public List<Node> Intersection { get; private set; } = new();  //Liste des nodes qui vont devoir continuer � cr�e un chemin � partir d'eux
    public Dictionary<Vector3Int, Node> _dicoNode { get; set; } = new(); //ToDo :Faire en sorte qu'il soit priv� sauf pour la save.
    private int _currentHeight = 3;
    private Node _existingValue;
    #endregion
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i <= 40; i++) //Cr�ation de plein de node que on placera plus tard
        {
            Node NewNode = Instantiate(_nodePrefab, gameObject.transform);
            NewNode.transform.localPosition = _parentNode.transform.localPosition;
            _nodeList.Enqueue(NewNode);
        }
    }

    private void Start()
    {
        _probaIntersectionModifiable = _probaIntersection;
        MapMaking(1);
        ConstructionSecondaireGraph();
        Node.TriggerMapCompleted(); //Attribution des r�les
    }

    public void MapMaking(int StartPosition)
    {
        Vector3Int startPos = new Vector3Int(_firstNodePosition, 0, 0); // Enregistre le node de d�part
        _parentNode.transform.localPosition = startPos;
        _dicoNode.Add(startPos, _parentNode);

        for (int i = StartPosition; i <= MapRange; i++)
        {
            _currentHeight = _parentNode.Hauteur;
            CurrentNode = _nodeList.Dequeue();
            #region BossVerif
            if (_parentNode.Position >= MapRange - 2) //zone faut revenir au Boss
            {
                if (_currentHeight != 3)
                {
                    if (_currentHeight > 3)
                    {
                        CreateBranch(i, false);
                    }
                    else
                    {
                        CreateBranch(i, true);
                    }
                }
                else { ToutDroit(i, _parentNode); }
            }
            else if (MapBuildingTools.Instance.Intersection(i, _probaIntersection))
            {
                CreateBranch(i);
            }
            else { ToutDroit(i, _parentNode); }
            #endregion
            MapBuildingTools.Instance.AttributeEvent(MapRange);
            _parentNode = CurrentNode;
        }
    }

    public void CreateBranch(int tourboucle)
    {
        if (_currentHeight + 1 <= 4 && _currentHeight - 1 >= 2) //Si on peut monter et descendre
        {
            if (Random.Range(1, 110) <= _probaToutDroitCroisement) //Proba de faire un tout droit
            {
                _probaToutDroitCroisement = 0;
                CurrentNode = _nodeList.Dequeue();
                CurrentNode.OnYReviendra = true;
                Intersection.Add(CurrentNode);
                ToutDroit(tourboucle, _parentNode);
                CurrentNode.Hauteur = _currentHeight;
            }

            CurrentNode = _nodeList.Dequeue();
            _distanceSpawnYModifiable = _distanceSpawnY;
            CurrentNode.OnYReviendra = true;
            Intersection.Add(CurrentNode);
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
            print(CurrentNode.Hauteur);

            CurrentNode = _nodeList.Dequeue();
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight - 1;
        }
        else if (_currentHeight + 1 <= 4) //Si on peut monter
        {
            _distanceSpawnYModifiable = _distanceSpawnY;
            CurrentNode.OnYReviendra = true;
            Intersection.Add(CurrentNode);
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
        }
        else if (_currentHeight - 1 >= 2) //Si on peut descendre
        {
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight - 1;
        }
        _probaIntersection = 0;
        _distanceSpawnYModifiable = 0;
    }
    public void ToutDroit(int tourboucle, Node NodeForY)
    {
        int maxToutDroit = Random.Range(1, 3); // en bas
        if (_toutdroit >= maxToutDroit) { _probaIntersection = _probaIntersectionModifiable; }
        //On le place et on arrondit pour le dicooo
        Vector3Int newPosition = new Vector3Int((_distanceSpawnX * tourboucle) + Mathf.RoundToInt(CurrentNode.transform.localPosition.x), Mathf.RoundToInt(NodeForY.transform.localPosition.y) + _distanceSpawnYModifiable, Mathf.RoundToInt(CurrentNode.transform.localPosition.z));
        if (_dicoNode.ContainsKey(newPosition))
        {
            _existingValue = _dicoNode[newPosition];
            print("Un node est d�ja pr�sent ici" + _existingValue);
            _nodeList.Enqueue(CurrentNode);
        }
        else
        {
            CurrentNode.transform.localPosition = newPosition;
            CurrentNode.gameObject.SetActive(true);
            CurrentNode.Hauteur = _currentHeight;
            _dicoNode.Add(newPosition, CurrentNode);
            MapBuildingTools.Instance.TraceTonTrait(_parentNode, CurrentNode);
            CurrentNode.Creator = _parentNode;
            CurrentNode.Position = tourboucle;
        }
        _toutdroit++;
    }

    public void CreateBranch(int tourboucle, bool Up)
    {
        if (Up) //Si on peut monter
        {
            _distanceSpawnYModifiable = _distanceSpawnY;
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
        }
        else //Si on peut descendre
        {
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, _parentNode);
            CurrentNode.Hauteur = _currentHeight - 1;
        }
        _distanceSpawnYModifiable = 0;
    }

    public void ConstructionSecondaireGraph()
    {
        _toutdroit = 0;
        int maxToutDroit = Random.Range(1, 5); //en haut
        foreach (Node node in Intersection)
        {
            _parentNode = node;
            while (true)
            {
                _currentHeight = _parentNode.Hauteur;
                CurrentNode = _nodeList.Dequeue();
                int tour = _parentNode.Position + 1;

                // Calcul de la position potentielle
                Vector3Int nextPosition = new Vector3Int((_distanceSpawnX * tour) + Mathf.RoundToInt(CurrentNode.transform.localPosition.x), Mathf.RoundToInt(_parentNode.transform.localPosition.y), Mathf.RoundToInt(CurrentNode.transform.localPosition.z));

                if (_dicoNode.ContainsKey(nextPosition))
                {
                    Node nodeExistant = _dicoNode[nextPosition];

                    MapBuildingTools.Instance.TraceTonTrait(_parentNode, nodeExistant);// Trace une ligne entre le parent actuel et le node d�j� existant
                    nodeExistant.Creator = _parentNode;

                    _nodeList.Enqueue(CurrentNode);
                    break;
                }
                if (_parentNode.Position >= MapRange - 2) //Zone du boss
                {
                    if (_currentHeight != 3)
                    {
                        if (_currentHeight > 3)
                        {
                            CreateBranch(tour, false); // descendre
                            MapBuildingTools.Instance.TraceTonTrait(_parentNode, _existingValue);
                        }
                        else
                        {
                            CreateBranch(tour, true); // monter
                            MapBuildingTools.Instance.TraceTonTrait(_parentNode, _existingValue);
                        }
                    }
                }
                else if (_toutdroit >= maxToutDroit && _currentHeight > 3)
                {
                    CreateBranch(tour, false); // descendre
                    if (_existingValue != null)
                    {
                        MapBuildingTools.Instance.TraceTonTrait(_parentNode, _existingValue);
                        _toutdroit = 0;
                    }
                }
                else if (_toutdroit >= maxToutDroit && _currentHeight < 3)
                {
                    CreateBranch(tour, true); // monter
                }
                else
                {
                    ToutDroit(tour, _parentNode);
                }
                MapBuildingTools.Instance.AttributeEvent(MapRange);
                _parentNode = CurrentNode;
            }
        }
        MapBuildingTools.Instance.AttributeEvent(MapRange);
    }
}