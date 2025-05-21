using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class IconRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera _renderCamera;
    [SerializeField] ComputeShader _postProcessShader;

    [Header("image post processing")]
    [SerializeField] private int _outlinePixelSize;
    [SerializeField] private Color _outlineColor;
    [SerializeField] private float _exposureMultiplier = 1;

    [Header("generated files")]
    [SerializeField] private int _resolution = 256;
    [SerializeField] public string _spriteSuffix = "sprt_rendered_icon_";
    [SerializeField] public string _textureSuffix = "txtr_rendered_icon_test";

    public const string TexturePath = "Assets/_Graph/UI/Sprites/Icons/Renders/Textures/";
    public const string SpritePath = "Assets/_Graph/UI/Sprites/Icons/Renders/";

    public void resetSuffixes()
    {
        _spriteSuffix = "sprt_rendered_icon_";
        _textureSuffix = "txtr_rendered_icon_";
    }

    /*public void RenderCurrentImage(string spriteName,string textureName)
    {
        RenderTexture rt = new(_resolution, _resolution, 0, RenderTextureFormat.ARGB32);
        rt.enableRandomWrite = true;
        rt.antiAliasing = 3; 
        rt.Create();

        RenderCurrentImage(rt, spriteName,textureName);

        rt.Release();
        DestroyImmediate(rt);
    }*/

    public void RenderCurrentImage(RenderTexture source, RenderTexture Result,string spriteName,string textureName)
    {
        Debug.Log("  - Rendering : " + spriteName);

        string texturePath = TexturePath + _textureSuffix + textureName + ".asset";
        string spritePath  = SpritePath + _spriteSuffix + spriteName + ".asset";

        //create or load output texture
        Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
        bool NoValidTextureAlreadyExists = t == null || t.width != _resolution || t.height != _resolution;
        if (NoValidTextureAlreadyExists) t = new(_resolution, _resolution);

        //render image
        _renderCamera.targetTexture = source;
        _renderCamera.depthTextureMode = DepthTextureMode.Depth;
        _renderCamera.Render();

        //apply icon post process
        _postProcessShader.SetTexture(0, "Source", source);
        _postProcessShader.SetTexture(0, "Result", Result);
        _postProcessShader.SetInt("OutlinePixelSize", _outlinePixelSize);
        _postProcessShader.SetFloat("ExposureMultiplier", _exposureMultiplier);
        _postProcessShader.SetVector("OutlineColor", _outlineColor);
        _postProcessShader.Dispatch(0, Result.width / 8, Result.height / 8, 1);

        //copy render to texture
        RenderTexture.active = Result;
        t.ReadPixels(new Rect(0, 0, Result.width, Result.height), 0, 0);
        t.Apply();


        //save output texture to disk
        if (NoValidTextureAlreadyExists)
        {
             AssetDatabase.CreateAsset(t, texturePath);
        }
        else
        {
            AssetDatabase.SaveAssetIfDirty(t);
        }

        //check if valid sprite already exists
        Sprite existingSprite = (Sprite)AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
        bool NoValidSpriteAlreadyExists = existingSprite == null || existingSprite.texture != t;

        //create new sprite if needed
        if (NoValidSpriteAlreadyExists || NoValidTextureAlreadyExists)
        {
            //create sprite
            Sprite sprite = Sprite.Create(
                t,
                new Rect(0, 0, t.width, t.height), 
                Vector2.zero, _resolution+1, 0, SpriteMeshType.FullRect,
                Vector4.zero,
                false);

            AssetDatabase.CreateAsset(sprite, spritePath);
        }

        //reset camera
        _renderCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    public async void RenderAllObjects()
    {
        Debug.Log("-- Beginning to render all child objects --");
        

        //disable all objects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        //initialize render texture
        RenderTexture result = new(_resolution, _resolution, 1, RenderTextureFormat.ARGB32);
        result.enableRandomWrite = true;
        result.Create();

        RenderTexture source = new(_resolution, _resolution, 1, RenderTextureFormat.ARGB32);
        source.enableRandomWrite = true;
        source.Create();

        try
        {
            //render every object
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(true);

                //await Task.Delay(1000);

                await Task.Yield();

                RenderCurrentImage(source,result, child.name, child.name) ;
                child.SetActive(false);
            }
        } catch ( Exception e) { RenderTexture.active = null; Debug.LogException(e); } 


        //release render texture 
        result.Release();
        DestroyImmediate(result);

        source.Release();
        DestroyImmediate(source);

        Debug.Log("-- render finished. --");
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(IconRenderer))]
public class IconRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("reset suffixes"))
        {
            ((IconRenderer)target).resetSuffixes();
        }

       

        GUILayout.Space(15);



        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Images will be saved at : " + IconRenderer.SpritePath, EditorStyles.helpBox);
        EditorGUILayout.EndVertical();


        if (GUILayout.Button("render all objects"))
        {
            ((IconRenderer)target).RenderAllObjects();
        }

        /*if (GUILayout.Button("render current view as test"))
        {
            ((IconRenderer)target).RenderCurrentImage("test", "test");
        }*/
        
    }
}

#endif

#endif