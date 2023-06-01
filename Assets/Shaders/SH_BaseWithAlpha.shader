Shader "Custom/BaseWithAlpha"
{
    Properties
    {
        _BaseTex ("Base Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _DotColor ("Dot Color", Color) = (1, 1, 1, 1)
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
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
                float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _BaseTex;
            sampler2D _AlphaTex;
            fixed4 _DotColor;
            float _AlphaCutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv1 = v.uv1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Fetch base color from the base texture
                fixed4 baseColor = tex2D(_BaseTex, i.uv);

                // Fetch alpha value from the alpha texture using UV channel 1
                float alpha = tex2D(_AlphaTex, i.uv1).r;

                // Apply alpha cutout threshold
                alpha = step(_AlphaCutoff, alpha);

                // Calculate dot color based on alpha value
                fixed4 dotColor = _DotColor * alpha;

                // Mix base color and dot color
                fixed4 outputColor = lerp(baseColor, dotColor, dotColor.a);

                return outputColor;
            }
            ENDCG
        }
    }

    FallBack "Standard"
}
