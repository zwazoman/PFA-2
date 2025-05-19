using UnityEngine;

[CreateAssetMenu(fileName = "new spell", menuName = "Combat/PremadeSpell")]
public class PremadeSpell : ScriptableObject
{
    public bool bien = false;
    public SpellData SpellData;
}
