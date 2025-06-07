using UnityEngine;
using System;
using UnityEngine.UI;
using static NodeTypes;
using System.Collections.Generic;
using DG.Tweening;

public class Node : MonoBehaviour
{
    public static event Action OnMapCompleted;
    public int Position;
    public int Hauteur;
    public Node Creator;
    public NodesEventTypes EventName;
    public bool OnYReviendra;
    private GameObject _mesh;
    public bool Visited;
    public bool Intersection;
    public List<Node> Children { get; private set; } = new List<Node>();
    public List<GameObject> PathBetweenNode = new();
    [SerializeField] public string CombatScene;
    [SerializeField] private GameObject _parentGO;
    [SerializeField] private GameObject _prefabBoss;
    [SerializeField] private Button _button;
    [SerializeField] private MeshRenderer _meshRend;
    [SerializeField] private Material _mat;
    [SerializeField] private GameObject _halo;

    public static void TriggerMapCompleted() { OnMapCompleted?.Invoke(); }

    public void InteractPlayer()
    {
        Visited = true;
        PlayerMap.Instance.clickedNode = this;
        TweenMesh();
    }

    /// <summary>
    /// Fonction qui peut setup un sprite pour le node selon son rôle
    /// </summary>
    public void SetupSprite()
    {
        _halo.SetActive(false);
        if (PlayerMap.Instance.PositionMap == Position) { return; }
        switch (EventName)
        {
            case NodesEventTypes.Cuisine:
                GameObject CuisinePrefab = PoolObject.Instance.CusineList.Dequeue();
                CuisinePrefab.transform.position = _parentGO.transform.position;
                CuisinePrefab.transform.SetParent(gameObject.transform);
                CuisinePrefab.SetActive(true);
                _mesh = CuisinePrefab;
                break;
            case NodesEventTypes.Combat:
                GameObject CombatPrefab = PoolObject.Instance.CombatList.Dequeue();
                CombatPrefab.transform.position = _parentGO.transform.position;
                CombatPrefab.transform.SetParent(gameObject.transform);
                CombatPrefab.SetActive(true);
                _mesh = CombatPrefab;
                _meshRend.material = _mat;
                CombatScene = GameManager.Instance.GetRandomCombatScene();
                break;
            case NodesEventTypes.Ingredient:
                GameObject IngredientPrefab = PoolObject.Instance.IngredientList.Dequeue();
                IngredientPrefab.transform.position = _parentGO.transform.position;
                IngredientPrefab.transform.parent = gameObject.transform;
                IngredientPrefab.SetActive(true);
                _mesh = IngredientPrefab;
                break;
            case NodesEventTypes.Heal:
                GameObject HealPrefab = PoolObject.Instance.HealList.Dequeue();
                HealPrefab.transform.position = _parentGO.transform.position;
                HealPrefab.transform.SetParent(gameObject.transform);
                HealPrefab.SetActive(true);
                _mesh = HealPrefab;
                break;
            case NodesEventTypes.Boss:
                GameObject go = Instantiate(_prefabBoss);
                go.transform.position = _parentGO.transform.position;
                go.transform.SetParent(gameObject.transform);
                go.SetActive(true);
                _mesh = go;
                break;
            case NodesEventTypes.Start:
                break;
        }
        if ((PlayerMap.Instance.PositionMap == Position - 1 && PlayerMap.Instance.Y == gameObject.transform.localPosition.y) || (PlayerMap.Instance.PositionMap == Position - 1 && Intersection )) 
        {
            _button.interactable = true; _halo.SetActive(true);
        }
        else { _button.interactable = false; }
        Vector3 rot = transform.eulerAngles;
        rot.z = -90f;
        transform.eulerAngles = rot;
        if (PathBetweenNode.Count > 1) { if (Hauteur != 3) { Destroy(PathBetweenNode[1]); } }
    }

    private void TweenMesh()
    {
        _halo.transform.SetParent(_mesh.transform);
        _mesh.transform.DOScale(new Vector3(0,0,0), 0.5f).SetEase(Ease.InBack);
    }

    public bool GetInteractable()
    {
        return _button.interactable;
    }
}
