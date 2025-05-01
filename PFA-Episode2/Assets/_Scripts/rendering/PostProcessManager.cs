using UnityEngine;

public class PostProcessManager :  MonoBehaviour
{
    [SerializeField] Material postProcessMaterial;

    RenderTexture RT;

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //RT = RenderTexture.GetTemporary(src.width , src.height , 0, RenderTextureFormat.DefaultHDR);

        //Graphics.Blit(src, RT, postProcessMaterial, 0);
        // postProcessMaterial.SetTexture("_CameraDepthTexture", src.depthBuffer);
        // Read pixels from the source RenderTexture, apply the material, copy the updated results to the destination RenderTexture
        postProcessMaterial.SetVector("_resolution", new Vector2(src.width, src.height));
        Graphics.Blit(src, dest, postProcessMaterial);
    }
}
