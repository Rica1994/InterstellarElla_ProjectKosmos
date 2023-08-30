Shader "Custom/UIGlitchEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _GlitchAmount ("Glitch Amount", Range(0, 50)) = 0.05
        _GlitchFrequency ("Glitch Frequency", Range(0, 100)) = 3.0
        _EmissiveColor ("Emissive Color", Color) = (1, 1, 1, 1)
        _EmissiveIntensity ("Emissive Intensity", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
            float4 _Color;
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
                o.uv = v.uv + float2(glitchOffset, 0.0); 
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texCol = tex2D(_MainTex, i.uv);

                // Convert RGB to grayscale and use as alpha
                float grayscaleAlpha = dot(texCol.rgb, half3(0.299, 0.587, 0.114));
                half4 col = _Color * half4(texCol.rgb, grayscaleAlpha);

                // Calculate emissive contribution
                half4 emissive = _EmissiveColor * _EmissiveIntensity;
                col.rgb += emissive.rgb;

                return col;
            }
            ENDCG
        }
    }
}
