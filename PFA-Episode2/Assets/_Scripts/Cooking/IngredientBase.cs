using Unity.VisualScripting;
using UnityEngine;

public abstract class IngredientBase : ScriptableObject
{
    public string Name;
    public abstract void ModifySpellEffect(SpellData Spell);
    public virtual void OnAfterModifySpellEffect(SpellData Spell) { }
}
