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
        print(gameObject.name);
        Tools.Flood(CurrentPoint);
    }

    public virtual async UniTask EndTurn()
    {
        Tools.ClearFlood();
    }

    public async UniTask ApplySpell(SpellData spell)
    {
        //EntityHealth.ApplyShield(spell.Damage);
        //EntityHealth.ApplyHealth(-spell.Damage);
        //EntityHealth.ApplyHealth(spell.Heal);
    }

    public virtual async UniTask TryMoveTo(WayPoint targetPoint, bool showTiles = true)
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
        Tools.Flood(CurrentPoint);
        ApplyWalkables(showTiles);
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

    public void ApplyWalkables(bool showTiles = true)
    {
        if(Walkables.Count == 0) 
            Walkables.AddRange(Tools.GetWaypointsInRange(MovePoints));

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
