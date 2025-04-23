using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAreaOfEffect", menuName = "Combat/AreaOfEffect")]
public class AreaOfEffect : ScriptableObject
{
    /// <summary>
    /// utilisé pour savoir la taille du canvas quand on dessine le sort dans la window. Le lançeur de sort est placé en 0,0
    /// </summary>
    public Rect Bounds = new Rect(-5, -5, 11, 11);

    /// <summary>
    /// Tuiles affecté par une capacité, relativement au bonhomme qui la lance
    /// </summary>
    public List<Vector2Int> AffectedTiles = new ();

}