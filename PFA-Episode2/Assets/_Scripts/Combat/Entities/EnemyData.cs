using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Entities")]
public class EnemyData : ScriptableObject
{
    [field:SerializeField] 
    public string EntityName;

    [field : SerializeField]
    public int MaxHealth { get; private set; }

    [field : SerializeField]
    public int MaxMovePoints { get; private set; }

    public SpellData[] Spells;
}
