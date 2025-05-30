using System.Collections.Generic;
using UnityEngine;

public class SpawnTree : MonoBehaviour
{
    [SerializeField] private List<MeshFilter> _listTree;
    [SerializeField] private List<MeshFilter> _listRandomMesh;

    void Start()
    {
        foreach(MeshFilter msh in _listTree)
        {
            int meshChoose = Random.Range(0, _listRandomMesh.Count);
            msh.mesh = _listRandomMesh[meshChoose].mesh;
        }
    }
}
