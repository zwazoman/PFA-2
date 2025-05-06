using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static int Number = 0;

    // Sauvegarde un objet ISavable
    public static void Save<T>(T savableObject, byte saveFileID) where T : ISavable<T>
    {
        InventoryWrapper inventoryWrapper = new();

        foreach (SpellData spd in Inventory.Instance.Spells)
        {
            SpellData spell = spd;

            InventoryData sdata = new()
            {
                type = "Spell",
                name = spell.Name,
                sprite = spell.Sprite.name,
                ingredientsCombination = spell.IngredientsCombination,
                isOccludedByWalls = spell.IsOccludedByWalls,
                range = spell.Range,
                coolDown = spell.CoolDown,
                effects = spell.Effects,
                areaOfEffect = spell.AreaOfEffect.name,
            };

            inventoryWrapper.items.Add(sdata);
        }


        foreach (Ingredient ing in Inventory.Instance.Ingredients)
        {
            Ingredient ingredient = ing;

            InventoryData sdata = new()
            {
                type = "Ingredient",
                name = ingredient.name,
            };

            inventoryWrapper.items.Add(sdata);
        }

        foreach (Sauce sc in Inventory.Instance.Sauces)
        {
            Sauce sauce = sc;

            InventoryData sdata = new()
            {
                type = "Sauce",
                name = sauce.name,
            };

            inventoryWrapper.items.Add(sdata);
        }

        string json = JsonUtility.ToJson(inventoryWrapper, true);
        string path = Application.persistentDataPath + $"/save_{saveFileID}.json";
        File.WriteAllText(path, json);
    }

    // Charge un objet ISavable
    public static T Load<T>(byte saveFileID) where T : ISavable<T>, new()
    {
        string path = Application.persistentDataPath + $"/save_{saveFileID}.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Fichier de sauvegarde introuvable à {path}");
            return new T();
        }

        string json = File.ReadAllText(path);
        InventoryWrapper inventoryWrapper = JsonUtility.FromJson<InventoryWrapper>(json);

        ClearAll();

        foreach (var item in inventoryWrapper.items)
        {
            switch (item.type)
            {
                case "Spell":
                    SpellData spell = new()
                    {
                        Name = item.name,
                        Sprite = Resources.Load<Sprite>($"Sprites/{item.sprite}"),
                        IngredientsCombination = item.ingredientsCombination,
                        IsOccludedByWalls = item.isOccludedByWalls,
                        Range = item.range,
                        CoolDown = item.coolDown,
                        Effects = item.effects,
                        AreaOfEffect = item.areaOfEffect,
                    };
                    Inventory.Instance.Spells.Add(spell);
                    break;

                case "Ingredient":
                    Ingredient ingredient = new() { name = item.name };
                    Inventory.Instance.Ingredients.Add(ingredient);
                    break;

                case "Sauce":
                    Sauce sauce = new() { name = item.name };
                    Inventory.Instance.Sauces.Add(sauce);
                    break;

                default:
                    Debug.LogWarning($"Type d'objet inconnu : {item.type}");
                    break;
            }
        }

        Debug.Log($"Inventaire chargé depuis {path} avec {inventoryWrapper.items.Count} objets !");

        return new T();
    }

    static void ClearAll()
    {
        Inventory.Instance.Spells.Clear();
        Inventory.Instance.Ingredients.Clear();
        Inventory.Instance.Sauces.Clear();
    }
}
