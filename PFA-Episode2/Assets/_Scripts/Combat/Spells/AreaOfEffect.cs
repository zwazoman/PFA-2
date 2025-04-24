using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAreaOfEffect", menuName = "Combat/AreaOfEffect")]
public class AreaOfEffect : ScriptableObject
{
    /// <summary>
    /// utilis� pour savoir la taille du canvas quand on dessine le sort dans la window. Le lan�eur de sort est plac� en 0,0
    /// </summary>
    public Rect Bounds = new Rect(-5, -5, 11, 11);

    /// <summary>
    /// Tuiles affect� par une capacit�, relativement au bonhomme qui la lance
    /// </summary>
    public List<Vector2Int> AffectedTiles = new ();

}