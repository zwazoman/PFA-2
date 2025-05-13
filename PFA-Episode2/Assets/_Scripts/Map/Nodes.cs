using UnityEngine;
using System;
using UnityEngine.UI;
using static NodeTypes;

public class Node : MonoBehaviour
{
    public static event Action OnMapCompleted;
    public int Position;
    public int Hauteur;
    public Node Creator;
    public NodesEventTypes EventName;
    public bool OnYReviendra;
    [SerializeField] private GameObject _parentGO;
    [SerializeField] private GameObject _prefabBoss;
    [SerializeField] private Button _button;
    public bool Visited;

    public static void TriggerMapCompleted()
    {
        OnMapCompleted?.Invoke();
    }

    public void InteractPlayer()
    {
        Visited = true;
        PlayerMap.Instance.clickedNode = this;
    }

    private void OnEnable()
    {
        OnMapCompleted += SetupSprite;
    }

    private void OnDisable()
    {
        OnMapCompleted -= SetupSprite;
    }

    /// <summary>
    /// Fonction qui peut setup un sprite pour le node selon son rôle
    /// </summary>
    private void SetupSprite()
    {
        switch (EventName)
        {
            case NodesEventTypes.Cuisine:
                GameObject CuisinePrefab = PoolObject.Instance.CusineList.Dequeue();
                CuisinePrefab.transform.position = _parentGO.transform.position;
                CuisinePrefab.transform.SetParent(gameObject.transform);
                CuisinePrefab.SetActive(true);
                break;
            case NodesEventTypes.Combat:
                GameObject CombatPrefab = PoolObject.Instance.CombatList.Dequeue();
                CombatPrefab.transform.position = _parentGO.transform.position;
                CombatPrefab.transform.SetParent(gameObject.transform);
                CombatPrefab.SetActive(true);
                break;
            case NodesEventTypes.Ingredient:
                GameObject IngredientPrefab = PoolObject.Instance.IngredientList.Dequeue();
                IngredientPrefab.transform.position = _parentGO.transform.position;
                IngredientPrefab.transform.parent = gameObject.transform;
                IngredientPrefab.SetActive(true);
                break;
            case NodesEventTypes.Heal:
                GameObject HealPrefab = PoolObject.Instance.HealList.Dequeue();
                HealPrefab.transform.position = _parentGO.transform.position;
                HealPrefab.transform.SetParent(gameObject.transform);
                HealPrefab.SetActive(true);
                break;
            case NodesEventTypes.Boss:
                GameObject go = Instantiate(_prefabBoss);
                go.transform.position = _parentGO.transform.position;
                go.transform.SetParent(gameObject.transform);
                go.SetActive(true);
                break;
            case NodesEventTypes.Start:
                break;
        }
        if(PlayerMap.Instance.PositionMap == Position - 1) { _button.interactable = true; }
        else { _button.interactable = false; }
        Vector3 rot = transform.eulerAngles;
        rot.z = -90f;
        transform.eulerAngles = rot;

    }
}
