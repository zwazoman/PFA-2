Shader "Unlit/shdr_test_unlit"
{
    Properties
    {
        _Palette ("Texture", 2D) = "grey" {}
        //_GradientID ("GradientID", Float) = 0
        [KeywordEnum(MAP0,MAP1,MAP2)] _SECONDUV ("use second uv map", Float) = 0
    }
    SubShader
    {
        Pass
        {
            Name "MainPass"
            Tags { "RenderType"="Opaque"  "LightMode"="ForwardBase" "PassFlags" = "OnlyDirectional"}
            LOD 100

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog


            //for shadow casting
            #pragma multi_compile_fwdbase
            #pragma shader_feature_local _SECONDUV_MAP0 _SECONDUV_MAP1 _SECONDUV_MAP2
            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"
            
            #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"
            #include "UnityShadowLibrary.cginc"
            #include "utils\Nathan.cginc"

            

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

                #if _SECONDUV_MAP0
                float2 uv : TEXCOORD0;
                #endif
                #if _SECONDUV_MAP1
                float2 uv : TEXCOORD1;
                #endif

                #if _SECONDUV_MAP2
                float2 uv : TEXCOORD2;
                #endif  
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;


                UNITY_FOG_COORDS(1)
                float3 normal : NORMAL;
                float4 pos : POSITION;

                unityShadowCoord4 _ShadowCoord : TEXCOORD3; // put shadows data into TEXCOORD1

            };

            sampler2D _Palette;
            sampler2D _lightGradientMap;
            float _enviroID;



            //vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _Palette);
                o.normal = UnityObjectToWorldNormal( v.normal);
                o.uv = v.uv;


                //compute fog data
                UNITY_TRANSFER_FOG(o,o.pos);
                
                // compute shadows data
                TRANSFER_SHADOW(o);

                return o;
            }

            //fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(1,1,1,0);

                // sample the texture

                col = tex2D(_Palette, i.uv);


                //lightning
                fixed lambert = dot(_WorldSpaceLightPos0,i.normal)*.5+.5;
                fixed castShadow = SHADOW_ATTENUATION(i);

                float shadow = saturate(/*sign*/(lambert) * castShadow )*.8+.1;
                shadow = shadow - shadow % (.2);

                float4 coloredShadow = (saturate(shadow)*.2+.8)* lerp(shadow, tex2D(_lightGradientMap,float2(shadow,_enviroID)),1);                
                
                col *= coloredShadow;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }


        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        
    }
}
