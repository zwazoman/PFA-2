using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [Header("Lightning")]
    [SerializeField] Texture2D _gradientMap;
    [SerializeField][Range(0,1)] float _enviroID;

    [Header("Stippling")]
    [SerializeField] Texture2D _stippling;
    [SerializeField] float _stipplingTiling = 1;

    public void UpdateLightningInfo()
    {
        Shader.SetGlobalTexture("_lightGradientMap", _gradientMap);
        Shader.SetGlobalTexture("_stippling", _stippling);
        Shader.SetGlobalFloat("_enviroID", _enviroID);
        Shader.SetGlobalFloat("_stipplingTiling", _stipplingTiling);
    }

    private void OnValidate()
    {
        UpdateLightningInfo();
    }
}
