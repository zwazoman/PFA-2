using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{
    [SerializeField] EnemyData Data;

    protected override void Start()
    {
        base.Start();

        CombatManager.Instance.EnemyEntities.Add(this);
    }

    public override async UniTask PlayTurn()
    {
        await base.PlayTurn();

        ApplyWalkables(true);

        await TryAttack(ChooseSpell(0).SpellData);

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

    protected PremadeSpell ChooseSpellWithRange(WayPoint targetPoint)
    {
        return default;
    }

    protected WayPoint FindClosestPlayerPoint()
    {
        List<WayPoint> points = new List<WayPoint>();

        foreach (PlayerEntity player in CombatManager.Instance.PlayerEntities)
        {
            points.Add(player.CurrentPoint);
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
        WayPoint targetPlayerPoint = FindClosestPlayerPoint();

        targetPlayerPoint.ChangeTileColor(targetPlayerPoint._zoneMaterial);

        Dictionary<WayPoint, WayPoint> targetPointsDict = new();

        //créé le dict zonePoint,targetPoint
        EntitySpellCaster.PreviewSpellRange(choosenSpell, targetPlayerPoint, true, choosenSpell.Range);
        foreach(WayPoint rangePoint in EntitySpellCaster.RangePoints)
        {
            EntitySpellCaster.PreviewSpellZone(choosenSpell, rangePoint);
            foreach (WayPoint zonePoint in EntitySpellCaster.ZonePoints)
            {
                if(!targetPointsDict.ContainsKey(zonePoint))
                    targetPointsDict.Add(zonePoint, rangePoint);
            }
            EntitySpellCaster.StopSpellZonePreview();
        }
        EntitySpellCaster.StopSpellRangePreview();

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

            Vector3Int selfPointPos = GraphMaker.Instance.PointDict.GetKeyFromValue(CurrentPoint);
            Vector3Int zonePointPos = GraphMaker.Instance.PointDict.GetKeyFromValue(targetPlayerPoint);
            Vector3Int rangepointPos = GraphMaker.Instance.PointDict.GetKeyFromValue(selected);

            WayPoint pointToSelect = GraphMaker.Instance.PointDict[selfPointPos + (zonePointPos - rangepointPos)];

            EntitySpellCaster.PreviewSpellRange(choosenSpell);
            await UniTask.Delay(2000);
            EntitySpellCaster.PreviewSpellZone(choosenSpell, pointToSelect);
            await UniTask.Delay(2000);
            await EntitySpellCaster.TryCastSpell(choosenSpell, pointToSelect);

            return true;
        }
        return false;
    }

    /// <summary>
    /// déplace l'entité vers la case la plus proche de la target
    /// </summary>
    /// <param name="targetPoint"></param>
    /// <returns></returns>
    protected async UniTask<bool> MoveToward(WayPoint targetPoint)
    {
        await UniTask.Delay(1000);

        if (Walkables.Contains(targetPoint))
        {
            print("target in range !");
            await TryMoveTo(targetPoint);
            return true;
        }
        print("target not in range yet ! getting closer...");
        print(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint])));

        await TryMoveTo(Tools.FindClosestFloodPoint(Walkables, Tools.SmallFlood(targetPoint, Tools.FloodDict[targetPoint])));
        return false;
    }

}
