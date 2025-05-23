using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "newAreaOfEffect", menuName = "Combat/AreaOfEffect")]
public class AreaOfEffect : ScriptableObject
{
    public Sprite sprite;

#if UNITY_EDITOR
    public void SavePreviewImage()
    {
        Texture2D texture = new((int)Bounds.width,(int)Bounds.height);
        texture.filterMode = FilterMode.Point;

        for(int i = 0; i < texture.width; i++)
        {
            for(int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i,j,AffectedTiles.Contains(new Vector2Int(i+(int)Bounds.xMin,j+ (int)Bounds.yMin)) ? Color.red : Color.white);
            }
        }

        texture.Apply();



        sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            Vector2.zero, 100, 0, SpriteMeshType.FullRect,
            Vector4.zero,
            false);

        AssetDatabase.CreateAsset(texture, "Assets/_Data/AreasOfEffect/Sprites/txtr_" + name + ".asset");
        AssetDatabase.CreateAsset(sprite, "Assets/_Data/AreasOfEffect/Sprites/sprt_" + name+".asset");
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif

    /// <summary>
    /// utilis� pour savoir la taille du canvas quand on dessine le sort dans la window.
    /// </summary>
    public Rect Bounds = new Rect(-5, -5, 11, 11);

    /// <summary>
    /// Tuiles affect�es par une capacit�, relativement au bonhomme qui la lance
    /// </summary>
    public List<Vector2Int> AffectedTiles = new ();



}