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
            if(targetToMaxReachOffset < offset)
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
        targetPlayerPoint.ChangeTileColor(targetPlayerPoint._zoneMaterial);

        Dictionary<WayPoint, WayPoint> targetPointsDict = new();

        //créé le dict zonePoint,targetPoint
        entitySpellCaster.PreviewSpellRange(choosenSpell, targetPlayerPoint, true, choosenSpell.Range);
        await UniTask.Delay(300);
        foreach (WayPoint rangePoint in entitySpellCaster.RangePoints)
        {
            entitySpellCaster.PreviewSpellZone(choosenSpell, rangePoint);
            foreach (WayPoint zonePoint in entitySpellCaster.ZonePoints)
            {
                if(!targetPointsDict.ContainsKey(zonePoint))
                    targetPointsDict.Add(zonePoint, rangePoint);
            }
            await UniTask.Delay(100);
            entitySpellCaster.StopSpellZonePreview();
        }
        await UniTask.Delay(300);
        entitySpellCaster.StopSpellRangePreview();

        List<WayPoint> allTargetPoints = new List<WayPoint>();
        allTargetPoints.AddRange(targetPointsDict.Keys);

        foreach(WayPoint point in allTargetPoints)
        {
            point.ChangeTileColor(point._zoneMaterial);
        }

        WayPoint choosenTargetPoint = allTargetPoints.FindClosestFloodPoint();

        print(choosenTargetPoint);

        await UniTask.Delay(1000);

        choosenTargetPoint.ChangeTileColor(choosenTargetPoint._rangeMaterial);

        await UniTask.Delay(1000);

        bool targetReached = await MoveToward(choosenTargetPoint); // le point le plus proche de lancé de sort

        foreach (WayPoint point in allTargetPoints)
        {
            point.ChangeTileColor(point._normalMaterial);
        }

        if (targetReached)
        {
            print("attack !");

            WayPoint selected = targetPointsDict[choosenTargetPoint];

            Vector3Int selfPointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(currentPoint);
            Vector3Int zonePointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(targetPlayerPoint);
            Vector3Int rangepointPos = GraphMaker.Instance.serializedPointDict.GetKeyFromValue(selected);

            WayPoint pointToSelect = GraphMaker.Instance.serializedPointDict[selfPointPos + (zonePointPos - rangepointPos)];

            entitySpellCaster.PreviewSpellRange(choosenSpell);
            await UniTask.Delay(2000);
            entitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect);
            await UniTask.Delay(2000);
            await entitySpellCaster.TryCastSpell(choosenSpell, pointToSelect);

            return true;
        }
        return false;
    }
}
