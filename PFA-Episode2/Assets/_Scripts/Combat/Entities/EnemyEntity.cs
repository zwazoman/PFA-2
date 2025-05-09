using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum AIBehaviour
{
    Intrepid,
    Coward
}

public class EnemyEntity : Entity
{
    [SerializeField] EnemyData Data;

    WayPoint targetPlayerPoint;

    protected override void Awake()
    {
        base.Awake();

        entityStats.maxHealth = Data.MaxHealth;
        entityStats.maxMovePoints = Data.MaxMovePoints;
    }

    protected override void Start()
    {
        base.Start();

        CombatManager.Instance.EnemyEntities.Add(this);
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        ApplyWalkables(true);

        targetPlayerPoint = FindClosestPlayerPoint();

        bool attacked = await TryAttack(ChooseSpell(0).SpellData);

        targetPlayerPoint = FindClosestPlayerPoint();

        if (attacked && entityStats.currentMovePoints > 0)
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

    protected PremadeSpell ChooseSpell(int spellIndex)
    {
        return Data.Spells[spellIndex];
    }

    protected PremadeSpell ChooseRandomSpell()
    {
        return Data.Spells.PickRandom();
    }

    protected PremadeSpell ChooseSpellWithRange()
    {
        int targetDistance = Tools.FloodDict[targetPlayerPoint];

        int offset = int.MaxValue;
        PremadeSpell choosenSpell = null;

        foreach (PremadeSpell spell in Data.Spells)
        {
            int spellMaxReach = entityStats.currentMovePoints + spell.SpellData.Range + Mathf.FloorToInt(spell.SpellData.AreaOfEffect.Bounds.width / 2);
            int targetToMaxReachOffset = Mathf.Abs(spellMaxReach - targetDistance);
            if (targetToMaxReachOffset < offset)
            {
                offset = targetToMaxReachOffset;
                choosenSpell = spell;
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
    protected async UniTask<bool> TryAttack(SpellData choosenSpell)
    {
        Dictionary<WayPoint, List<WayPoint>> targetPointsDict = new();

        List<WayPoint> rangePoints = new();
        List<WayPoint> zonePoints = new();

        rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, targetPlayerPoint, true, true);

        await UniTask.Delay(1000);

        foreach (WayPoint rangePoint in rangePoints)
        {
            zonePoints = entitySpellCaster.PreviewSpellZone(choosenSpell, rangePoint, rangePoints, true);
            foreach (WayPoint zonePoint in zonePoints)
            {
                if (!targetPointsDict.ContainsKey(zonePoint))
                    targetPointsDict.Add(zonePoint, new List<WayPoint>());
                targetPointsDict[zonePoint].Add(rangePoint);
            }
            await UniTask.Delay(100);

            entitySpellCaster.StopSpellZonePreview(rangePoints, ref zonePoints);
        }
        entitySpellCaster.StopSpellRangePreview(ref rangePoints);

        foreach (WayPoint zonePoint in targetPointsDict.Keys)
        {
            zonePoint.ChangeTileColor(zonePoint._zoneMaterial);
        }

        await UniTask.Delay(1000);

        foreach (WayPoint zonePoint in targetPointsDict.Keys)
        {
            zonePoint.ChangeTileColor(zonePoint._normalMaterial);
        }


        WayPoint choosenTargetPoint = null;
        WayPoint pointToSelect = null;

        zonePoints = null;

        while (zonePoints == null)
        {
            choosenTargetPoint = targetPointsDict.Keys.FindClosestFloodPoint();

            GetInvertShot(choosenTargetPoint, targetPointsDict[choosenTargetPoint][0], choosenSpell, out pointToSelect);

            print("singe encore encore");

            rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint, true );
            await UniTask.Delay(1000);
            zonePoints = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints, true);
            await UniTask.Delay(1000);

            targetPointsDict[choosenTargetPoint].Remove(targetPointsDict[choosenTargetPoint][0]);

            if (targetPointsDict[choosenTargetPoint].Count == 0)
                targetPointsDict.Remove(choosenTargetPoint);

            await UniTask.Yield();
        }

        // possibilité pour pas qu'elle se tire dessus ? ça serait rigolo n la stock qq part si ça se touche et on réésaie. si pas de solution on utilise celle qui touche

        await UniTask.Delay(1000);

        bool targetReached = await MoveToward(choosenTargetPoint); // le point le plus proche de lancé de sort

        if (targetReached)
        {
            print("attack !");
            rangePoints = entitySpellCaster.PreviewSpellRange(choosenSpell, choosenTargetPoint);
            await UniTask.Delay(2000);
            zonePoints = entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect, rangePoints);
            await UniTask.Delay(2000);
            await entitySpellCaster.TryCastSpell(choosenSpell, pointToSelect, rangePoints, zonePoints);

            return true;
        }
        return false;
    }

    WayPoint GetInvertShot(WayPoint originalTarget, WayPoint rangeTarget, SpellData choosenSpell, out WayPoint pointToSelect)
    {

        Vector3Int selfPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(originalTarget);
        Vector3Int zonePointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetPlayerPoint);
        Vector3Int rangepointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(rangeTarget);

        pointToSelect = GraphMaker.Instance.serializedPointDict[selfPointPos + (zonePointPos - rangepointPos)];

        print(pointToSelect);

        return pointToSelect;
    }
}
