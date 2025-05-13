using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
    [SerializeField][Range(4, 15)][Tooltip("Le nombre de node minimum entre le Node de départ et le boss")] public int MapRange;
    [SerializeField][Tooltip("Distance à laquelle le node va spawn sur l'axe X")] private int _distanceSpawnX;
    [SerializeField][Tooltip("Distance à laquelle le node va spawn sur l'axe Y")] private int _distanceSpawnY = 0;
    [SerializeField][Tooltip("Position en X à laquelle le 1er Node spawn (Le mieux : -1045)")] private int _firstNodePosition = -1045;

    [Header("Probality")]
    [SerializeField][Tooltip("Probabiltité à chaque node d'avoir une intersection (0 = impossible)")][Range(0, 100)] private int _probaIntersection = 3;
    [SerializeField][Tooltip("Probabiltité d'avoir un tout droit lors d'un croisement")][Range(0, 100)] private int _probaToutDroitCroisement = 5;

    private int _distanceSpawnYModifiable = 0;
    private int _probaIntersectionModifiable;
    private int _toutdroit = 3;

    [Header("Other ne pas toucher sauf code")]
    [field: SerializeField] public Node NodePrefab { get; private set; }
    [field: SerializeField] public Node ParentNode { get; private set; }
    public Node CurrentNode { get; private set; }

    /// <summary>
    /// Queue de node crée au début du jeu (environ 40)
    /// </summary>
    public Queue<Node> NodeList = new();
    public List<Node> Intersection { get; private set; } = new();  //Liste des nodes qui vont devoir continuer à crée un chemin à partir d'eux
    public Dictionary<Vector3Int, Node> DicoNode { get; set; } = new(); //ToDo :Faire en sorte qu'il soit privé sauf pour la save.
    private int _currentHeight = 3;
    private Node _existingValue;
    #endregion
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i <= 40; i++) //Création de plein de node que on placera plus tard
        {
            Node NewNode = Instantiate(NodePrefab, gameObject.transform);
            NewNode.transform.localPosition = ParentNode.transform.localPosition;
            NewNode.gameObject.SetActive(false);
            NodeList.Enqueue(NewNode);
        }
    }

    private void Start()
    {
        SaveMapGeneration.Instance.LoadMap();
    }

    public void StartGenerate()
    {
        _probaIntersectionModifiable = _probaIntersection;
        MapMaking(1);
        ConstructionSecondaireGraph();
        Node.TriggerMapCompleted(); //Attribution des rôles
    }

    public void MapMaking(int StartPosition)
    {
        Vector3Int startPos = new Vector3Int(_firstNodePosition, 0, 0); // Enregistre le node de départ
        ParentNode.transform.localPosition = startPos;
        DicoNode.Add(startPos, ParentNode);

        for (int i = StartPosition; i <= MapRange; i++)
        {
            _currentHeight = ParentNode.Hauteur;
            CurrentNode = NodeList.Dequeue();
            #region BossVerif
            if (ParentNode.Position >= MapRange - 2) //zone faut revenir au Boss
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
                else { ToutDroit(i, ParentNode); }
            }
            else if (MapBuildingTools.Instance.Intersection(i, _probaIntersection))
            {
                CreateBranch(i);
            }
            else { ToutDroit(i, ParentNode); }
            #endregion
            MapBuildingTools.Instance.AttributeEvent(MapRange);
            ParentNode = CurrentNode;
        }
    }

    public void CreateBranch(int tourboucle)
    {
        if (_currentHeight + 1 <= 4 && _currentHeight - 1 >= 2) //Si on peut monter et descendre
        {
            if (Random.Range(1, 110) <= _probaToutDroitCroisement) //Proba de faire un tout droit
            {
                _probaToutDroitCroisement = 0;
                CurrentNode = NodeList.Dequeue();
                CurrentNode.Intersection = true;
                CurrentNode.OnYReviendra = true;
                Intersection.Add(CurrentNode);
                ToutDroit(tourboucle, ParentNode);
                CurrentNode.Hauteur = _currentHeight;
            }

            CurrentNode = NodeList.Dequeue();
            CurrentNode.Intersection = true;
            _distanceSpawnYModifiable = _distanceSpawnY;
            CurrentNode.OnYReviendra = true;
            Intersection.Add(CurrentNode);
            ToutDroit(tourboucle, ParentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
            print(CurrentNode.Hauteur);

            CurrentNode = NodeList.Dequeue();
            CurrentNode.Intersection = true;
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, ParentNode);
            CurrentNode.Hauteur = _currentHeight - 1;
        }
        else if (_currentHeight + 1 <= 4) //Si on peut monter
        {
            _distanceSpawnYModifiable = _distanceSpawnY;
            CurrentNode.OnYReviendra = true;
            Intersection.Add(CurrentNode);
            ToutDroit(tourboucle, ParentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
        }
        else if (_currentHeight - 1 >= 2) //Si on peut descendre
        {
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, ParentNode);
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
        if (DicoNode.ContainsKey(newPosition))
        {
            _existingValue = DicoNode[newPosition];
            print("Un node est déja présent ici" + _existingValue);
            NodeList.Enqueue(CurrentNode);
        }
        else
        {
            CurrentNode.transform.localPosition = newPosition;
            CurrentNode.gameObject.SetActive(true);
            CurrentNode.Hauteur = _currentHeight;
            DicoNode.Add(newPosition, CurrentNode);
            MapBuildingTools.Instance.TraceTonTrait(ParentNode, CurrentNode);
            CurrentNode.Creator = ParentNode;
            CurrentNode.Position = tourboucle;
        }
        _toutdroit++;
    }

    public void CreateBranch(int tourboucle, bool Up)
    {
        CurrentNode.Intersection = true;
        if (Up) //Si on peut monter
        {
            _distanceSpawnYModifiable = _distanceSpawnY;
            ToutDroit(tourboucle, ParentNode);
            CurrentNode.Hauteur = _currentHeight + 1;
        }
        else //Si on peut descendre
        {
            _distanceSpawnYModifiable = -_distanceSpawnY;
            ToutDroit(tourboucle, ParentNode);
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
            ParentNode = node;
            while (true)
            {
                _currentHeight = ParentNode.Hauteur;
                CurrentNode = NodeList.Dequeue();
                int tour = ParentNode.Position + 1;

                // Calcul de la position potentielle
                Vector3Int nextPosition = new Vector3Int((_distanceSpawnX * tour) + Mathf.RoundToInt(CurrentNode.transform.localPosition.x), Mathf.RoundToInt(ParentNode.transform.localPosition.y), Mathf.RoundToInt(CurrentNode.transform.localPosition.z));

                if (DicoNode.ContainsKey(nextPosition))
                {
                    Node nodeExistant = DicoNode[nextPosition];

                    MapBuildingTools.Instance.TraceTonTrait(ParentNode, nodeExistant);// Trace une ligne entre le parent actuel et le node déjà existant
                    nodeExistant.Creator = ParentNode;

                    NodeList.Enqueue(CurrentNode);
                    break;
                }
                if (ParentNode.Position >= MapRange - 2) //Zone du boss
                {
                    if (_currentHeight != 3)
                    {
                        if (_currentHeight > 3)
                        {
                            CreateBranch(tour, false); // descendre
                            MapBuildingTools.Instance.TraceTonTrait(ParentNode, _existingValue);
                        }
                        else
                        {
                            CreateBranch(tour, true); // monter
                            MapBuildingTools.Instance.TraceTonTrait(ParentNode, _existingValue);
                        }
                    }
                }
                else if (_toutdroit >= maxToutDroit && _currentHeight > 3)
                {
                    CreateBranch(tour, false); // descendre
                    if (_existingValue != null)
                    {
                        MapBuildingTools.Instance.TraceTonTrait(ParentNode, _existingValue);
                        _toutdroit = 0;
                    }
                }
                else if (_toutdroit >= maxToutDroit && _currentHeight < 3)
                {
                    CreateBranch(tour, true); // monter
                }
                else
                {
                    ToutDroit(tour, ParentNode);
                }
                MapBuildingTools.Instance.AttributeEvent(MapRange);
                ParentNode = CurrentNode;
            }
        }
        foreach (Node node in DicoNode.Values)
        {
            if (node.OnYReviendra)
            {
                node.Intersection = true;
            }
        }
        MapBuildingTools.Instance.AttributeEvent(MapRange);
    }
}