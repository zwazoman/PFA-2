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
    private int _numberLink = 0;

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

            List<Vector3> list = new()
            {
                MapBuildingTools.Instance._savePath[_numberLink].transform.localPosition,
                MapBuildingTools.Instance._savePath[_numberLink].transform.localScale
            };

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

                    transformLink = list,
                    rotationLink = MapBuildingTools.Instance._savePath[_numberLink].transform.rotation,

                    // Sauvegarde la clé du créateur ou Vector3Int.zero si null
                    creatorKey = node.Creator != null ? MapBuildingTools.Instance.GetKeyFromNode(node.Creator) : Vector3Int.zero
                };

                wrapper.nodes.Add(snode);
            }

            _numberLink++;
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

                MapBuildingTools.Instance._trailList[_numberLink].transform.localPosition = item.transformLink[0];
                MapBuildingTools.Instance._trailList[_numberLink].transform.localRotation = item.rotationLink;
                MapBuildingTools.Instance._trailList[_numberLink].transform.localScale = item.transformLink[1];
                MapBuildingTools.Instance._savePath.Add(MapBuildingTools.Instance._trailList[_numberLink]);
                MapBuildingTools.Instance._savePath[_numberLink].gameObject.SetActive(true);

                //Place le joueur sur le bon node
                if (Vector3Int.Distance(item.key, item.playerPosition) <= 1f)
                {
                    //PlayerMap.Instance.SetupTarget(node.transform.position);
                    PlayerMap.Instance.transform.localPosition = item.playerPosition;
                    PlayerMap.Instance.PositionMap = item.PositionMap;
                }

                _numberLink++;
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
            //AdoptChild(tempDico);
            Node.TriggerMapCompleted(); // Redéclenche l'affichage des sprites

            // Redessiner les traits entre les nodes
            /*if (MapBuildingTools.Instance != null)
            {
                MapBuildingTools.Instance.FirstTimeDraw = true;
                foreach (Node node in tempDico.Values)
                {
                    if (node.Creator != null)
                    {
                        MapBuildingTools.Instance.TraceTonTrait(node.Creator, node);
                    }
                }
            }*/

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

    /*private void AdoptChild(Dictionary<Vector3Int, Node> nodeDictionary)
    {
        // Trouve tous les créateurs
        HashSet<Node> allCreators = new();

        foreach (Node node in nodeDictionary.Values)
        {
            if (node.Creator != null)
            {
                allCreators.Add(node.Creator);
            }
        }

        // Liste les node qui ne sont pas créateur
        List<Node> Adopter = new();

        foreach (Node node in nodeDictionary.Values)
        {
            if (!allCreators.Contains(node) && node.Position < byte.MaxValue)
            {
                Adopter.Add(node);
            }
        }

        // Chaque créateur qui n'as pas d'enfant cherche un enfant
        foreach (Node potentialParent in Adopter)
        {
            Node bestChild = null;
            float minHauteurDiff = float.MaxValue;

            foreach (Node potentialChild in nodeDictionary.Values)
            {
                if (potentialChild == potentialParent) continue;

                if (potentialChild.Position == potentialParent.Position + 1)
                {
                    float hauteurDiff = Mathf.Abs(potentialChild.Hauteur - potentialParent.Hauteur);
                    if (hauteurDiff < minHauteurDiff)
                    {
                        minHauteurDiff = hauteurDiff;
                        bestChild = potentialChild;
                    }
                }
            }

            // Relie
            if (bestChild != null)
            {
                if (MapBuildingTools.Instance != null)
                {
                    MapBuildingTools.Instance.TraceTonTrait(potentialParent, bestChild);
                }
            }
        }
    }*/
}
