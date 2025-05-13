Shader "Unlit/shdr_outline"
{
    Properties
    {
        _Thickness ("outline thickenn", float) = .1
         _Palette ("Texture", 2D) = "grey" {}
         _lightness ("Lighness", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        cull Front

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _Palette;
            float _lightness;
            float _Thickness;
            v2f vert (appdata v)
            { 
                v2f o;
                o.normal = UnityObjectToWorldNormal(v.normal); 
                o.vertex =  mul(UNITY_MATRIX_VP,(mul(unity_ObjectToWorld,  v.vertex)+_Thickness*float4(o.normal.xyz,0)) );
                o.uv = v.uv;

                return o;
            } 

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_Palette, i.uv) * _lightness;

                return col;
            }
            ENDCG
        }
    }
}
