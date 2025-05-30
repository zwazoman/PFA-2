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
        Texture2D texture = new(50,50);
        texture.filterMode = FilterMode.Point;

        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                for(int x = 0; x < 10; x++)
                {
                    for(int y = 0; y < 10; y++) 
                    { 
                        texture.SetPixel(i*10+x,j*10+y,
                            (AffectedTiles.Contains(new Vector2Int(i-2,j-2)) && x>1 && x<9 && y>1 && y<9)
                            
                            ? Color.white : new Color(1,0,0,0));
                    }
                }
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
    /// utilisé pour savoir la taille du canvas quand on dessine le sort dans la window.
    /// </summary>
    public Rect Bounds = new Rect(-5, -5, 11, 11);

    /// <summary>
    /// Tuiles affectées par une capacité, relativement au bonhomme qui la lance
    /// </summary>
    public List<Vector2Int> AffectedTiles = new ();



}