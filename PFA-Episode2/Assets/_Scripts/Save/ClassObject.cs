using static NodeTypes;
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SerializableNode
{
    public Vector3Int key;
    public int position;
    public int hauteur;
    public NodesEventTypes eventName;
    public bool onYReviendra;

    //référence au créateur
    public Vector3Int creatorKey;

    //information Player
    public Vector3Int playerPosition;
    public int PositionMap;
}

[Serializable]
public class SerializableLink
{
    // pour la poc mais vaut mieux changer pour après avec des ref au parent directement
    public List<Vector3> transformLink;
    public Quaternion rotationLink;
}

[Serializable]
public class MapWrapper
{
    public List<SerializableNode> nodes = new();
    public List<SerializableLink> links = new();
}

[Serializable]
public class InventoryData
{
    // Savoir si c'est un Spell, un Ingredient ou une Sauce
    public string type;

    // Dans le cas d'un Spell juste lui donner son nom, pour les autre permet de trouver les ScriptableObject associé
    public string name;

    // Variable pour le spellData
    public string sprite;
    public string ingredientsCombination;
    public bool isOccludedByWalls;
    public byte range;
    public byte coolDown;
    public List<SpellEffect> effects = new();
    public string areaOfEffect;
}

[Serializable]
public class InventoryWrapper
{
    public List<InventoryData> items = new();
}