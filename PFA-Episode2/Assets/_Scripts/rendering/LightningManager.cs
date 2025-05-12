using System;
using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [Header("Lightning")]
    [SerializeField] Texture2D _gradientMap;
    [SerializeField] [Range(0,1)] float _enviroID;
    [SerializeField] float _bands = 5;

    [Header("Stippling")]
    [SerializeField] Texture2D _stippling;
    [SerializeField] float _stipplingTiling = 1;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void onSceneLoad()
    {
        try
        {
            ((LightningManager)FindAnyObjectByType(typeof(LightningManager))).UpdateLightningInfo();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }


    public void UpdateLightningInfo()
    {
        Shader.SetGlobalTexture("_lightGradientMap", _gradientMap);
        Shader.SetGlobalTexture("_stippling", _stippling);
        Shader.SetGlobalFloat("_enviroID", _enviroID);
        Shader.SetGlobalFloat("_bands", _bands);
        Shader.SetGlobalFloat("_stipplingTiling", _stipplingTiling);
    }

    private void OnValidate()
    {
        UpdateLightningInfo();
    }

    private void Start()
    {
        UpdateLightningInfo();
    }


}
