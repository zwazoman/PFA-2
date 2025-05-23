using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Script qui gère la sauvegarde la map
/// </summary>
public class SaveMapGeneration : MonoBehaviour
{
    [Tooltip("Sauvegarde crypter ou non")] public bool Encrypt;
    [Tooltip("Numéro de fichier de sauvegarde")] public byte SaveID;
    const string ENCRYPT_KEY = "Tr0mp1ne7te";
    public int PositionMap { get; private set; }

    #region Singleton
    public static SaveMapGeneration Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    // Fonction qui sauvegarde toutes les infos relatives au nodes
    public void SaveMap()
    {
        MapWrapper wrapper = new();

        SerializablePlayer player = new()
        {
            playerPosition = Vector3Int.RoundToInt(PlayerMap.Instance.transform.localPosition),
            PositionMap = PlayerMap.Instance.PositionMap,
            Y = PlayerMap.Instance.Y
        };

        wrapper.player = player;

        foreach (KeyValuePair<Vector3Int, Node> kvp in MapMaker2.Instance.DicoNode)
        {
            Node node = kvp.Value;

            // Ne sauvegarde pas le Startnode
            if (node.Position != 0)
            {
                List<SerializableLink> links = new();

                System.Collections.IList link = node.PathBetweenNode;
                for (int i = 0; i < node.PathBetweenNode.Count; i++)
                {
                    List<Vector3> list = new()
                    {
                        node.PathBetweenNode[i].transform.localPosition,
                        node.PathBetweenNode[i].transform.localScale
                    };

                    SerializableLink linkObj = new()
                    {
                        transformLink = list,
                        rotationLink = node.PathBetweenNode[i].transform.localRotation
                    };

                    links.Add(linkObj);
                }

                SerializableNode snode = new()
                {
                    key = kvp.Key,
                    position = node.Position,
                    hauteur = node.Hauteur,
                    eventName = node.EventName,
                    onYReviendra = node.OnYReviendra,
                    Intersection = node.Intersection,
                    Visited = node.Visited,

                    // Sauvegarde la clé du créateur ou Vector3Int.zero si null
                    creatorKey = node.Creator != null ? MapBuildingTools.Instance.GetKeyFromNode(node.Creator) : Vector3Int.zero,

                    paths = links
                };

                wrapper.nodes.Add(snode);
            }
        }

        //for (int i = 0; i < SpawnRiver.Instance.)

        string json = JsonUtility.ToJson(wrapper, true);
        string path = Application.persistentDataPath + $"/MapSave{SaveID}.json";

        if (Encrypt)
        {
            string encryptedJson = EncryptDecrypt(json);
            File.WriteAllText(path, encryptedJson);
        }
        else
        {
            File.WriteAllText(path, json);
        }
    }

    // Fonction pour lire les information contenu dans le fichier json de sauvegarde
    public void LoadMap()
    {
        string path = Application.persistentDataPath + $"/MapSave{SaveID}.json";
        print(path);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            if (Encrypt)
            {
                json = EncryptDecrypt(json);
            }

            MapWrapper wrapper = JsonUtility.FromJson<MapWrapper>(json);

            PlayerMap.Instance.transform.localPosition = wrapper.player.playerPosition;
            PlayerMap.Instance.PositionMap = wrapper.player.PositionMap;
            PlayerMap.Instance.Y = wrapper.player.Y;

            MapMaker2.Instance.DicoNode.Clear();

            Dictionary<Vector3Int, Node> tempDico = new();

            foreach (SerializableNode item in wrapper.nodes)
            {
                Node node = MapMaker2.Instance.NodeList.Dequeue();
                node.transform.localPosition = item.key;
                node.Position = item.position;
                node.Hauteur = item.hauteur;
                node.EventName = item.eventName;
                node.OnYReviendra = item.onYReviendra;
                node.Intersection = item.Intersection;
                node.Visited = item.Visited;

                tempDico[item.key] = node;

                foreach (SerializableLink subItem in item.paths)
                {
                    GameObject Path = MapBuildingTools.Instance.TrueListPath[0];
                    MapBuildingTools.Instance.TrueListPath.RemoveAt(0);

                    Path.transform.localPosition = subItem.transformLink[0];
                    Path.transform.localRotation = subItem.rotationLink;
                    Path.transform.localScale = subItem.transformLink[1];
                    MapBuildingTools.Instance._savePath.Add(Path);

                    node.PathBetweenNode.Add(Path);
                }

                foreach (GameObject obj in node.PathBetweenNode) { obj.SetActive(false); }
                if (node.Position <= PlayerMap.Instance.PositionMap + 3 && node.Position >= PlayerMap.Instance.PositionMap - 1)
                { 
                    node.gameObject.SetActive(true);
                    node.SetupSprite();
                    for (int i = 0; i < node.PathBetweenNode.Count; i++)
                    {
                        node.PathBetweenNode[i].gameObject.SetActive(true);
                    }
                    //foreach (GameObject obj in node.PathBetweenNode) { obj.SetActive(true); }
                }
            }

            // Relie les créateurs une fois que tous les nodes sont instanciés
            foreach (SerializableNode item in wrapper.nodes)
            {
                if (tempDico.ContainsKey(item.key))
                {
                    Node node = tempDico[item.key];

                    // Lie le deuxième node avec le Startnode
                    if (item.position == 1)
                    {
                        node.Creator = MapMaker2.Instance.ParentNode;
                    }
                    else if (item.creatorKey != Vector3Int.zero && tempDico.ContainsKey(item.creatorKey))
                    {
                        node.Creator = tempDico[item.creatorKey];
                    }
                }
            }

            MapMaker2.Instance.DicoNode = tempDico;
            Node.TriggerMapCompleted(); // Redéclenche l'affichage des sprites
        }
        else
        {
            MapMaker2.Instance.StartGenerate();
        }
    }

    // Fonction pour supprimer la sauvegarde
    public void DeleteMap()
    {
        string path = Application.persistentDataPath + $"/MapSave{SaveID}.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    // Fonction pour crypter et décrypter les informations
    public string EncryptDecrypt(string json)
    {
        string result = "";

        for (int i = 0; i < json.Length; i++)
        {
            result += (char)(json[i] ^ ENCRYPT_KEY[i % ENCRYPT_KEY.Length]);
        }

        return result;
    }
}
