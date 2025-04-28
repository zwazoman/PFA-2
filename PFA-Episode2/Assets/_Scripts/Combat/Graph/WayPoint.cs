using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WayPoint : MonoBehaviour
{
    public event Action OnSteppedOn;
    public event Action OnSteppedOff;

    public List<WayPoint> Neighbours = new List<WayPoint>();

    public GameObject Content;

    public bool IsActive;

    [SerializeField] LayerMask _mask;

    [Header("Materials")]

    [SerializeField] public Material _walkableMaterial;
    [SerializeField] public Material _rangeMaterial;
    [SerializeField] public Material _zoneMaterial;
    [SerializeField] public Material _normalMaterial;

    MeshRenderer _mR;

    #region Astar Fields
    [HideInInspector] public WayPoint FormerPoint;

    [HideInInspector] public bool IsOpen = false;
    [HideInInspector] public bool IsClosed = false;

    [HideInInspector] public float H;
    [HideInInspector] public float G;
    [HideInInspector] public float F => G + H ;
    #endregion

    private void Awake()
    {
        IsActive = true;

        TryGetComponent(out _mR);
    }

    private void Start()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.up, out hit, 1, _mask))
        {
            StepOn(hit.collider.gameObject);
        }
    }

    public void StepOn(GameObject entity)
    {
        Deactivate();
        Content = entity;
        OnSteppedOn?.Invoke();
    }

    public void StepOff()
    {
        Content = null;
        OnSteppedOff?.Invoke();
        Activate();
    }

    public void TryApplySpell(SpellData spell)
    {
        if (Content == null)
            return;

        //if(Content.TryGetComponent(out Health health))
        //{
        //    health.ApplyDamage(damages);
        //}
    }

    public void ChangeTileColor(Material material)
    {
        if(_mR.material == material) return;

        _mR.material = material;
    }

    #region Astar
    public void TravelThrough(ref List<WayPoint> openPoints,ref List<WayPoint> closedPoints, ref Stack<WayPoint> shorterPath, WayPoint endPoint, WayPoint startPoint)
    {
        if(this == endPoint)
        {
            Close(ref openPoints, ref closedPoints);
            WayPoint currentPoint = endPoint;
            while(currentPoint != startPoint)
            {
                shorterPath.Push(currentPoint);
                currentPoint = currentPoint.FormerPoint;
            }
            return;
        }

        Close(ref openPoints, ref closedPoints);

        foreach(WayPoint point in Neighbours)
        {
            if (point.IsClosed || point.IsOpen || !point.IsActive) continue;

            point.Open(this, endPoint,  ref openPoints);
        }

        if(openPoints.Count == 0)
        {
            print("Oh cong la target est pas dans le graph cagole");
            return;
        }

        WayPoint bestPoint = null;
        foreach (WayPoint point in openPoints)
        {
            if (bestPoint == null) bestPoint = point;
            else if (point.F < bestPoint.F) bestPoint = point;
        }

        bestPoint.TravelThrough(ref openPoints,ref closedPoints, ref shorterPath, endPoint, startPoint);
    }

    void Open(WayPoint formerPoint, WayPoint endPoint, ref List<WayPoint> openPoints)
    {
        IsOpen = true;

        openPoints.Add(this);

        FormerPoint = formerPoint;

        H = Vector3.Distance(transform.position, endPoint.transform.position);
        G ++;

    }

    void Close(ref List<WayPoint> openPoints, ref List<WayPoint> closedPoints)
    {
        IsClosed = true;
        closedPoints.Add(this);
        if(openPoints.Contains(this)) openPoints.Remove(this);
    }

    public void ResetState()
    {
        FormerPoint = null;
        G = 0;
        H = 0;
        IsClosed = false;
        IsOpen = false;
    }

    public void Activate()
    {
        IsActive = true;
        //gameObject.SetActive(true); // visuels pour l'instant
    }

    public void Deactivate()
    {
        IsActive = false;
        //gameObject.SetActive(false); // visuels pour l'instant
    }
    #endregion Astar

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (WayPoint point in Neighbours) //il dessine 2 fois mais t'inquietes
        {
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }


}