using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [SerializeField] SerializedDictionary<GameObject, float> entitiesProba = new();

    WayPoint spawnPoint;
    float totalProba;

    private void Awake()
    {
        foreach (GameObject entity in entitiesProba.Keys)
            totalProba += entitiesProba[entity];


        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit) && hit.collider.TryGetComponent(out spawnPoint))
            return;
    }

    public async void SummonEntity()
    {
        //if(entitiesProba.Count == 1)
        //    foreach(GameObject entity in entitiesProba.Keys)
        //    {
        //        Instantiate(entity, transform);
        //        print("connard");
        //    }

        float randomProba = Random.Range(0, totalProba);

        List<float> probas = new();

        foreach(GameObject entity in entitiesProba.Keys)
            probas.Add(entitiesProba[entity]);

        probas.Sort();

        float delta = Mathf.Infinity;
        GameObject choosenEntity = null;

        foreach(float proba in probas)
        {
            if(delta > Mathf.Abs(randomProba - proba))
            {
                delta = proba;
                choosenEntity = entitiesProba.GetKeyFromValue(proba);
            }
        }

        GameObject go = Instantiate(choosenEntity, transform);
    }
}
