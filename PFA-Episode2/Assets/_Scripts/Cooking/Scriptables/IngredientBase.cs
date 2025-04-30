public abstract class IngredientBase : Item
{
    public abstract void ModifySpellEffect(SpellData Spell);
    public virtual void OnAfterModifySpellEffect(SpellData Spell) { }
}
