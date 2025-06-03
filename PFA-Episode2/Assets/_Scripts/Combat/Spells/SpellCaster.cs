using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

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
                point.SetPreviewState(WayPoint.PreviewState.SpellAreaOfEffect);

            rangePoints.Add(point);
        }

        return rangePoints;
    }
    public void StopSpellRangePreview(ref List<WayPoint> rangePoints, ref SpellCastData zoneData)
    {
        foreach (WayPoint point in rangePoints)
        {
            point.SetPreviewState(WayPoint.PreviewState.NoPreview);
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
                {
                    //bleu si + de shield que de degats
                    choosenWaypoint.SetPreviewState(ComputeShieldVsDamageDiff(spell) <= 0 ? WayPoint.PreviewState.SpellCastZone_Agressive : WayPoint.PreviewState.SpellCastZone_Shield);
                }

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

        castData.target = targetedPoint;

        return castData;
    }

    public void StopSpellZonePreview(List<WayPoint> rangePoints, ref SpellCastData zoneData, bool showZone = true)
    {
        if (zoneData.zonePoints == null || zoneData.zonePoints.Count == 0) return;

        foreach (WayPoint point in zoneData.zonePoints)
        {
            if (rangePoints.Count != 0 && rangePoints.Contains(point) && showZone)
            {
                point.SetPreviewState(WayPoint.PreviewState.SpellAreaOfEffect);
            }
            else
            {
                point.SetPreviewState(WayPoint.PreviewState.NoPreview);
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

            int xPushDirection = Mathf.Round(casterToEntity.x) != 0 ? (int)Mathf.Sign(casterToEntity.x) : 0;
            int zPushDirection = Mathf.Round(casterToEntity.z) != 0 ? (int)Mathf.Sign(casterToEntity.z) : 0;

            pushDirection = new Vector3(xPushDirection, 0, zPushDirection);

            Debug.DrawLine(posWithHeigth, posWithHeigth + pushDirection * 2, Color.red, 20);

        }

        bool isDiagonal = Mathf.RoundToInt(pushDirection.x) != 0 && Mathf.RoundToInt(pushDirection.z) != 0;

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

    void SummonEntityAtPoint(WayPoint point)
    {
        GameObject kamikaze = Instantiate(GameManager.Instance.staticData.kamikaze, new Vector3(point.transform.position.x, .5f, point.transform.position.z), Quaternion.identity);
        Entity entity = kamikaze.GetComponent<Entity>();

        if (castingEntity.team == Team.Player)
            entity.team = Team.Player;
        else if (castingEntity.team == Team.Enemy)
            entity.team = Team.Enemy;
    }

    BakedUtilitarySpellEffect ComputeUtilitarySpellEffect(Spell spell, ref SpellCastData zoneData)
    {
        BakedUtilitarySpellEffect e = new();

        foreach (SpellEffect effect in spell.spellData.Effects)
            switch (effect.effectType)
            {
                case SpellEffectType.EntitySummon:
                    if (zoneData.target.State == WaypointState.Free)
                        e.summonPoint = zoneData.target;
                    else
                    {
                        List<WayPoint> wayPoints = new List<WayPoint>();
                        foreach (WayPoint point in zoneData.zonePoints)
                        {
                            if (point.State == WaypointState.Free)
                                wayPoints.Add(point);
                        }
                        if (wayPoints.Count == 0)
                            break;
                        e.summonPoint = wayPoints.PickRandom();
                    }
                    break;
            }

        return e;
    }

    /// <summary>
    /// bake tous les effets du spell en fonction du contexte
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="entity"></param>
    /// <param name="zoneData"></param>
    /// <returns></returns>
    BakedTargetedSpellEffect ComputeTargetedSpellEffect(Spell spell, ref SpellCastData zoneData, Entity entity)
    {
        BakedTargetedSpellEffect e = new();

        foreach (SpellEffect effect in spell.spellData.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    if (effect.statType == StatType.FlatIncrease) e.damage += effect.value;
                    else if (effect.statType == StatType.Multiplier) e.damage *= effect.value;
                    else throw new System.Exception("y'a un pb là");

                    break;
                
                case SpellEffectType.Recoil:
                    WayPoint pushPoint = ComputePushPoint(
                        zoneData.hitEntityCTXDict[entity].pushDirection,
                        entity,
                        (int)effect.value,
                        out int pushDamages);

                    zoneData.hitEntityCTXDict[entity].PushDamage = pushDamages;
                    zoneData.hitEntityCTXDict[entity].PushPoint = pushPoint;

                    e.pushDamage = pushDamages * 2;
                    e.pushPoint = zoneData.hitEntityCTXDict[entity].PushPoint;
                    break;
                
                case SpellEffectType.Shield:
                    if (effect.statType == StatType.FlatIncrease) e.shield += effect.value;
                    else if (effect.statType == StatType.Multiplier) e.shield *= effect.value;
                    else throw new System.Exception("y'a un pb l�");
                    break;

                case SpellEffectType.DamageIncreaseForEachHitEnnemy:
                    e.damage += 8 * (zoneData.hitEntityCTXDict[entity].numberOfHitEnnemies - 1);
                    break;
                case SpellEffectType.DamageIncreasePercentageByDistanceToCaster:
                    e.damage *= (1 + zoneData.hitEntityCTXDict[entity].distanceToHitEnemy * .2f);
                    break;
                case SpellEffectType.DamageIncreaseMeleeRange:
                    if(zoneData.hitEntityCTXDict[entity].distanceToHitEnemy == 1)
                    {
                        e.damage *= (effect.value);
                    }
                    break;
            }

            
        }

        e.damage = Mathf.Ceil(e.damage);
        e.shield = Mathf.Ceil(e.shield);
        e.pushDamage = Mathf.Ceil(e.pushDamage);

        return e;
    }

    /// <summary>
    ///  calcul la difference entre les degats et le shield du spell. n�gatif si le spell fait des d�gats, positif si il donne du shield
    /// </summary>
    /// <param name="spell"></param>
    /// <returns></returns>
    public float ComputeShieldVsDamageDiff(Spell spell)
    {
        float shield = 0;
        float damage = 0;

        foreach (SpellEffect effect in spell.spellData.Effects)
        {
            switch (effect.effectType)
            {
                case SpellEffectType.Damage:
                    if (effect.statType == StatType.FlatIncrease)
                        damage += effect.value;
                    else if (effect.statType == StatType.Multiplier)
                        damage *= effect.value;
                    break;

                case SpellEffectType.Shield:
                    if (effect.statType == StatType.FlatIncrease)
                        shield += effect.value;
                    else if (effect.statType == StatType.Multiplier)
                        shield *= effect.value;
                    break;
            }
        }

        return shield - damage;
    }

    //preview spell effect
    void PreviewSpellEffect(Spell spell, Entity entity, ref SpellCastData zoneData)
    {
        BakedTargetedSpellEffect e = ComputeTargetedSpellEffect(spell, ref zoneData, entity);
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
        try
        {
            castingEntity.visuals.animator.SetTrigger(Entity.attackTrigger);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            attackEventCompleted = true;
        }

        while (!attackEventCompleted)
            await UniTask.Yield();

        List<UniTask> tasks = new();

        if (zoneData.hitEntityCTXDict != null && zoneData.hitEntityCTXDict.Keys != null)
        {
            foreach (Entity entity in zoneData.hitEntityCTXDict.Keys)
            {
                tasks.Add(HitEntityBehaviour(entity, spell, zoneData));
            }
        }

        tasks.Add(UtilitaryBehaviour(spell, zoneData, target));

        await UniTask.WhenAll(tasks);


        if (playerCastingEntity != null)
            playerCastingEntity.ShowSpellsUI();

        spell.StartCooldown();

        StopSpellRangePreview(ref rangePoints, ref zoneData);

        return true;
    }

    async UniTask UtilitaryBehaviour(Spell spell, SpellCastData zoneData, WayPoint target)
    {
        BakedUtilitarySpellEffect e = ComputeUtilitarySpellEffect(spell, ref zoneData);
        await ApplyUtilitarySpell(e, spell);
    }

    async UniTask HitEntityBehaviour(Entity entity, Spell spell, SpellCastData zoneData)
    {
        if (entity != castingEntity)
            await entity.LookAt(castingEntity.currentPoint);

        SpellProjectile projectile;
        Vector3 spawnPos;
        if (_spellCastingSocket != null)
            spawnPos = _spellCastingSocket.position;
        else
            spawnPos = transform.position;

        PoolManager.Instance.ProjectilePool.PullObjectFromPool(spawnPos).TryGetComponent(out projectile);
        await projectile.Launch(castingEntity, entity, spell.spellData.Mesh);

        BakedTargetedSpellEffect e = ComputeTargetedSpellEffect(spell, ref zoneData, entity);

        //wait for animations to play
        if (e.pushPoint==null) try
        {
            await entity.visuals.animator.PlayAnimationTrigger(Entity.hitTrigger);
        }
        catch (Exception ex) { Debug.LogException(ex); }

        //cancel preview
        StopSpellEffectPreview(entity);

        await entity.ApplySpell(e);

        attackEventCompleted = false;
    }

    async UniTask ApplyUtilitarySpell(BakedUtilitarySpellEffect effect, Spell spell)
    {
        if (effect.summonPoint != null)
        {
            SpellProjectile projectile;
            Vector3 spawnPos;
            if (_spellCastingSocket != null)
                spawnPos = _spellCastingSocket.position;
            else
                spawnPos = transform.position;

            PoolManager.Instance.ProjectilePool.PullObjectFromPool(spawnPos).TryGetComponent(out projectile);
            await projectile.Launch(castingEntity, effect.summonPoint, spell.spellData.Mesh);

            SummonEntityAtPoint(effect.summonPoint);
        }
    }
}


// == data ==

public struct BakedTargetedSpellEffect
{
    public float damage;
    public float shield;
    public float pushDamage;
    public WayPoint pushPoint;
}

public struct BakedUtilitarySpellEffect
{
    public WayPoint summonPoint;
}

public struct SpellCastData
{
    public List<WayPoint> zonePoints;
    public Dictionary<Entity, SpellCastingContext> hitEntityCTXDict;

    public WayPoint target;
}

/// <summary>
/// utilis� pour appliquer des effets en plus
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
