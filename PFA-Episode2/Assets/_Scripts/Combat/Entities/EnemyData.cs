using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field:SerializeField] 
    public string EntityName { get; private set; }

    [field : SerializeField]
    public AIBehaviour aiBehaviour { get; private set; }

    [field : SerializeField]
    public int MaxHealth { get; private set; }

    [field : SerializeField]
    public int MaxMovePoints { get; private set; }

    public PremadeSpell[] Spells;
}
