Shader "Unlit/shdr_unlit"
{
    Properties
    {
        _color ("Color", Color) = (1,1,1,1) 
    }
    SubShader
    {
        Pass
        {
            Name "MainPass"
            Tags { "RenderType"="Opaque"}
            LOD 100
            Cull Off
            

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "utils\Nathan.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
            };
 
            struct v2f
            {
                float4 pos : POSITION;
                half4 color : COLOR;
            };

            //color
            half4 _color;
            
            //stippling
            sampler2D _stippling;
            float _stipplingTiling;

            //vertex shader
            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;

                return o;
            }

            // Converts the rgb value to hsv, where H's range is -1 to 5
float3 rgb_to_hsv(float3 RGB)
{
    float r = RGB.x;
    float g = RGB.y;
    float b = RGB.z;

    float minChannel = min(r, min(g, b));
    float maxChannel = max(r, max(g, b));

    float h = 0;
    float s = 0;
    float v = maxChannel;

    float delta = maxChannel - minChannel;

    if (delta != 0)
    {
        s = delta / v;

        if (r == v) h = (g - b) / delta;
        else if (g == v) h = 2 + (b - r) / delta;
        else if (b == v) h = 4 + (r - g) / delta;
    }

    return float3(h, s, v);
}

float3 hsv_to_rgb(float3 HSV)
{
    float3 RGB = HSV.z;

    float h = HSV.x;
    float s = HSV.y;
    float v = HSV.z;

    float i = floor(h);
    float f = h - i;

    float p = (1.0 - s);
    float q = (1.0 - s * f);
    float t = (1.0 - s * (1 - f));

    if (i == 0) { RGB = float3(1, t, p); }
    else if (i == 1) { RGB = float3(q, 1, p); }
    else if (i == 2) { RGB = float3(p, 1, t); }
    else if (i == 3) { RGB = float3(p, q, 1); }
    else if (i == 4) { RGB = float3(t, p, 1); }
    else /* i == -1 */ { RGB = float3(1, p, q); }

    RGB *= v;

    return RGB;
}


            //fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = half4(1,1,1,0);

                // sample the texture
                float3 hsl = rgb_to_hsv( _color.xyz);
                hsl += float3(1-i.color.x,0,0);
                col = half4( hsv_to_rgb(hsl),1);

                float4 stippling = tex2D( _stippling, i.pos/_ScreenParams.x * _stipplingTiling);
                col += stippling.x*.05;
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
