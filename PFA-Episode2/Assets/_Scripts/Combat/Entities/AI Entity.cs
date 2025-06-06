using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum AIBehaviour
{
    Intrepid,
    Coward
}

[RequireComponent(typeof(EliteEntity))]
public class AIEntity : Entity
{
    [SerializeField] public EnemyData Data;

    WayPoint targetEntityPoint;

    const int ThinkDelayMilis = 150;

    EliteEntity _elite;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out _elite);

        team = Team.Enemy;
    }

    protected override void Start()
    {
        base.Start();

        //elite handle
        List<PremadeSpell> premadeSpells = new();
        if (Random.value < 0 && Data.CanBeElite )                //CONNARD MATEO TODO todo
        {
            _elite.ApplyEliteStats(ref premadeSpells, Data.Spells);

        }
        else
        {
            stats.maxMovePoints = Data.MaxMovePoints;
            stats.Setup(Data.MaxHealth, Data.MaxHealth);
        }
        if (premadeSpells.Count == 0)
            premadeSpells.AddRange(Data.Spells);

        CombatManager.Instance.RegisterEntity(this);

        foreach(PremadeSpell premadeSpell in premadeSpells)
        {
            Spell spell = new();
            spell.spellData = premadeSpell.SpellData;
            spell.spellType = premadeSpell.spellType;
            spells.Add(spell);
        }
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        //await visuals.PlayAnimation("Attack");

        ApplyWalkables(true);

        targetEntityPoint = FindClosestEnemyEntityPoint();

        Spell choosenSpell = ChooseSpellWithCooldown();
        bool attacked;

        if (choosenSpell != null)
            attacked = await ComputeSpellTarget(choosenSpell);
        else
            attacked = true;

        if (attacked && stats.currentMovePoints > 0 && isDead == false)
        {
            switch (Data.aiBehaviour)
            {
                case AIBehaviour.Intrepid:
                    //check si les walkables contiennent le joueur (si oui le retirer)
                    await MoveToward(targetEntityPoint);
                    break;
                case AIBehaviour.Coward:
                    await MoveAwayFrom(targetEntityPoint);
                    break;
            }
        }
        await EndTurn();
    }

    public override async UniTask EndTurn()
    {
        await UniTask.Delay(ThinkDelayMilis);

        await base.EndTurn();
    }

    List<Spell> ComputeCastableSpells(List<Spell> spells)
    {
        List<Spell> castableSpells = new();

        foreach(Spell spell in spells)
        {
            if(spell.canUse)
                castableSpells.Add(spell);
        }

        return castableSpells;
    }

    #region Spell Selection Methods
    protected Spell ChooseSpellWithIndex(int spellIndex)
    {
        return ComputeCastableSpells(spells)[spellIndex];
    }

    protected Spell ChooseRandomSpell()
    {
        return ComputeCastableSpells(spells).PickRandom();
    }

    protected Spell ChooseSpellWithCooldown()
    {
        List<Spell> castableSpells = ComputeCastableSpells(spells);

        int maxCooldown = -1;
        Spell choosenSpell = null;

        foreach (Spell spell in castableSpells)
            if(choosenSpell == null || spell.spellData.CoolDown > maxCooldown)
            {
                choosenSpell = spell;
                maxCooldown = choosenSpell.spellData.CoolDown;
            }

        return choosenSpell;
    }

    protected Spell ChooseSpellWithRange()
    {
        int targetDistance = Tools.FloodDict[targetEntityPoint];

        int offset = int.MaxValue;
        Spell choosenSpell = null;

        foreach (Spell premadeSpell in ComputeCastableSpells(spells))
        {
            int spellMaxReach = stats.currentMovePoints + premadeSpell.spellData.Range + Mathf.FloorToInt(premadeSpell.spellData.AreaOfEffect.Bounds.width / 2);
            int targetToMaxReachOffset = Mathf.Abs(spellMaxReach - targetDistance);
            if (targetToMaxReachOffset < offset)
            {
                offset = targetToMaxReachOffset;
                choosenSpell = premadeSpell;
            }
        }

        return choosenSpell;
    }
    #endregion

    protected WayPoint FindClosestEnemyEntityPoint()
    {
        List<Entity> enemyEntities = GetEnemyList();

        if (enemyEntities.Count == 0)
            return null;

        List<WayPoint> points = new List<WayPoint>();

        foreach (Entity entity in enemyEntities)
            points.Add(entity.currentPoint);

        WayPoint result;
        points.FindClosestFloodPoint(out result);

        return result;
    }

    protected async UniTask<bool> ComputeSpellTarget(Spell choosenSpell)
    {
        WayPoint choosenTargetPoint = targetEntityPoint;

        switch (choosenSpell.spellType)
        {
            case SpellType.Attack:
                return await CastSpellAtPoint(choosenSpell, choosenTargetPoint);
            case SpellType.Defense:
                List<WayPoint> enemyPoints = new();

                foreach (Entity entity in GetAllyEntities())
                {
                    enemyPoints.Add(entity.currentPoint);
                }

                enemyPoints.FindClosestFloodPoint(out choosenTargetPoint, Tools.SmallFlood(targetEntityPoint, 6, false, true));
                return await CastSpellAtPoint(choosenSpell, choosenTargetPoint);
            case SpellType.Utilitary:
                //cast le sort le plus proche possible du joueur

                List<WayPoint> targetPoints = new();

                List<WayPoint> rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, currentPoint, false);

                foreach (WayPoint point in rangePoints)
                {
                    if(point.State != WaypointState.HasEntity)
                        targetPoints.Add(point);
                }

                WayPoint choosenPoint;

                targetPoints.FindClosestFloodPoint(out choosenPoint, Tools.SmallFlood(targetEntityPoint, Tools.FloodDict[targetEntityPoint]));

                if (choosenPoint == null)
                    return true;

                await CastSpell(choosenSpell, choosenPoint, choosenPoint);
                break;
        }
        return true;

    }

    /// <summary>
    /// preview le spell choosenSpell depuis le waypoint de la cible pour trouver le waypoint le plus proche depuis lequel tirer. Une fois cela fait : tire le sort depuis la cible
    /// </summary>
    /// <param name="choosenSpell"></param>
    /// <returns></returns>
    protected async UniTask<bool> CastSpellAtPoint(Spell choosenSpell, WayPoint targetPoint)
    {
        if (targetPoint == null)
            return false;

        if(targetPoint == currentPoint)
        {
            return await CastSpell(choosenSpell, currentPoint, currentPoint);
        }

        Dictionary<WayPoint, List<WayPoint>> targetPointsDict = new();

        List<WayPoint> rangePoints;
        SpellCastData castData = new();

        rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, targetPoint, false, true);

        foreach (WayPoint rangePoint in rangePoints)
        {
            castData = entitySpellCaster.PreviewSpellZone(choosenSpell, rangePoint, rangePoints, false);
            foreach (WayPoint zonePoint in castData.zonePoints)
            {
                if (!targetPointsDict.ContainsKey(zonePoint))
                    targetPointsDict.Add(zonePoint, new List<WayPoint>());
                targetPointsDict[zonePoint].Add(rangePoint);
            }
        }

        entitySpellCaster.StopSpellRangePreview(ref rangePoints, ref castData);

        WayPoint choosenTargetPoint = null;
        WayPoint pointToSelect = null;

        while (castData.zonePoints == null || castData.zonePoints.Count == 0)
        {
            choosenTargetPoint = targetPointsDict.Keys.FindClosestFloodPoint();

            GetInvertShot(choosenTargetPoint, targetPointsDict[choosenTargetPoint][0], choosenSpell, out pointToSelect, targetPoint);

            rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint, false );
            castData = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints, false);

            targetPointsDict[choosenTargetPoint].Remove(targetPointsDict[choosenTargetPoint][0]);

            if (targetPointsDict[choosenTargetPoint].Count == 0)
                targetPointsDict.Remove(choosenTargetPoint);

            await UniTask.Yield();
        }

        
        //choosenTargetPoint.SetPreviewState(entitySpellCaster.ComputeShieldVsDamageDiff(choosenSpell) <= 0 ? WayPoint.PreviewState.SpellCastZone_Agressive : WayPoint.PreviewState.SpellCastZone_Shield); //@todo
        // possibilit� pour pas qu'elle se tire dessus ? �a serait rigolo n la stock qq part si �a se touche et on r��saie. si pas de solution on utilise celle qui touche

        await UniTask.Delay(ThinkDelayMilis);

        bool targetReached = await MoveToward(choosenTargetPoint); // le point le plus proche de lanc� de sort

        if (targetReached)
        {
            return await CastSpell(choosenSpell,pointToSelect, targetPoint);
        }
        return false;
    }

    async UniTask<bool> CastSpell(Spell choosenSpell, WayPoint pointToSelect, WayPoint target)
    {
        HideWalkables();
        List<WayPoint> rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, currentPoint);
        await UniTask.Delay(ThinkDelayMilis);
        SpellCastData castData = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints);
        await UniTask.Delay(ThinkDelayMilis);
        await entitySpellCaster.TryCastSpell(choosenSpell, pointToSelect, rangePoints, castData);

        return target.Content != null;
    }

    WayPoint GetInvertShot(WayPoint originalTarget, WayPoint rangeTarget, Spell choosenSpell, out WayPoint pointToSelect, WayPoint targetPoint)
    {
        Vector3Int selfPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(originalTarget);
        Vector3Int zonePointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetPoint);
        Vector3Int rangepointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(rangeTarget);

        pointToSelect = GraphMaker.Instance.serializedPointDict[selfPointPos + (zonePointPos - rangepointPos)];

        return pointToSelect;
    }
}
