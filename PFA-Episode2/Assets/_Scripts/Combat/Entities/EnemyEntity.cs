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
            spells.Add(spell);
        }
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        ApplyWalkables(true);

        targetPlayerPoint = FindClosestPlayerPoint();

        bool attacked = await TryAttack(ChooseSpell(0));

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

    protected Spell ChooseSpell(int spellIndex)
    {
        return spells[spellIndex];
    }

    protected Spell ChooseRandomSpell()
    {
        return spells.PickRandom();
    }

    protected Spell ChooseSpellWithRange()
    {
        int targetDistance = Tools.FloodDict[targetPlayerPoint];

        int offset = int.MaxValue;
        Spell choosenSpell = null;

        foreach (Spell premadeSpell in spells)
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
        List<WayPoint> points = new List<WayPoint>();

        foreach (PlayerEntity player in CombatManager.Instance.PlayerEntities)
        {
            points.Add(player.currentPoint);
        }

        WayPoint result = points.FindClosestFloodPoint();

        return result;
    }

    /// <summary>
    /// preview le spell choosenSpell depuis le waypoint de la cible pour trouver le waypoint le plus proche depuis lequel tirer. Une fois cela fait : tire le sort depuis la cible
    /// </summary>
    /// <param name="choosenSpell"></param>
    /// <returns></returns>
    protected async UniTask<bool> TryAttack(Spell choosenSpell)
    {
        Dictionary<WayPoint, List<WayPoint>> targetPointsDict = new();

        List<WayPoint> rangePoints;
        SpellCastData castData = new();

        rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, targetPlayerPoint, false, true);

        foreach (WayPoint rangePoint in rangePoints)
        {
            castData = entitySpellCaster.PreviewSpellZone(choosenSpell, rangePoint, rangePoints, false);
            print(castData.zonePoints.Count);
            foreach (WayPoint zonePoint in castData.zonePoints)
            {
                if (!targetPointsDict.ContainsKey(zonePoint))
                    targetPointsDict.Add(zonePoint, new List<WayPoint>());
                targetPointsDict[zonePoint].Add(rangePoint);
            }
        }


        WayPoint choosenTargetPoint = null;
        WayPoint pointToSelect = null;

        castData.zonePoints = null;

        while (castData.zonePoints == null)
        {
            choosenTargetPoint = targetPointsDict.Keys.FindClosestFloodPoint();

            print(targetPointsDict.Keys.Count);
            print(choosenTargetPoint);

            GetInvertShot(choosenTargetPoint, targetPointsDict[choosenTargetPoint][0], choosenSpell, out pointToSelect);

            print("singe encore encore");

            rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint, false );
            castData = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints, false);

            targetPointsDict[choosenTargetPoint].Remove(targetPointsDict[choosenTargetPoint][0]);

            if (targetPointsDict[choosenTargetPoint].Count == 0)
                targetPointsDict.Remove(choosenTargetPoint);

            await UniTask.Yield();
        }

        // possibilité pour pas qu'elle se tire dessus ? ça serait rigolo n la stock qq part si ça se touche et on réésaie. si pas de solution on utilise celle qui touche

        await UniTask.Delay(ThinkDelayMilis);

        bool targetReached = await MoveToward(choosenTargetPoint); // le point le plus proche de lancé de sort

        if (targetReached)
        {
            return await CastSpell(rangePoints, castData,choosenSpell,choosenTargetPoint,pointToSelect);
        }
        return false;
    }

    async UniTask<bool> CastSpell(List<WayPoint> rangePoints, SpellCastData castData, Spell choosenSpell, WayPoint choosenTargetPoint, WayPoint pointToSelect)
    {
        print("attack !");
        rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint);
        await UniTask.Delay(ThinkDelayMilis);
        castData = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints);
        await UniTask.Delay(ThinkDelayMilis);
        await entitySpellCaster.TryCastSpell(choosenSpell, pointToSelect, rangePoints, castData);

        return targetPlayerPoint.Content != null;
    }

    WayPoint GetInvertShot(WayPoint originalTarget, WayPoint rangeTarget, Spell choosenSpell, out WayPoint pointToSelect)
    {
        Vector3Int selfPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(originalTarget);
        Vector3Int zonePointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetPlayerPoint);
        Vector3Int rangepointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(rangeTarget);

        pointToSelect = GraphMaker.Instance.serializedPointDict[selfPointPos + (zonePointPos - rangepointPos)];

        print(pointToSelect);

        return pointToSelect;
    }
}
