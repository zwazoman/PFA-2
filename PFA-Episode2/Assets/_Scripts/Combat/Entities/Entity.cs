using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

[RequireComponent(typeof(SpellCaster))]
public class Entity : MonoBehaviour
{
    public EntityStats stats = new();

    [HideInInspector] public WayPoint currentPoint;
    [HideInInspector] public SpellCaster entitySpellCaster;

    

    protected Dictionary<WayPoint, int> WaypointDistance = new Dictionary<WayPoint, int>();
    protected List<WayPoint> Walkables = new List<WayPoint>();

    protected virtual void Awake()
    {
        TryGetComponent(out entitySpellCaster);
        stats.owner = this;
    }

    protected virtual void Start()
    {

        //set up position on graph
        Vector3Int roundedPos = transform.position.SnapOnGrid();
        currentPoint = GraphMaker.Instance.serializedPointDict[roundedPos]; 
        currentPoint.StepOn(this);

        //set up health and movement
        stats.currentHealth = stats.maxHealth;
        stats.ApplyHealth(0);
        stats.currentMovePoints = stats.maxMovePoints;
        
    }

    public virtual async UniTask PlayTurn()
    {
        print(gameObject.name);
        Tools.Flood(currentPoint);
        stats.currentMovePoints = stats.maxMovePoints;
        await stats.ApplyShield(-1);
    }

    public virtual async UniTask EndTurn()
    {
        Tools.ClearFlood();
        ClearWalkables();
    }

    public async UniTask ApplySpell(SpellData spell, SpellCastingContext context)
    {
        print("apply Spell");
        foreach (SpellEffect effect in spell.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    await stats.ApplyDamage(effect.value);
                    break;
                case SpellEffectType.Recoil:
                    await Push(context.PushDirection);
                    break;
                case SpellEffectType.Shield:
                    await stats.ApplyShield(effect.value);
                    break;
                case SpellEffectType.DamageIncreaseForEachHitEnnemy:
                    throw new NotImplementedException();
                case SpellEffectType.DamageIncreasePercentageByDistanceToCaster:
                    throw new NotImplementedException();
                case SpellEffectType.Fire:
                    throw new NotImplementedException();
            }
        }
    }


    public void ApplyWalkables(bool showTiles = true)
    {
        if (Walkables.Count == 0)
            Walkables.AddRange(Tools.SmallFlood(currentPoint, stats.currentMovePoints,true,true).Keys);

        foreach (WayPoint point in Walkables)
        {
            if (point.State == WaypointState.Free)
            {
                point.ChangeTileColor(point._walkableMaterial);
            }
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

    async UniTask Push(Vector3Int pushDirection)
    {
        WayPoint choosenPoint = null;
        float damages = 0;

        // si diagonale -> tirer un spherecast dans la direction longueur = force * racine de 2:
        //si ça touche : pousser le joueur jusqu' au hit
        //sinon le pousser jusu'a force (condition si hors du terrain/ bloquée mieux que raycast ? tomber dans le vide ?
        //sinon -> tirer un raycast dans la direction longueur = force
        //si ça touche pousser de force
        //sinon -> le pousser jusu'a force (condition si hors du terrain/ bloquée mieux que raycast ? tomber dans le vide ?

        if (damages > 0)
        {
            await stats.ApplyDamage(damages);
        }

        await StartMoving(choosenPoint.transform.position);

        choosenPoint.StepOn(this);
        currentPoint = choosenPoint;
    }

    /// <summary>
    /// fait se déplacer l'entité vers la case la plus proche de la target
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask<bool> MoveToward(WayPoint targetPoint)
    {
        print("move !");

        await UniTask.Delay(1000);

        if (targetPoint == currentPoint)
            return true;

        if (Walkables.Contains(targetPoint) && targetPoint.State == WaypointState.Free)
        {
            print("target in range !");
            await TryMoveTo(targetPoint);
            return true;
        }

        await TryMoveTo(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint])));
        return false;
    }

    /// <summary>
    /// fait se déplcaer l'entité le plus loin possible de l'entité ciblée
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask MoveAwayFrom(WayPoint targetPoint)
    {
        float distanceToFurthestPoint = 0;
        WayPoint furthestPoint = null;

        foreach (WayPoint point in Walkables)
        {
            float pointDistanceToTarget = Tools.FloodDict[point] + Tools.FloodDict[targetPoint];

            if (pointDistanceToTarget > distanceToFurthestPoint)
            {
                distanceToFurthestPoint = pointDistanceToTarget;
                furthestPoint = point;
            }
        }

        await TryMoveTo(furthestPoint);
    }
    public virtual async UniTask TryMoveTo(WayPoint targetPoint, bool showTiles = true)
    {
        Stack<WayPoint> path = Tools.FindBestPath(currentPoint, targetPoint);
        int pathlength = path.Count;

        if (pathlength > stats.currentMovePoints)
        {
            print("plus de pm !");
            return;
        }

        foreach (WayPoint p in path)
        {
            p.ChangeTileColor(p._walkedMaterial);
        }

        for (int i = 0; i < pathlength; i++)
        {
            currentPoint.StepOff();

            WayPoint steppedOnPoint = path.Pop();

            await StartMoving(steppedOnPoint.transform.position);

            currentPoint = steppedOnPoint;
            steppedOnPoint.StepOn(this);

            steppedOnPoint.ChangeTileColor(steppedOnPoint._normalMaterial);

            stats.currentMovePoints--;
        }
        Tools.Flood(currentPoint);
        ClearWalkables();
        ApplyWalkables(showTiles);
    }

    async UniTask StartMoving(Vector3 targetPos, float moveSpeed = 8)
    {
        targetPos.y = transform.position.y;
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

    public async UniTask Die()
    {
        print("Die");

        currentPoint.StepOff();
        Destroy(gameObject);
        await CombatManager.Instance.UnRegisterEntity(this);
        //si il ets entrain de jouer EndTurn
    }
}
