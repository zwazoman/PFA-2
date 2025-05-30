using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomSize : MonoBehaviour
{
    [SerializeField] private List<GameObject> _propsList = new();
    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;

    [SerializeField] private Material _material;

    private void Start() { GeneratePropsSize(); }
    private void GeneratePropsSize()
    {
        foreach (GameObject go in _propsList)
        {
            float result = Random.Range(_minSize, _maxSize + 0.1f);
            go.transform.localScale = new Vector3(result, result, result);
            go.GetComponent<MeshRenderer>().material = _material;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RandomSize))]
    class RandomSizeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test Size"))
            {
                ((RandomSize)target).GeneratePropsSize();
            }
        }
    }
}
#endif