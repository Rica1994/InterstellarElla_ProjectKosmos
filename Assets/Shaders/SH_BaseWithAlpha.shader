Shader "Custom/DefaultStandard"
{
    Properties
    {
        _BaseTex ("Base", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _BaseTex;
        sampler2D _AlphaTex;
        sampler2D _MetallicGlossMap;
        half _Glossiness;
        half _Metallic;


        struct Input
        {
            float2 uv_BaseTex;
            float2 uv2_AlphaTex;
            float2 uv_MetallicGlossMap;
        };


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_BaseTex, IN.uv_BaseTex);
            fixed4 a = tex2D(_AlphaTex, IN.uv2_AlphaTex);
            
            o.Albedo = c.rgb + a.rgb;
            o.Metallic = tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap).r * _Metallic;
            o.Smoothness = tex2D(_MetallicGlossMap, IN.uv_MetallicGlossMap).a * _Glossiness;

        }
        ENDCG
    }

    FallBack "Diffuse"
}