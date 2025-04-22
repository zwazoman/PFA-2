using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(SpellCaster))]

public class PlayerEntity : Entity
{
    SpellCaster _spellCaster;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent(out _spellCaster);
        CombatManager.Instance.PlayerEntities.Add(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override async UniTask TryMoveTo(WayPoint targetPoint)
    {
        await base.TryMoveTo(targetPoint);
    }

    public void UseSpell(int spellIndex)
    {
        
    }

    public void CancelSpellUse(int spellIndex)
    {
        
    }
}
