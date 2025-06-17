using UnityEngine;
using System;
using UnityEngine.UI;
using static NodeTypes;
using System.Collections.Generic;
using DG.Tweening;

public class Node : MonoBehaviour
{
    public static event Action OnMapCompleted;

    [Header ("Settings Node")]
    public int Position; // de 0 à MapRange
    public int Hauteur; // de 2 à 4
    public Node Creator; // son parent
    public List<Node> Children = new();
    public NodesEventTypes EventName;
    public List<GameObject> PathBetweenNode = new();
    public bool NodeExisting;

    [HideInInspector] public bool OnYReviendra;
    [HideInInspector] public string CombatScene;
    [SerializeField] private GameObject _parentGO;
    [SerializeField] private GameObject _prefabBoss;
    [SerializeField] private Button _button;
    [SerializeField] private MeshRenderer _meshRend;
    [SerializeField] private Material _mat;
    [SerializeField] private GameObject _halo;
    private GameObject _mesh;

    public static void TriggerMapCompleted() { OnMapCompleted?.Invoke(); }

    public void InteractPlayer()
    {
        PlayerMap.Instance.clickedNode = this;
        SFXManager.Instance.PlaySFXClip(Sounds.ButtonPress);
        TweenMesh();
    }

    /// <summary>
    /// Fonction qui peut setup un sprite pour le node selon son rôle
    /// </summary>
    public void SetupSprite()
    {
        if (PlayerMap.Instance.PositionMap != Position) 
        {
            GameObject prefab = null;
            switch (EventName)
            {
                case NodesEventTypes.Cuisine:
                    prefab = PoolObject.Instance.CusineList.Dequeue();
                    break;
                case NodesEventTypes.Combat:
                    prefab = PoolObject.Instance.CombatList.Dequeue();
                    break;
                case NodesEventTypes.Ingredient:
                    prefab = PoolObject.Instance.IngredientList.Dequeue();
                    break;
                case NodesEventTypes.Heal:
                    prefab = PoolObject.Instance.HealList.Dequeue();
                    break;
                case NodesEventTypes.Boss:
                    prefab = Instantiate(_prefabBoss);
                    break;
                case NodesEventTypes.Tuto:
                    prefab = PoolObject.Instance.TutoMesh;
                    break;
                case NodesEventTypes.Start:
                    return;
            }

            if (prefab != null)
            {
                prefab.transform.position = _parentGO.transform.position;
                prefab.transform.SetParent(transform);
                prefab.SetActive(true);
                _mesh = prefab;

                if (EventName == NodesEventTypes.Combat)
                {
                    _meshRend.material = _mat;
                    CombatScene = GameManager.Instance.GetRandomCombatScene();
                }
            }
        }
        if (Creator == null ) { return; }
        if (PlayerMap.Instance.PositionMap == 0 && Position == 1)
        {
            _button.interactable = true;
            _halo.SetActive(true);
        }
        else if (Position == PlayerMap.Instance.PositionMap && gameObject.transform.position.y == PlayerMap.Instance.gameObject.transform.position.y)
        {
            foreach (Node node in Children)
            {
                node._button.interactable = true;
                node._halo.SetActive(true);
                _button.interactable = false;
                _halo.SetActive(false);
            }
        }
        else if (Creator.Position != PlayerMap.Instance.PositionMap)
        {
            _button.interactable = false;
            _halo.SetActive(false);
        }

        Vector3 rot = transform.eulerAngles;
        rot.z = -90f;
        transform.eulerAngles = rot;
        //if (PathBetweenNode.Count > 1) { if (Hauteur != 3 && !Children[0].Children[0].NodeExisting) { Destroy(PathBetweenNode[1]); } }
    }

    private void TweenMesh()
    {
        _halo.transform.SetParent(_mesh.transform);
        _mesh.transform.DOScale(new Vector3(0,0,0), 0.5f).SetEase(Ease.InBack);
    }

    public bool GetInteractable() { return _button.interactable; }
}
