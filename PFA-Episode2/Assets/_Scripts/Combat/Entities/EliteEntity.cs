using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EliteEntity : MonoBehaviour
{
    public const float EliteFactor = 1.3f;

    [SerializeField] VisualEffect _eliteEffect;

    [HideInInspector] public bool isElite;

    AIEntity _entity;

    private void Awake()
    {
        TryGetComponent(out _entity);
    }

    public void ApplyEliteStats(ref List<PremadeSpell> boostedSpells, PremadeSpell[] premadeSpells)
    {
        print("GROS ELITE SA MERE");
        try
        {
            _eliteEffect.Play();
        }catch(Exception e) { Debug.LogException(e); }

        foreach (PremadeSpell premadeSpell in premadeSpells)
        {
            PremadeSpell boostedSpell = premadeSpell.Copy();

            for (int i = 0; i < boostedSpell.SpellData.Effects.Count; i++)
            {
                SpellEffect effect = boostedSpell.SpellData.Effects[i];
                effect.value *= EliteFactor;
            }

            boostedSpell.SpellData.CoolDown = Mathf.RoundToInt(boostedSpell.SpellData.CoolDown * 1-EliteFactor);
            boostedSpell.SpellData.Range = Mathf.RoundToInt(boostedSpell.SpellData.Range * EliteFactor);

            boostedSpells.Add(boostedSpell);
        }

        _entity.stats.Setup(_entity.stats.maxHealth * EliteFactor, _entity.stats.maxHealth * EliteFactor);
        _entity.stats.maxMovePoints = Mathf.RoundToInt(_entity.stats.maxMovePoints * EliteFactor);

    }
}
