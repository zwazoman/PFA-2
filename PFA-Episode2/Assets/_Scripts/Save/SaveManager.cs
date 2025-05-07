using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static int Number = 0;

    // Sauvegarde un objet ISavable
    public static void Save<T>(byte saveFileID) where T : ISavable<T>
    {
        InventoryWrapper inventoryWrapper = new();

        foreach (SpellData spd in GameManager.Instance.playerInventory.Spells)
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


        foreach (Ingredient ing in GameManager.Instance.playerInventory.Ingredients)
        {
            Ingredient ingredient = ing;

            InventoryData sdata = new()
            {
                type = "Ingredient",
                name = ingredient.name,
            };

            inventoryWrapper.items.Add(sdata);
        }

        foreach (Sauce sc in GameManager.Instance.playerInventory.Sauces)
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

        if (SaveMapGeneration.Instance.Encrypt)
        {
            string encryptedJson = SaveMapGeneration.Instance.EncryptDecrypt(json);
            File.WriteAllText(path, encryptedJson);
        }
        else
        {
            File.WriteAllText(path, json);
        }
    }

    // Charge un objet ISavable
    public static Inventory Load(byte saveFileID)
    {
        string path = Application.persistentDataPath + $"/save_{saveFileID}.json";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Fichier de sauvegarde introuvable à {path}");
        }

        string json = File.ReadAllText(path);

        if (SaveMapGeneration.Instance.Encrypt)
        {
            json = SaveMapGeneration.Instance.EncryptDecrypt(json);
        }

        InventoryWrapper inventoryWrapper = JsonUtility.FromJson<InventoryWrapper>(json);

        Inventory inventory = new();

        foreach (InventoryData item in inventoryWrapper.items)
        {
            switch (item.type)
            {
                case "Spell":
                    SpellData spell = new()
                    {
                        Name = item.name,
                        Sprite = SaveReference.Instance.GetSprite(item.sprite),
                        IngredientsCombination = item.ingredientsCombination,
                        IsOccludedByWalls = item.isOccludedByWalls,
                        Range = item.range,
                        CoolDown = item.coolDown,
                        Effects = item.effects,
                        AreaOfEffect = (AreaOfEffect)SaveReference.Instance.GetScriptableObject(item.areaOfEffect),
                    };
                    inventory.Spells.Add(spell);
                    break;

                case "Ingredient":
                    ScriptableObject ingredient = SaveReference.Instance.GetScriptableObject(item.name);
                    inventory.Ingredients.Add((Ingredient)ingredient);
                    break;

                case "Sauce":
                    ScriptableObject sauce = SaveReference.Instance.GetScriptableObject(item.name);
                    inventory.Sauces.Add((Sauce)sauce);
                    break;

                default:
                    Debug.LogWarning($"Type d'objet inconnu : {item.type}");
                    break;
            }
        }

        Debug.Log($"Inventaire chargé depuis {path} avec {inventoryWrapper.items.Count} objets !");
        return inventory;
    }
}
