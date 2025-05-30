using UnityEngine;
using System.Collections.Generic;

public class SwipeOnMap : MonoBehaviour
{
    private Vector2 _mouseDown;
    private Vector2 _mouseUp;

    [SerializeField] private float detectionRadius = 1.5f;

    private List<Node> _directions = new();

    private void OnMouseDown()
    {
        _mouseDown = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        _mouseUp = Input.mousePosition;

        _directions.Clear();

        int playerPos = PlayerMap.Instance.PositionMap;

        for (int i = 0; i < MapMaker2.Instance.AllNodeGood.Count; i++)
        {
            Node node = MapMaker2.Instance.AllNodeGood[i];
            if (node.Position == playerPos + 1)
            {
                _directions.Add(node);
            }
        }

        Vector3 swipeDirection = GetSwipeDirection(_mouseDown, _mouseUp);
        if (swipeDirection == Vector3.zero || _directions.Count == 0) return;

        int directionIndex = GetDirectionIndex(swipeDirection, _directions);
        Node chooseNode = _directions[directionIndex];

        Debug.Log($"Swipe d�tect�, va vers le node : position : {chooseNode.Position}, hauteur : {chooseNode.Hauteur}");

        bool interactable = chooseNode.GetInteractable();

        if (interactable)
        {
            chooseNode.InteractPlayer();
        }
    }

    private Vector3 GetSwipeDirection(Vector2 startScreenPos, Vector2 endScreenPos)
    {
        Ray startRay = Camera.main.ScreenPointToRay(startScreenPos);
        Ray endRay = Camera.main.ScreenPointToRay(endScreenPos);

        Plane plane = new(Camera.main.transform.forward * -1, transform.position);

        if (plane.Raycast(startRay, out float startDist) && plane.Raycast(endRay, out float endDist))
        {
            Vector3 worldStart = startRay.GetPoint(startDist);
            Vector3 worldEnd = endRay.GetPoint(endDist);

            return (worldEnd - worldStart).normalized;
        }

        return Vector3.zero;
    }

    public int GetDirectionIndex(Vector3 referenceDirection, List<Node> directions)
    {
        int Index = 0;
        float minDot = float.NegativeInfinity;
        Vector3 playerPos = PlayerMap.Instance.transform.position;

        for (int i = 0; i < directions.Count; i++)
        {
            Vector3 dirToNode = (directions[i].transform.position - playerPos).normalized;
            float dot = Vector3.Dot(referenceDirection.normalized, dirToNode);

            if (dot > minDot)
            {
                minDot = dot;
                Index = i;
            }
        }

        return Index;
    }
}
