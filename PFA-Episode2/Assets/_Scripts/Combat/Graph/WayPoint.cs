using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum WaypointState
{
    Free,
    Obstructed,
    HasEntity
}



public class WayPoint : MonoBehaviour
{
    const float TweenDuration = .2f;
    public enum PreviewState
    {
        NoPreview,
        Movement,
        SpellAreaOfEffect,
        SpellCastZone_Agressive,
        SpellCastZone_Shield,
        occludedAreaOfEffect
    }

    public event Action OnSteppedOn;
    public event Action OnSteppedOff;

    public Vector3Int graphPos;

    public List<WayPoint> Neighbours;

    public Entity Content;

    public WaypointState State;

    [Header("Tile preview")]
    [SerializeField] public MeshRenderer _previewVisuals;
    [SerializeField] public float heightOffset;

    
    PreviewState _currentPreviewState;


    #region Astar Fields
    [HideInInspector] public WayPoint FormerPoint;

    [HideInInspector] public bool IsOpen = false;
    [HideInInspector] public bool IsClosed = false;

    [HideInInspector] public float H;
    [HideInInspector] public float G;
    [HideInInspector] public float F => G + H;
    #endregion



    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1))
        {
            if (hit.collider.TryGetComponent(out Entity entity))
            {
                State = WaypointState.HasEntity;
            }
            else if(hit.collider.gameObject.layer == 7) //wall
            {
                State = WaypointState.Obstructed;
            }
        }
    }

    public async void StepOn(Entity entity)
    {
        if (State == WaypointState.Obstructed)
            await entity.Die();
        else
        {
            State = WaypointState.HasEntity;
            Content = entity;
            entity.currentPoint = this;
            OnSteppedOn?.Invoke();
        }
    }

    public void StepOff()
    {
        Content = null;
        OnSteppedOff?.Invoke();
        State = WaypointState.Free;
    }

    public void SetPreviewState(PreviewState state)
    {
        if (state == _currentPreviewState || _previewVisuals==null) return;

        _currentPreviewState = state;
        transform.DOKill();
        // material
        switch (state)
        {
            case PreviewState.Movement:
                _previewVisuals.transform.DOScale(1f, TweenDuration);
                _previewVisuals.sharedMaterial = GameManager.Instance.staticData._mat_movementPreview;
                break;
            case PreviewState.SpellAreaOfEffect:
                _previewVisuals.transform.DOScale(1f, TweenDuration);
                _previewVisuals.sharedMaterial = GameManager.Instance.staticData._mat_spellAoePreview;
                break;
            case PreviewState.SpellCastZone_Agressive:
                _previewVisuals.transform.DOScale(1f, TweenDuration);
                _previewVisuals.sharedMaterial = GameManager.Instance.staticData._mat_spellCastZonePreview;
                break;
            case PreviewState.SpellCastZone_Shield:
                _previewVisuals.transform.DOScale(1f, TweenDuration);
                _previewVisuals.sharedMaterial = GameManager.Instance.staticData._mat_spellAoePreview_shield;
                break;
            case PreviewState.occludedAreaOfEffect:
                Sequence s = DOTween.Sequence();
                s.Append(_previewVisuals.transform.DOScale(1f, TweenDuration).SetEase(Ease.OutCubic));
                s.Append(_previewVisuals.transform.DOScale(0, TweenDuration*2).SetEase(Ease.InCubic));
                s.onComplete = () => { if (state == PreviewState.occludedAreaOfEffect) SetPreviewState(PreviewState.NoPreview); };
                _previewVisuals.sharedMaterial = GameManager.Instance.staticData._mat_OccludedspellCastZonePreview;
                break;
            
            case PreviewState.NoPreview:
                break;
            
        }

        //dotween & activation
        if (state == PreviewState.NoPreview)
        {
            _previewVisuals.transform.DOScale(0, TweenDuration)
                .OnComplete(() => {
                    if (_currentPreviewState == PreviewState.NoPreview)
                        _previewVisuals.gameObject.SetActive(false); 
                });
        }
        else
        {
            _previewVisuals.gameObject.SetActive(true);
            if(state != PreviewState.occludedAreaOfEffect)_previewVisuals.transform.DOScale(1, TweenDuration);
        }
    }

    #region Astar
    public void TravelThrough(ref List<WayPoint> openPoints, ref List<WayPoint> closedPoints, ref Stack<WayPoint> shorterPath, WayPoint endPoint, WayPoint startPoint)
    {
        if (this == endPoint)
        {
            Close(ref openPoints, ref closedPoints);
            WayPoint currentPoint = endPoint;
            while (currentPoint != startPoint)
            {
                shorterPath.Push(currentPoint);
                currentPoint = currentPoint.FormerPoint;
            }
            return;
        }

        Close(ref openPoints, ref closedPoints);

        foreach (WayPoint point in Neighbours)
        {
            if (point.IsClosed || point.IsOpen || point.State != WaypointState.Free) continue;

            point.Open(this, endPoint, ref openPoints);
        }

        if (openPoints.Count == 0)
        {
            return;
        }

        WayPoint bestPoint = null;
        foreach (WayPoint point in openPoints)
        {
            if (bestPoint == null) bestPoint = point;
            else if (point.F < bestPoint.F) bestPoint = point;
        }

        bestPoint.TravelThrough(ref openPoints, ref closedPoints, ref shorterPath, endPoint, startPoint);
    }

    void Open(WayPoint formerPoint, WayPoint endPoint, ref List<WayPoint> openPoints)
    {
        IsOpen = true;

        openPoints.Add(this);

        FormerPoint = formerPoint;

        H = Vector3.Distance(transform.position, endPoint.transform.position);
        G++;

    }

    void Close(ref List<WayPoint> openPoints, ref List<WayPoint> closedPoints)
    {
        IsClosed = true;
        closedPoints.Add(this);
        if (openPoints.Contains(this)) openPoints.Remove(this);
    }

    public void ResetState()
    {
        FormerPoint = null;
        G = 0;
        H = 0;
        IsClosed = false;
        IsOpen = false;
    }
    #endregion Astar

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (WayPoint point in Neighbours) //il dessine 2 fois mais t'inquietes
        {
            if (point != null && point.Neighbours.Contains(this)) Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }

}