using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.IO;
using UnityEngine;

public class VerifySaveFolder : MonoBehaviour
{
    void Start()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");

        if (files.Length == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void TriggerDesactivation()
    {
        DesactivateAsync().Forget();
    }

    public async UniTaskVoid DesactivateAsync()
    {
        await gameObject.transform.DOScale(0, 0.5f).ToUniTask();
        gameObject.SetActive(false);
    }
}
