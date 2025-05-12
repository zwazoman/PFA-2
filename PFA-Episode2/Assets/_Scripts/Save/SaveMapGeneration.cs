using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Script qui gère la sauvegarde la map
/// </summary>
public class SaveMapGeneration : MonoBehaviour
{
    [Tooltip("Sauvegarde crypter ou non")] public bool Encrypt;
    [Tooltip("Numéro de fichier de sauvegarde")] public byte SaveID;
    const string ENCRYPT_KEY = "Tr0mp1ne7te";
    private int _numberLink = 0;
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

        foreach (KeyValuePair<Vector3Int, Node> kvp in MapMaker2.Instance.DicoNode)
        {
            Node node = kvp.Value;

            // Ne sauvegarde pas le Startnode
            if (node.Position != 0)
            {
                SerializableNode snode = new()
                {
                    key = kvp.Key,
                    position = node.Position,
                    hauteur = node.Hauteur,
                    eventName = node.EventName,
                    onYReviendra = node.OnYReviendra,
                    playerPosition = Vector3Int.RoundToInt(PlayerMap.Instance.transform.localPosition),
                    PositionMap = PlayerMap.Instance.PositionMap,

                    // Sauvegarde la clé du créateur ou Vector3Int.zero si null
                    creatorKey = node.Creator != null ? MapBuildingTools.Instance.GetKeyFromNode(node.Creator) : Vector3Int.zero
                };

                wrapper.nodes.Add(snode);
            }
        }

        foreach (Image link in MapBuildingTools.Instance._savePath)
        {
            //Image link = image;
            List<Vector3> list = new()
            {
                link.transform.localPosition,
                link.transform.localScale,
            };

            SerializableLink linkObj = new()
            {
                transformLink = list,
                rotationLink = link.transform.localRotation
            };

            wrapper.links.Add(linkObj);
        }

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

        _numberLink = 0;
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
                node.gameObject.SetActive(true);

                //Place le joueur sur le bon node
                if (Vector3Int.Distance(item.key, item.playerPosition) <= 1f)
                {
                    //PlayerMap.Instance.SetupTarget(node.transform.position);
                    PlayerMap.Instance.transform.localPosition = item.playerPosition;
                    PlayerMap.Instance.PositionMap = item.PositionMap;
                    PositionMap = item.PositionMap;

                }
            }

            // Load les link
            foreach (SerializableLink item in wrapper.links)
            {
                Image image = MapBuildingTools.Instance._trailList[_numberLink];

                image.transform.localPosition = item.transformLink[0];
                image.transform.localRotation = item.rotationLink;
                image.transform.localScale = item.transformLink[1];
                image.gameObject.SetActive(true);
                MapBuildingTools.Instance._savePath.Add(image);

                _numberLink++;
            }

            _numberLink = 0;

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

            _numberLink = 0;
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
