using UnityEngine;

public class RandomSize : MonoBehaviour
{
    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;

    [SerializeField] private Material _material1;
    [SerializeField] private Material _material2;

    private void Start()
    {
        float result = Random.Range(_minSize, _maxSize + 0.1f);
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y, result);
        int RandomMaterial = Random.Range(0, 2);
        if (RandomMaterial == 1) { gameObject.GetComponent<MeshRenderer>().material = _material1; }
        else { gameObject.GetComponent<MeshRenderer>().material = _material2; }
    }
}
