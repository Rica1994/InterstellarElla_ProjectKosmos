// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_RingDust_Remake"
{
	Properties
	{
		_Dust("Dust", 2D) = "white" {}
		_Lines("Lines", 2D) = "white" {}
		_LineIntensity("LineIntensity", Range( 0 , 2)) = 0.9128159
		_LinesColor("LinesColor", Color) = (1,0,0,0)
		_Spcks_Color("Spcks_Color", Color) = (0,0.1406798,1,0)
		_Tiling("Tiling", Vector) = (1,1,0,0)
		_Line_Power("Line_Power", Float) = 2.28
		_Line_Multiply("Line_Multiply", Float) = 2.28
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _LinesColor;
		uniform float4 _Spcks_Color;
		uniform sampler2D _Dust;
		uniform sampler2D _Lines;
		uniform float2 _Tiling;
		uniform float _LineIntensity;
		uniform float _Line_Power;
		uniform float _Line_Multiply;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 tex2DNode1 = tex2D( _Dust, i.uv_texcoord );
			float4 lerpResult79 = lerp( float4( (_LinesColor).rgb , 0.0 ) , _Spcks_Color , tex2DNode1.r);
			o.Albedo = lerpResult79.rgb;
			float temp_output_6_0 = ( (-1.0 + (i.uv_texcoord.x - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * 1.58 );
			float2 uv_TexCoord81 = i.uv_texcoord * _Tiling;
			float temp_output_4_0 = ( cos( temp_output_6_0 ) * ( tex2D( _Lines, uv_TexCoord81 ).r * _LineIntensity ) );
			float4 temp_cast_2 = (_Line_Power).xxxx;
			o.Alpha = ( pow( ( temp_output_4_0 + tex2DNode1 ) , temp_cast_2 ) * _Line_Multiply ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.CoshOpNode;9;-38.96677,-678.5079;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-886.4773,-307.5685;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-306.2037,-473.8079;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1.58;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;8;-27.29387,-472.1156;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;0.436846,-190.7225;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-359.1571,-25.07187;Inherit;False;Property;_LineIntensity;LineIntensity;2;0;Create;True;0;0;0;False;0;False;0.9128159;1.532;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectScaleNode;23;-1135.467,230.4726;Inherit;False;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-740.3268,94.15776;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-859.6135,317.2119;Inherit;False;Property;_DustScale;DustScale;3;0;Create;True;0;0;0;False;0;False;1;0.789;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;948.9274,-99.18567;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;25;-973.2708,225.2959;Inherit;False;True;False;True;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-308.1733,70.22987;Inherit;True;Property;_Dust;Dust;0;0;Create;True;0;0;0;False;0;False;-1;42249e2b82201a544b388419e993a1f0;42249e2b82201a544b388419e993a1f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-391.45,-215.6679;Inherit;True;Property;_Lines;Lines;1;0;Create;True;0;0;0;False;0;False;-1;a3c32e9f1528b924fb206a56dc29b973;a3c32e9f1528b924fb206a56dc29b973;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;10;-529.4487,-389.2166;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;20;-482.6347,158.5428;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;76;2431.617,-276.6376;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_RingDust_Remake;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;1410.621,516.3421;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;16;446.3689,-70.40325;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;1412.222,169.1411;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;435.5443,-328.3395;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;32;145.6782,-109.5854;Inherit;False;Property;_LinesColor;LinesColor;4;0;Create;True;0;0;0;False;0;False;1,0,0,0;0.2044024,0.8643638,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;79;681.1068,41.50268;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;80;71.50635,366.3026;Inherit;False;Property;_Spcks_Color;Spcks_Color;5;0;Create;True;0;0;0;False;0;False;0,0.1406798,1,0;0.2044024,0.8643638,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1059.545,83.80474;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-1032.969,-119.9537;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;82;-1393.241,-124.1591;Inherit;False;Property;_Tiling;Tiling;6;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;1342.487,-232.9942;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;83;1716.696,617.6018;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;2102.801,717.8732;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;84;1461.841,843.2112;Inherit;False;Property;_Line_Power;Line_Power;7;0;Create;True;0;0;0;False;0;False;2.28;2.28;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;1900.171,918.4143;Inherit;False;Property;_Line_Multiply;Line_Multiply;8;0;Create;True;0;0;0;False;0;False;2.28;2.28;0;0;0;1;FLOAT;0
WireConnection;9;0;6;0
WireConnection;6;0;10;0
WireConnection;8;0;6;0
WireConnection;11;0;2;1
WireConnection;11;1;12;0
WireConnection;26;0;22;0
WireConnection;26;1;25;0
WireConnection;70;0;79;0
WireConnection;70;1;1;1
WireConnection;25;0;23;0
WireConnection;1;1;22;0
WireConnection;2;1;81;0
WireConnection;10;0;3;1
WireConnection;20;0;26;0
WireConnection;20;1;21;0
WireConnection;76;0;79;0
WireConnection;76;9;85;0
WireConnection;77;0;4;0
WireConnection;77;1;1;0
WireConnection;16;0;32;0
WireConnection;4;0;8;0
WireConnection;4;1;11;0
WireConnection;79;0;16;0
WireConnection;79;1;80;0
WireConnection;79;2;1;1
WireConnection;81;0;82;0
WireConnection;35;0;4;0
WireConnection;35;1;79;0
WireConnection;83;0;77;0
WireConnection;83;1;84;0
WireConnection;85;0;83;0
WireConnection;85;1;86;0
ASEEND*/
//CHKSM=A327984FA2594F620400B14E40B94BAF141B7192