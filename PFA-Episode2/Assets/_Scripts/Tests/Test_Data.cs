using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Test_Data
{
    [MenuItem("Data/Validate game data")]
    public static void ValidateGameData()
    {
        Debug.Log("=== Validating Game Data ===");

        #region AOE
        Debug.Log("-- validating : Areas of effect --");
        string[] files = Directory.GetFiles("Assets/_Data/AreasOfEffect", "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            AreaOfEffect AOE = (AreaOfEffect) AssetDatabase.LoadAssetAtPath(path, typeof(AreaOfEffect));
            
            //assets mal rangés
            if ((AOE == null))
            {
                Debug.LogError("Y'a un asset mal rangé ici connard : " + path);
                continue;
            }
            
            //tiles oustide of bounds
            foreach(Vector2Int t in AOE.AffectedTiles)
            {
                if (!AOE.Bounds.Contains(t)) Debug.LogError("Y'a des tiles en dehors des bounds sur cet asset : " + path);
            }

            //bounds max size
            if (AOE.Bounds.width > 7 || AOE.Bounds.height > 7) Debug.LogError("les bounds sont trop gros ici : " + path);

            //null sprite
            if (AOE.sprite == null) Debug.LogError("Le sprite est null ici : " + path);

            //null texture
            if (AOE.sprite.texture == null) Debug.LogError("La texture du sprite est null ici : " + path);

        }
        #endregion

        #region Ingredients
        Debug.Log("-- validating : Ingredients --");
        files = Directory.GetFiles("Assets/_Data/Ingredients/ingredients", "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            Ingredient Ing = (Ingredient)AssetDatabase.LoadAssetAtPath(path, typeof(Ingredient));

            //assets mal rangés
            if ((Ing == null))
            {
                Debug.LogError("Y'a un asset mal rangé ici connard : " + path);
                continue;
            }

            //invalid name
            if ((Ing.name == "")) Debug.LogError("cet ingredient n'a pas de nom : " + path);

            //null sprite
            if ((Ing.sprite==null)) Debug.LogError("L'ingredient ici a pas de sprite : " + path);

            //null sprite texture
            if ((Ing.sprite == null)) Debug.LogError("le sprite de cet ingredient a pas de texture : " + path);

            //0 value
            if ((Ing.EffectValue == 0)) Debug.LogError("cet ingredient n'aura pas d'effet car sa valeur est 0 : " + path);
            

        }
        #endregion

        #region Sauces
        Debug.Log("-- validating : Sauces --");
        files = Directory.GetFiles("Assets/_Data/Ingredients/Sauce", "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            Sauce Sauce = (Sauce)AssetDatabase.LoadAssetAtPath(path, typeof(Sauce));

            //assets mal rangés
            if ((Sauce == null))
            {
                Debug.LogError("Y'a un asset mal rangé ici connard : " + path);
                continue;
            }

            //invalid name
            if ((Sauce.name == "")) Debug.LogError("cette sauce n'a pas de nom : " + path);

            //null sprite
            if ((Sauce.sprite == null)) Debug.LogError("la sauce ici a pas de sprite : " + path);

            //null sprite texture
            if ((Sauce.sprite == null)) Debug.LogError("le sprite de cette sauce a pas de texture : " + path);

            //null area of effect
            if ((Sauce.areaOfEffect == null)) Debug.LogError("Cette sauce n'a pas d'Area Of Effect : " + path);

        }
        #endregion

        #region Premade spells
        Debug.Log("-- validating : premade spells --");
        files = Directory.GetFiles("Assets/_Data/PremadeSpells", "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            PremadeSpell asset = (PremadeSpell)AssetDatabase.LoadAssetAtPath(path, typeof(PremadeSpell));

            //assets mal rangés
            if ((asset == null))
            {
                Debug.LogError("Y'a un asset mal rangé ici connard : " + path);
                continue;
            }

            //invalid name
            if ((asset.SpellData.Name == "")) Debug.LogError("ce spell n'a pas de nom : " + path);

            //null sprite
            if ((asset.SpellData.Sprite == null)) Debug.LogError("ce spell n'a pas de sprite : " + path);

            //null sprite texture
            if ((asset.SpellData.Sprite.texture == null)) Debug.LogError("le sprite de ce spell n'a pas de texture : " + path);

            //effects
            if (asset.SpellData.Effects.Count == 0) Debug.LogError("Ce spell n'a pas d'effet : " + path);
            foreach(SpellEffect effect in asset.SpellData.Effects) if (effect.value == 0) Debug.LogError("Ce spell a un effet avec une valeur de 0 : " + path);

            //null area of effect
            if ((asset.SpellData.AreaOfEffect == null)) Debug.LogError("ce spell n'a pas d'Area of effect : " + path);

        }
        #endregion


        #region Entities
        Debug.Log("-- validating : entities --");
        files = Directory.GetFiles("Assets/_Data/Entitites", "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string path in files)
        {
            EnemyData Enemy = (EnemyData)AssetDatabase.LoadAssetAtPath(path, typeof(EnemyData));

            //assets mal rangés
            if ((Enemy == null))
            {
                Debug.LogError("Y'a un asset mal rangé ici connard : " + path, Enemy);
                continue;
            }

            //invalid name
            if ((Enemy.EntityName == "")) Debug.LogError("cet ennemi n'a pas de nom : " + path,Enemy);

            //0 health
            if (Enemy.MaxHealth <= 0) Debug.LogError("Cet ennemi a 0 HP connard. " + path,Enemy);

            //0 movement
            if (Enemy.MaxMovePoints <= 0) Debug.LogError("Cet ennemi a 0 move points. " + path,Enemy);

            //spells
            if (Enemy.Spells.Length == 0) Debug.LogError("Cet ennemi a 0 spells. " + path, Enemy);
            foreach(PremadeSpell s in Enemy.Spells) if(s== null) Debug.LogError("Cet ennemi a une nullref dans ses spells. " + path, Enemy);
        }
        #endregion

        Debug.Log("=== Data Validation Complete ! ===");


    }
}
