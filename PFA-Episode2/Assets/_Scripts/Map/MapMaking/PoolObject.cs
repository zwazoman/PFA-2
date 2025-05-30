using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectToDuplicate = new();
    public Queue<GameObject> CusineList { get; private set; } = new();
    public Queue<GameObject> IngredientList { get; private set; } = new();
    public Queue<GameObject> HealList { get; private set; } = new();
    public Queue<GameObject> CombatList { get; private set; } = new();

    public static PoolObject Instance;


    private void Awake() { Instance = this; }
    private void Start() { Pool(); }
    public void Pool()
    {
        for (int index = 0; index < _objectToDuplicate.Count; index++)
        {
            GameObject obj = _objectToDuplicate[index];
            for (int i = 0; i < 15; i++)
            {
                GameObject test = Instantiate(obj, gameObject.transform);
                test.SetActive(false);
                switch(index)
                {
                    case 0:
                        CombatList.Enqueue(test);
                        break;
                    case 1:
                        CusineList.Enqueue(test);
                        break;
                    case 2:
                        HealList.Enqueue(test);
                        break;
                    case 3:
                        IngredientList.Enqueue(test);
                        break;
                }
            }
        }
    }
}
