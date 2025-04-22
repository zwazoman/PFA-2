using System.Collections;
using System.Collections.Generic;
using mup;
using UnityEngine;

public class Seaside : MonoBehaviour
{
    [SerializeField] Transform _pointA;
    [SerializeField] Transform _pointB;
    
    Vector3 _seasideVector;

    private void Awake()
    {
        _seasideVector = _pointB.position - _pointA.position;
    }

    private void Update()
    {
        Vector3 playerPos = GameManager.Instance.playerManager.currentController.GetTransform().position;
        Vector3 offset = playerPos - _pointA.position;
        Vector3 projection = Vector3.Project(offset, _seasideVector);
        transform.position = _pointA.position + projection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_pointA.position, _pointB.position);
    }
}
