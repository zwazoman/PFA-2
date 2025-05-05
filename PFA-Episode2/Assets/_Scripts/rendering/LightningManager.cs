using UnityEngine;

public class LightningManager : MonoBehaviour
{
    [SerializeField] Texture2D _gradientMap;
    [SerializeField][Range(0,1)] float _enviroID;

    public void UpdateLightningInfo()
    {
        Shader.SetGlobalTexture("_lightGradientMap", _gradientMap);
        Shader.SetGlobalFloat("_enviroID", _enviroID);
    }

    private void OnValidate()
    {
        UpdateLightningInfo();
    }
}
