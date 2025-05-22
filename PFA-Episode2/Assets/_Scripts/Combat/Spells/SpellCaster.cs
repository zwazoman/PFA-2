using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] public Entity castingEntity;

    [SerializeField] LayerMask _obstacleMask;

    [SerializeField] Transform _spellCastingSocket;

    public const byte RangeRingThickness = 3;

    public bool attackEventCompleted;

    private void Awake()
    {
        if (castingEntity == null)
            TryGetComponent(out castingEntity);
    }


    //preview spell range
    public List<WayPoint> PreviewSpellRange(Spell spell, WayPoint center = null, bool showZone = true, bool ignoreTerrain = false)
    {
        if (center == null)
            center = castingEntity.currentPoint;

        Dictionary<WayPoint, int> floodDict = Tools.SmallFlood(center, spell.spellData.Range);

        List<WayPoint> rangePoints = new();

        foreach (WayPoint point in floodDict.Keys)
        {
            if ((!ignoreTerrain && (spell.spellData.IsOccludedByWalls && Tools.CheckWallsBetween(center, point) || point.State == WaypointState.Obstructed)) || spell.spellData.Range > RangeRingThickness && (floodDict[point] - RangeRingThickness) < 0)
                continue;
            else if (showZone)
                point.ChangeTileColor(point._rangeMaterial);

            rangePoints.Add(point);
        }

        return rangePoints;
    }
    public void StopSpellRangePreview(ref List<WayPoint> rangePoints, ref SpellCastData zoneData)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        rangePoints.Clear();

        StopSpellZonePreview(rangePoints, ref zoneData);
    }

    //preview spell zone
    public SpellCastData PreviewSpellZone(Spell spell, WayPoint targetedPoint, List<WayPoint> rangePoints, bool showZone = true)
    {
        SpellCastData castData = new();

        List<WayPoint> zonePoints = new();
        Dictionary<Entity, SpellCastingContext> hitEntityCTXDict = new();

        List<Entity> hitEntities = new();

        if (!rangePoints.Contains(targetedPoint))
        {
            return castData;
        }

        Vector3Int targetedPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetedPoint);

        foreach (Vector2Int pos in spell.spellData.AreaOfEffect.AffectedTiles)
        {
            Vector3Int posOffset = new Vector3Int(pos.x, 0, pos.y);
            Vector3Int newPos = targetedPointPos + posOffset;

            if (GraphMaker.Instance.serializedPointDict.ContainsKey(newPos))
            {
                WayPoint choosenWaypoint = GraphMaker.Instance.serializedPointDict[newPos];

                if (showZone)
                    choosenWaypoint.ChangeTileColor(choosenWaypoint._zoneMaterial);

                zonePoints.Add(choosenWaypoint);

                if (choosenWaypoint.State == WaypointState.HasEntity)
                    hitEntities.Add(choosenWaypoint.Content);
            }
        }
        castData.zonePoints = zonePoints;

        if (showZone)
            foreach (Entity entity in CombatManager.Instance.Entities)
            {
                if (hitEntities.Contains(entity))
                {
                    SpellCastingContext context = new();

                    context = ComputeCTX(spell, entity, hitEntities, targetedPoint);

                    hitEntityCTXDict.Add(entity, context);

                    castData.hitEntityCTXDict = hitEntityCTXDict;

                    PreviewSpellEffect(spell, entity, ref castData);
                }
                else
                {
                    StopSpellEffectPreview(entity);
                }

            }

        return castData;
    }

    public void StopSpellZonePreview(List<WayPoint> rangePoints, ref SpellCastData zoneData, bool showZone = true)
    {
        if (zoneData.zonePoints == null || zoneData.zonePoints.Count == 0) return;

        foreach (WayPoint point in zoneData.zonePoints)
        {
            if (rangePoints.Count != 0 && rangePoints.Contains(point) && showZone)
            {
                point.ChangeTileColor(point._rangeMaterial);
            }
            else
            {
                point.ChangeTileColor(point._normalMaterial);
            }
        }

        foreach (Entity entity in CombatManager.Instance.Entities)
            StopSpellEffectPreview(entity);

        zoneData.zonePoints.Clear();
    }

    //computations
    SpellCastingContext ComputeCTX(Spell spell, Entity hitEntity, List<Entity> hitEntities, WayPoint target)
    {
        Vector3Int entityTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(hitEntity.currentPoint);
        Vector3Int targetTilePos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(target);
        Vector3Int targetToEntity = entityTilePos - targetTilePos;

        int xPushDirection = targetToEntity.x != 0 ? (int)Mathf.Sign(targetToEntity.x) : 0;
        int zPushDirection = targetToEntity.z != 0 ? (int)Mathf.Sign(targetToEntity.z) : 0;
        Vector3 pushDirection = new Vector3(xPushDirection, 0, zPushDirection);

        SpellCastingContext context = new();

        context.numberOfHitEnnemies = (byte)hitEntities.Count;
        context.distanceToHitEnemy = (byte)targetToEntity.magnitude;
        context.pushDirection = pushDirection;
        context.casterPos = hitEntity.transform.position;

        return context;
    }

    WayPoint ComputePushPoint(Vector3 pushDirection, Entity hitEntity, int pushForce, out int pushDamages)
    {
        WayPoint choosenPoint = null;
        pushDamages = 0;

        Vector3 posWithHeigth = hitEntity.transform.position + Vector3.up * 0.5f;

        if (pushDirection == Vector3.zero)
        {
            Vector3 casterPosWithHeight = transform.position + Vector3.up * 0.5f;
            Vector3 casterToEntity = posWithHeigth - casterPosWithHeight;

            Debug.DrawLine(casterPosWithHeight, casterPosWithHeight + casterToEntity, Color.blue, 20);

            if (casterToEntity == Vector3.zero)
                return hitEntity.currentPoint;

            int xPushDirection = casterToEntity.x != 0 ? (int)Mathf.Sign(casterToEntity.x) : 0;
            int zPushDirection = casterToEntity.z != 0 ? (int)Mathf.Sign(casterToEntity.z) : 0;

            pushDirection = new Vector3(xPushDirection, 0, zPushDirection);
        }

        bool isDiagonal = pushDirection.x != 0 && pushDirection.z != 0;

        if (isDiagonal)
        {
            RaycastHit hit;
            if (Physics.SphereCast(posWithHeigth, .45f, pushDirection /*+Vector3.up * 0.5f*/, out hit, pushForce * Mathf.Sqrt(2)))
            {

                pushDamages = pushForce;
                Vector3 hitPos = hit.point/*.SnapOnGrid()*/;

                pushDamages -= Mathf.FloorToInt(hit.distance);
                choosenPoint = GraphMaker.Instance.serializedPointDict[(hitPos - pushDirection * .3f).SnapOnGrid()];
            }
            else
            {
                Vector3Int choosenPos = (posWithHeigth + (pushDirection * pushForce)).SnapOnGrid();
                choosenPoint = GraphMaker.Instance.serializedPointDict[choosenPos];
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(posWithHeigth, pushDirection, out hit, pushForce))
            {
                pushDamages = pushForce;

                Vector3 hitPos = hit.point;

                pushDamages -= Mathf.FloorToInt(hit.distance);
                choosenPoint = GraphMaker.Instance.serializedPointDict[(hitPos - pushDirection * .3f).SnapOnGrid()];
            }
            else
            {
                choosenPoint = GraphMaker.Instance.serializedPointDict[(posWithHeigth + pushDirection * pushForce).SnapOnGrid()];
            }
        }

        return choosenPoint;
    }

    BakedSpellEffect ComputeBakedSpellEffect(Spell spell, Entity entity, ref SpellCastData zoneData)
    {
        BakedSpellEffect e = new();

        foreach (SpellEffect effect in spell.spellData.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    e.damage += effect.value;
                    break;
                case SpellEffectType.Recoil:

                    WayPoint pushPoint = ComputePushPoint(
                        zoneData.hitEntityCTXDict[entity].pushDirection,
                        entity,
                        (int)effect.value,
                        out int pushDamages);

                    zoneData.hitEntityCTXDict[entity].PushDamage = pushDamages;
                    zoneData.hitEntityCTXDict[entity].PushPoint = pushPoint;

                    e.pushDamage = pushDamages;
                    e.pushPoint = zoneData.hitEntityCTXDict[entity].PushPoint;

                    break;
                case SpellEffectType.Shield:
                    e.shield += effect.value;
                    break;

                case SpellEffectType.DamageIncreaseForEachHitEnnemy:
                    e.damage += 8 * (zoneData.hitEntityCTXDict[entity].numberOfHitEnnemies - 1);
                    break;
                case SpellEffectType.DamageIncreasePercentageByDistanceToCaster:
                    e.damage += 5 * zoneData.hitEntityCTXDict[entity].distanceToHitEnemy;
                    break;

                case SpellEffectType.Fire:
                    break;
            }
        }

        return e;
    }

    //preview spell effect
    void PreviewSpellEffect(Spell spell, Entity entity, ref SpellCastData zoneData)
    {
        BakedSpellEffect e = ComputeBakedSpellEffect(spell, entity, ref zoneData);
        entity.PreviewSpellEffect(e);
    }

    void StopSpellEffectPreview(Entity entity)
    {
        entity.StopPreviewingSpellEffect();
    }

    //spell casting
    public async UniTask<bool> TryCastSpell(Spell spell, WayPoint target, List<WayPoint> rangePoints, SpellCastData zoneData)
    {
        if (zoneData.zonePoints == null || zoneData.zonePoints.Count == 0)
        {
            StopSpellRangePreview(ref rangePoints, ref zoneData);
            return false;
        }

        PlayerEntity playerCastingEntity = null;

        if (castingEntity is PlayerEntity)
        {
            playerCastingEntity = castingEntity as PlayerEntity;
            playerCastingEntity.HideSpellsUI();
        }

        await castingEntity.LookAt(target);

        attackEventCompleted = false;
        castingEntity.visuals.animator.SetTrigger(castingEntity.attackTrigger);

        while (!attackEventCompleted)
            await UniTask.Yield();


        if (zoneData.hitEntityCTXDict != null && zoneData.hitEntityCTXDict.Keys != null)
        {
            List<UniTask> tasks = new();
            foreach (Entity entity in zoneData.hitEntityCTXDict.Keys)
            {
                tasks.Add(HitEntityBehaviour(entity, spell, zoneData));
            }

            await UniTask.WhenAll(tasks);
        }


        if (playerCastingEntity != null)
            playerCastingEntity.ShowSpellsUI();

        spell.StartCooldown();

        StopSpellRangePreview(ref rangePoints, ref zoneData);

        return true;
    }

    async UniTask HitEntityBehaviour(Entity entity, Spell spell, SpellCastData zoneData)
    {
        if (entity != castingEntity)
            await entity.LookAt(castingEntity.currentPoint);

        SpellProjectile projectile;
        PoolManager.Instance.ProjectilePool.PullObjectFromPool(_spellCastingSocket.position).TryGetComponent(out projectile);
        await projectile.Launch(castingEntity, entity, spell.spellData.Mesh);

        //cancel preview
        StopSpellEffectPreview(entity);

        await entity.visuals.animator.PlayAnimationTrigger(entity.hitTrigger);

        BakedSpellEffect e = ComputeBakedSpellEffect(spell, entity, ref zoneData);
        await entity.ApplySpell(e);

        attackEventCompleted = false;
    }
}


// == data  ==

public struct BakedSpellEffect
{
    public float damage;
    public float shield;
    public float pushDamage;
    public WayPoint pushPoint;
}

public struct SpellCastData
{
    public List<WayPoint> zonePoints;
    public Dictionary<Entity, SpellCastingContext> hitEntityCTXDict;
}

/// <summary>
/// utilisé pour appliquer des effets en plus
/// au moment de lancer un sort.
/// </summary>
public class SpellCastingContext
{
    public byte numberOfHitEnnemies;
    public byte distanceToHitEnemy;
    public Vector3 pushDirection;
    public Vector3 casterPos;
    public WayPoint PushPoint;
    public int PushDamage;
}
