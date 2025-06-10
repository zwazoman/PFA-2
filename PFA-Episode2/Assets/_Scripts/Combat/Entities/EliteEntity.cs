using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EliteEntity : MonoBehaviour
{
    public const float EliteFactor = .3f;

    [SerializeField] VisualEffect _eliteEffect;

    [HideInInspector] public bool isElite;

    AIEntity _entity;

    private void Awake()
    {
        TryGetComponent(out _entity);
    }

    public void ApplyEliteStats(ref List<PremadeSpell> boostedSpells, PremadeSpell[] premadeSpells)
    {
        print("GROS ELITE SA MERE" + "" + _entity.Data.name);

        try
        {
            _eliteEffect.Play();
        }catch(Exception e) { Debug.LogException(e); }


        foreach (PremadeSpell premadeSpell in premadeSpells)
        {
            SpellData boostedSpell = premadeSpell.SpellData.Copy();

            for (int i = 0; i < boostedSpell.Effects.Count; i++)
            {
                SpellEffect effect = boostedSpell.Effects[i];
                effect.value *= (1 + EliteFactor);
                print("new effect value for" + " " + boostedSpell.Name + " : " + effect.effectType.ToString() + " " + effect.value);
            }

            boostedSpell.CoolDown = Mathf.RoundToInt(boostedSpell.CoolDown * (1-EliteFactor));
            boostedSpell.Range = Mathf.RoundToInt(boostedSpell.Range * (1 +EliteFactor));

            PremadeSpell newSpell = new();
            newSpell.SpellData = boostedSpell;
            newSpell.spellType = premadeSpell.spellType;

            boostedSpells.Add(newSpell);
        }

        _entity.stats.Setup(_entity.Data.MaxHealth * (1 +EliteFactor), _entity.Data.MaxHealth * (1 + EliteFactor));
        _entity.stats.maxMovePoints = Mathf.RoundToInt(_entity.Data.MaxMovePoints * (1 + EliteFactor));
    }
}
