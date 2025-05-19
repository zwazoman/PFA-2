using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AIBehaviour
{
    Intrepid,
    Coward
}

public class EnemyEntity : Entity
{
    [SerializeField] EnemyData Data;

    WayPoint targetPlayerPoint;

    const int ThinkDelayMilis = 300;

    protected override void Awake()
    {
        base.Awake();

        
        stats.maxMovePoints = Data.MaxMovePoints;
    }

    protected override void Start()
    {
        base.Start();

        stats.Setup(Data.MaxHealth);
        CombatManager.Instance.RegisterEntity(this);

        foreach(PremadeSpell premadeSpell in Data.Spells)
        {
            Spell spell = new();
            spell.spellData = premadeSpell.SpellData;
            spell.isDamaging = premadeSpell.isDamaging;
            spells.Add(spell);
        }
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        //await visuals.PlayAnimation("Attack");

        ApplyWalkables(true);

        targetPlayerPoint = FindClosestPlayerPoint();

        Spell choosenSpell = ChooseSpell(0);
        bool attacked;

        if (choosenSpell != null)
            attacked = await ChooseTarget(choosenSpell, choosenSpell.isDamaging);
        else
            attacked = true;

        if (attacked && stats.currentMovePoints > 0)
        {
            switch (Data.aiBehaviour)
            {
                case AIBehaviour.Intrepid:
                    //check si les walkables contiennent le joueur (si oui le retirer)
                    await MoveToward(targetPlayerPoint);
                    break;
                case AIBehaviour.Coward:
                    await MoveAwayFrom(targetPlayerPoint);
                    break;
            }
        }
        await EndTurn();
    }

    public override async UniTask EndTurn()
    {
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

    protected Spell ChooseSpell(int spellIndex)
    {
        return ComputeCastableSpells(spells)[spellIndex];
    }

    protected Spell ChooseRandomSpell()
    {
        return ComputeCastableSpells(spells).PickRandom();
    }

    protected Spell ChooseSpellWithRange()
    {
        int targetDistance = Tools.FloodDict[targetPlayerPoint];

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

    protected WayPoint FindClosestPlayerPoint()
    {
        if (CombatManager.Instance.PlayerEntities.Count == 0)
            return null;

        List<WayPoint> points = new List<WayPoint>();

        foreach (PlayerEntity player in CombatManager.Instance.PlayerEntities)
        {
            points.Add(player.currentPoint);
        }

        WayPoint result; 
        points.FindClosestFloodPoint(out result);

        return result;
    }

    protected async UniTask<bool> ChooseTarget(Spell choosenSpell, bool damageSpell)
    {
        WayPoint choosenTargetPoint;

        if (damageSpell)
            choosenTargetPoint = targetPlayerPoint;
        else
        {
            List<WayPoint> enemyPoints = new();

            foreach (EnemyEntity enemy in CombatManager.Instance.EnemyEntities)
            {
                enemyPoints.Add(enemy.currentPoint);
            }

            enemyPoints.FindClosestFloodPoint(out choosenTargetPoint, Tools.SmallFlood(targetPlayerPoint, 6, false, true));
        }

        return await TryAttack(choosenSpell, choosenTargetPoint);

    }

    /// <summary>
    /// preview le spell choosenSpell depuis le waypoint de la cible pour trouver le waypoint le plus proche depuis lequel tirer. Une fois cela fait : tire le sort depuis la cible
    /// </summary>
    /// <param name="choosenSpell"></param>
    /// <returns></returns>
    protected async UniTask<bool> TryAttack(Spell choosenSpell, WayPoint targetPoint)
    {
        print(targetPoint.Content.gameObject.name);

        if (targetPoint == null)
            return false;

        if(targetPoint == currentPoint)
        {
            return await CastSpell(choosenSpell, currentPoint, currentPoint, currentPoint);
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

        choosenTargetPoint.ChangeTileColor(choosenTargetPoint._zoneMaterial);

        // possibilité pour pas qu'elle se tire dessus ? ça serait rigolo n la stock qq part si ça se touche et on réésaie. si pas de solution on utilise celle qui touche

        await UniTask.Delay(ThinkDelayMilis);

        bool targetReached = await MoveToward(choosenTargetPoint); // le point le plus proche de lancé de sort

        if (targetReached)
        {
            return await CastSpell(choosenSpell,choosenTargetPoint,pointToSelect, targetPoint);
        }
        return false;
    }

    async UniTask<bool> CastSpell(Spell choosenSpell, WayPoint choosenTargetPoint, WayPoint pointToSelect, WayPoint target)
    {
        List<WayPoint> rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint);
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
