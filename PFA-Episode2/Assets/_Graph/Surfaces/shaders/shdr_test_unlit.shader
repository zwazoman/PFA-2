Shader "Unlit/shdr_test_unlit"
{
    Properties
    {
        _Palette ("Texture", 2D) = "grey" {}
        _lightness ("Lighness", Float) = 1
        [KeywordEnum(MAP0,MAP1,MAP2)] _SECONDUV ("use second uv map", Float) = 0
    }
    SubShader
    {
        Pass
        {
            Name "MainPass"
            Tags { "RenderType"="Opaque"  "LightMode"="ForwardBase" "PassFlags" = "OnlyDirectional"}
            LOD 100
            Cull Off
            

            CGPROGRAM
            #pragma multi_compile FOG_ON
            
            #pragma vertex vert
            #pragma fragment frag

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

                float3 normal : NORMAL;
                float3 wsPos : TEXCOORD1;

                float4 pos : POSITION;

                unityShadowCoord4 _ShadowCoord : TEXCOORD3; // put shadows data into TEXCOORD1

            };

            //color
            sampler2D _Palette;
            float _lightness;
	        float _bands;

            //lightning
            sampler2D _lightGradientMap;
            float _enviroID;
            
            //stippling
            sampler2D _stippling;
            float _stipplingTiling;

            //fog
            float _fogRadius;
            float _fogIntensity;

            //vertex shader
            v2f vert (appdata v)
            {
                v2f o;

                o.wsPos =  mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _Palette);
                o.normal = UnityObjectToWorldNormal( v.normal);
                o.uv = v.uv;

                // compute shadows data
                TRANSFER_SHADOW(o);
                
                return o;
            }

            //fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(1,1,1,0);

                // sample the texture
                col = tex2D(_Palette, i.uv) ;
                float4 stippling = tex2D( _stippling, i.pos/_ScreenParams.x * _stipplingTiling);


                //lightning base
                fixed lambert = dot(_WorldSpaceLightPos0,i.normal)*.5+.5;
                fixed castShadow = SHADOW_ATTENUATION(i);

                //toon shadow
                float shadow = saturate(/*sign*/(lambert) * castShadow )*.8+.1;
                
                //fog
                #if FOG_ON
                    float camDist = max(0, distance(i.wsPos, _WorldSpaceCameraPos)-30);
                    float fog = max(0, (_fogRadius - camDist)/_fogRadius);
                    fog = 1-(1-fog) * (1-fog) ;
                #endif
                
#if FOG_ON
                    shadow *= fog;
#endif

                
                shadow = shadow - shadow % (1/_bands); //quatization


                float4 coloredShadow =  tex2D(_lightGradientMap,float2(shadow,_enviroID)); //gradient mapping

                //stippling
                coloredShadow = lerp(coloredShadow,coloredShadow*.5  ,(1 -stippling.x * (1-shadow))*.5); //tiling : 0.218 //petits triangles clairs
                coloredShadow = lerp(coloredShadow,coloredShadow*.8  ,stippling.y * (1-shadow)*(1-shadow)*(1-shadow)); //tiling : 0.218 //petits triangles clairs

                //light blending 
                //col = lerp(col,col * coloredShadow,.5);
                //col = (col > 0.5) * (1 - (1-2*(col-0.5)) * (1-coloredShadow)) + (col <= 0.5) * ((2*col) * coloredShadow); // overlay
                
                
                col =  lerp(col, (coloredShadow > 0.5) * (1 - (1-col) * (1-(coloredShadow-0.5))) + (coloredShadow <= 0.5) * (col * (coloredShadow+0.5)),1); //softlight
                
                col = lerp(col,coloredShadow,saturate((1-shadow)*(1-shadow)*(1-shadow))*.8);

                //color adjustment
                col *= _lightness;

                //vignette (mettre dans post process plutot si y en a)
                float vignette =  1-saturate((distance(float2(0.5,0.5),i.pos.xy/_ScreenParams.xy)-.4)*1);
                col *= vignette;
                
                return col;
            }
            ENDCG
        }


        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

        
    }
}
