using UnityEngine;

[CreateAssetMenu(fileName = "new spell", menuName = "Combat/PremadeSpell")]
public class PremadeSpell : ScriptableObject
{
    public SpellType spellType = SpellType.Attack;
    public SpellData SpellData;
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        bool utilitary = false;
        foreach (var effect in SpellData.Effects)
        {
            if (effect.effectType == SpellEffectType.EntitySummon)
            {
                utilitary = true;
                break;
            }
        }

        SpellData.IsUtilitary = utilitary;
    }
#endif
}
