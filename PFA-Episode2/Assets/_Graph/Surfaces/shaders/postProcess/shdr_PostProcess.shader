Shader "Unlit/shdr_PostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _outlineRadius ("_outlineRadius", Float) = 0.01
        _outlineTresholdOffset ("_outlineRadius", Float) = 0.15
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;
            float2 _resolution;
            float _outlineRadius;
            float _outlineTresholdOffset;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                

                float rx = _outlineRadius;
                float ry = rx * _resolution.x / _resolution.y;
                float d1 = Linear01Depth (tex2D(_CameraDepthTexture, i.uv + float2(rx,0)));
                float d2 = Linear01Depth (tex2D(_CameraDepthTexture, i.uv - float2(rx,0)));
                float d3 = Linear01Depth (tex2D(_CameraDepthTexture, i.uv + float2(0,ry)));
                float d4 = Linear01Depth (tex2D(_CameraDepthTexture, i.uv - float2(0,ry)));
                float outline = max(abs(d1-d2),abs(d3-d4));
                outline = saturate(saturate(outline)*1000 - _outlineTresholdOffset)*100000;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(col,col*col*.2, saturate(outline));
            }
            ENDCG
        }
    }
}
