using UnityEngine;

[CreateAssetMenu(fileName = "new spell", menuName = "Combat/PremadeSpell")]
public class PremadeSpell : ScriptableObject
{
    public SpellType spellType = SpellType.Attack;
    public SpellData SpellData;
}
