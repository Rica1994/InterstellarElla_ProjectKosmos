Shader "Custom/GlitchEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GlitchAmount ("Glitch Amount", Range(0, 0.50)) = 0.05
        _GlitchFrequency ("Glitch Frequency", Range(0, 100)) = 3.0
        _EmissiveColor ("Emissive Color", Color) = (1, 1, 1, 1)
        _EmissiveIntensity ("Emissive Intensity", Range(0, 100)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float _GlitchAmount;
            float _GlitchFrequency;
            float4 _EmissiveColor;
            float _EmissiveIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                float glitchOffset = _GlitchAmount * sin(_Time.y * _GlitchFrequency) * sin(v.uv.y * 50.0);
                v.vertex.x += glitchOffset;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv + float2(glitchOffset, 0.0); // also modify UVs for consistent glitch effect
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                // Calculate emissive contribution
                half4 emissive = _EmissiveColor * _EmissiveIntensity;
                col += emissive;

                return col;
            }
            ENDCG
        }
    }
}
