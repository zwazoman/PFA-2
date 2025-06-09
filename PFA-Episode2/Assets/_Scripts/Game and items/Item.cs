using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item")]
    public string name;
    public Sprite sprite;
    public Rarity rarity;
    public Sounds dragSound;

}
