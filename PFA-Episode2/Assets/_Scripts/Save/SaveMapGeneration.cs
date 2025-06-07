using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Script qui gère la sauvegarde la map
/// </summary>
public class SaveMapGeneration : MonoBehaviour
{
    [Tooltip("Sauvegarde crypter ou non")] public bool Encrypt = true;
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

        if (PlayerMap.Instance == null)
        {
            Debug.LogWarning("PlayerMap.Instance est null, impossible de sauvegarder la position du joueur.");
            return;
        }

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

            List<SerializableTransform> links = new();

            for (int i = 0; i < node.PathBetweenNode.Count; i++)
            {
                if (node.PathBetweenNode[i] == null) continue;

                List<Vector3> list = new()
                {
                    node.PathBetweenNode[i].transform.localPosition,
                    node.PathBetweenNode[i].transform.localScale
                };

                SerializableTransform linkObj = new()
                {
                    PosiScale = list,
                    rotation = node.PathBetweenNode[i].transform.localRotation
                };

                links.Add(linkObj);
            }
            //foreach (Node n in node.Children)
            //{
            //    List<SerializableNode> childList = new();
            //    SerializableNode child = new()
            //    {
            //        n,
            //    };
            //    childList.Add(child);

            //}
            SerializableNode snode = new()
            {
                key = kvp.Key,
                position = node.Position,
                hauteur = node.Hauteur,
                eventName = node.EventName,
                onYReviendra = node.OnYReviendra,
                creatorKey = node.Creator != null ? MapBuildingTools.Instance.GetKeyFromNode(node.Creator) : Vector3Int.zero,
                paths = links

            };

            wrapper.nodes.Add(snode);
        }

        SerializableSeed seed = new()
        {
            useSeed = SpawnGround.Instance.UseSeed,
            seed = SpawnGround.Instance.Seed
        };

        wrapper.seed = seed;

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

            if (GameManager.Instance.PlayerPosMap.PositionMap == 0)
            {
                PlayerMap.Instance.transform.localPosition = wrapper.player.playerPosition;
                PlayerMap.Instance.PositionMap = wrapper.player.PositionMap;
                PlayerMap.Instance.Y = wrapper.player.Y;
            }
            else
            {
                SerializablePlayer player = GameManager.Instance.PlayerPosMap;

                PlayerMap.Instance.transform.localPosition = player.playerPosition;
                PlayerMap.Instance.PositionMap = player.PositionMap;
                PlayerMap.Instance.Y = player.Y;
            }

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

                tempDico[item.key] = node;

                foreach (SerializableTransform subItem in item.paths)
                {
                    GameObject Path = MapBuildingTools.Instance.TrueListPath[0];
                    MapBuildingTools.Instance.TrueListPath.RemoveAt(0);

                    Path.transform.localPosition = subItem.PosiScale[0];
                    Path.transform.localRotation = subItem.rotation;
                    Path.transform.localScale = subItem.PosiScale[1];
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

            SpawnGround.Instance.UseSeed = wrapper.seed.useSeed;
            SpawnGround.Instance.Seed = wrapper.seed.seed;

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
            MapMaker2.Instance.AllNodeGood = new List<Node>(tempDico.Values);
            SpawnGround.Instance.StartSpawnRiver();
            Node.TriggerMapCompleted(); // Redéclenche l'affichage des sprites
            SaveMap();
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
