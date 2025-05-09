using System.Collections.Generic;
using UnityEngine;

public class SaveReference : MonoBehaviour
{
    #region Singleton
    public static SaveReference Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public List<Sprite> Sprites;
    public List<ScriptableObject> ScriptableObjects; 

    public Sprite GetSprite(string name)
    {
        Sprite sprite = null;

        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i].name == name)
            {
                sprite = Sprites[i];
            }
        }

        return sprite;
    }

    public ScriptableObject GetScriptableObject(string name)
    {
        ScriptableObject scriptableObject = null;

        Debug.Log("bite");
        for (int i = 0; i < ScriptableObjects.Count; i++)
        {
            Debug.Log("bite1");
            if (ScriptableObjects[i].name == name)
            {
                Debug.Log("bite2");
                scriptableObject = ScriptableObjects[i];
                Debug.Log(scriptableObject);
            }
        }

        return scriptableObject;
    }
}
