Shader "Unlit/shdr_vfx_godray"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("Color",Color) = (1,1,1,1)
        _scrollingAndTiling("Scrolling,Tiling",Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
	    ZWrite Off
	    
	    Blend SrcAlpha OneMinusSrcAlpha
        
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

            Texture2D _MainTex;
            SamplerState my_linear_repeat_sampler;
            
            float4 _Color;
            float4 _scrollingAndTiling;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv = uv *  _scrollingAndTiling.zw + _Time[1] * _scrollingAndTiling.xy;

                float alpha = _MainTex.Sample(my_linear_repeat_sampler,uv);


                
                alpha *= i.uv.y*i.uv.y * _Color.w;
                alpha *= saturate((1.0-i.uv.y)*5);
                
                // sample the texture
                fixed4 col = fixed4( _Color.xyz*2,alpha*5);
                
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
