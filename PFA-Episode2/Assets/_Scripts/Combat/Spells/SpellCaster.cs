using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] Entity _entity;

    WayPoint _entityWaypoint;

    private void Awake()
    {
        _entityWaypoint = _entity.CurrentPoint;
    }

    public List<WayPoint> PreviewSpellRange(SpellData spell,WayPoint center = null)
    {
        if (center == null) center = _entityWaypoint;

        //attache le spell au doigt du joueur. tire un raycast dans la direction du doigt. si il est sur une case, preview la zone du sort et ses dégâts. si le joueur relache dans le vide/sur une case non accessible : reset, sinon, await CastSpell(spell)
        return null;
    }

    public void PreviewSpellCast(SpellData spell, WayPoint targetedPoint)
    {

    }

    public void StopSpellCastPreview()
    {

    }

    public async UniTask CastSpell(SpellData spell, WayPoint target)
    {
        while(true /* tant que le joueur a pas relaché le spell*/)
        {
            await UniTask.Yield();
        }
    }
}
