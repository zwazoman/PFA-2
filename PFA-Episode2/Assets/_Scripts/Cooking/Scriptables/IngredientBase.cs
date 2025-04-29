using Unity.VisualScripting;
using UnityEngine;

public abstract class IngredientBase : ScriptableObject
{
    [Header("Ingredient")]
    public string name;
    public Sprite sprite;
    public abstract void ModifySpellEffect(SpellData Spell);
    public virtual void OnAfterModifySpellEffect(SpellData Spell) { }
}
