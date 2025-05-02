Shader "Unlit/shdr_test_unlit"
{
    Properties
    {
        _Palette ("Texture", 2D) = "grey" {}
        _GradientID ("GradientID", Float) = 0
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                unityShadowCoord4 _ShadowCoord : TEXCOORD2; // put shadows data into TEXCOORD1
                UNITY_FOG_COORDS(1)
                float3 normal : NORMAL;
                float4 pos : POSITION;

            };

            sampler2D _Palette;
            float _GradientID;



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
                // sample the texture
                fixed4 col = tex2D(_Palette, i.uv);
                
                //lightning
                fixed lambert = dot(_WorldSpaceLightPos0,i.normal);
                fixed castShadow = SHADOW_ATTENUATION(i);

                //shadowOutline
                //float shadowOutline = computeOutline(i,lambert,castShadow);


                /*float3 boule = col.xyz ;
                //boule *= shadowOutline;
                float3 hsl = rgb2hsv(boule);
                hsl.y*=1.1;
                hsl.z*=1;
                boule = hsv2rgb(hsl);*/
                float shadow = saturate(/*sign*/(lambert) * castShadow )*.8+.1;
                shadow = shadow - shadow % (.2);

                float4 coloredShadow = (saturate(shadow)*.2+.8)* lerp(shadow, tex2D(_Palette,float2(shadow,_GradientID)),1);

                //col = lerp(float4(.1,.2,.3,0),col,shadow*.5+.5);
                
                
                col *= coloredShadow;//fixed4(i._ShadowCoord.zw,0,0);//fixed4(shadowDifX,shadowDifY,shadowDifZ,1)
                
                //col = lerp(col,float4(boule,0),saturate(shadowOutline));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }

        Pass
        {
            Tags { "RenderType"="Opaque"  "LightMode"="ForwardAdd"  }
            LOD 100
            Blend One One // Additive

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "utils\Nathan.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 normal : NORMAL;
                float4 pos : POSITION;
            };


            //vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _Palette);
                o.normal = UnityObjectToWorldNormal( v.normal);
                

                return o;
            }

            //fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = float4(1,1,1,0);
                
                return col;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        
    }
}
