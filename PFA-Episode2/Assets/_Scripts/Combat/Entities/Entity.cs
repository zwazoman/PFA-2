using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpellCaster))]
public class Entity : MonoBehaviour
{
    [HideInInspector] public WayPoint CurrentPoint;
    [HideInInspector] public Health EntityHealth;
    [HideInInspector] public SpellCaster EntitySpellCaster;

    [SerializeField] public int MovePoints;

    protected Dictionary<WayPoint,int> WaypointDistance = new Dictionary<WayPoint,int>();
    protected List<WayPoint> Walkables = new List<WayPoint>();

    protected virtual void Awake()
    {
        TryGetComponent(out EntityHealth);
        TryGetComponent(out EntitySpellCaster);
    }

    protected virtual void Start()
    {
        Vector3Int roundedPos = transform.position.SnapOnGrid();
        transform.position = roundedPos;
        transform.position += Vector3.up * 1.3f;

        CurrentPoint = GraphMaker.Instance.PointDict[roundedPos];
        CurrentPoint.StepOn(this);
    }

    public virtual async UniTask PlayTurn()
    {
        ApplyWalkables();
    }

    public virtual async UniTask EndTurn()
    {
        ClearWalkables();
    }

    public async UniTask ApplySpell(SpellData spell)
    {
        EntityHealth.ApplyShield(spell.ShieldAmount);
        EntityHealth.ApplyHealth(-spell.Damage);
        EntityHealth.ApplyHealth(spell.Heal);
    }

    public virtual async UniTask TryMoveTo(WayPoint targetPoint)
    {
        ClearWalkables();

        Stack<WayPoint> path = Tools.FindBestPath(CurrentPoint, targetPoint);
        int pathlength = path.Count;

        if(pathlength > MovePoints)
        {
            print("plus de pm !");
            return;
        }

        foreach(WayPoint p in path)
        {
            p.ChangeTileColor(p._walkedMaterial);
        }

        for (int i = 0; i < pathlength; i++)
        {
            CurrentPoint.StepOff();

            WayPoint steppedOnPoint = path.Pop();

            await StartMoving(steppedOnPoint.transform.position);

            CurrentPoint = steppedOnPoint;
            steppedOnPoint.StepOn(this);

            steppedOnPoint.ChangeTileColor(steppedOnPoint._normalMaterial);

            MovePoints--;
        }
        ApplyWalkables();
    }

    protected void Flood(WayPoint targetPoint, int maxIndex = 0)
    {
        WaypointDistance.Clear();
        Queue<WayPoint> tmp = new Queue<WayPoint>();

        WaypointDistance.Add(targetPoint, 0);
        tmp.Enqueue(targetPoint);

        int index = 0;

        while(tmp.Count > 0 /*&& index != maxIndex*/)
        {
            index++;
            WayPoint currentPoint = tmp.Dequeue();
           
            foreach (WayPoint neighbour in currentPoint.Neighbours)
            {
                if (!WaypointDistance.ContainsKey(neighbour))
                {
                    tmp.Enqueue(neighbour);
                    WaypointDistance.Add(neighbour, index);
                }
            }
        }
    }

    async UniTask StartMoving(Vector3 targetPos, float moveSpeed = 2)
    {
        targetPos.y = 1f;
        Vector3 offset = targetPos - (Vector3)transform.position;
        Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg, 0);
        transform.rotation = targetRotation;
        while ((Vector3)transform.position != targetPos)
        {
            Vector3 offset2 = targetPos - (Vector3)transform.position;
            offset2 = Vector3.ClampMagnitude(offset2, Time.deltaTime * moveSpeed);
            transform.Translate(offset2, Space.World);
            await Task.Yield();
        }
    }

    public void ApplyWalkables()
    {
        Walkables.AddRange(Tools.GetReachablePoints(CurrentPoint, MovePoints).Keys);

        print(Walkables.Count);

        foreach (WayPoint point in Walkables)
        {
            point.ChangeTileColor(point._walkableMaterial);
        }
    }

    public void ClearWalkables()
    {
        foreach (WayPoint point in Walkables)
        {
            point.ChangeTileColor(point._normalMaterial);
        }
        Walkables.Clear();
    }


}
