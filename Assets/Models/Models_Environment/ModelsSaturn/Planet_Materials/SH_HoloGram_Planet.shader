// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_Hologram_Planet"
{
	Properties
	{
		_Fresnel_Bias_01("Fresnel_Bias_01", Float) = 0
		_Color("Color", Color) = (0,0.953454,1,0)
		_Fresnel_Bias_02("Fresnel_Bias_02", Float) = 0
		_Fresnel_Scale_01("Fresnel_Scale_01", Float) = 1
		_Fresnel_Scale_02("Fresnel_Scale_02", Float) = 1
		_Fresnel_power_01("Fresnel_power_01", Float) = 5
		_Fresnel_power_02("Fresnel_power_02", Float) = 5
		_Noise34("Noise34", 2D) = "white" {}
		_Float0("Float 0", Float) = 0.7
		_Lines_Opacity("Lines_Opacity", Float) = 0.3
		_Depth_Fade("Depth_Fade", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth_Fade;
		uniform float _Fresnel_Bias_01;
		uniform float _Fresnel_Scale_01;
		uniform float _Fresnel_power_01;
		uniform float _Fresnel_Bias_02;
		uniform float _Fresnel_Scale_02;
		uniform float _Fresnel_power_02;
		uniform float _Float0;
		uniform sampler2D _Noise34;
		uniform float _Lines_Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Emission = _Color.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth264 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth264 = abs( ( screenDepth264 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth_Fade ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV225 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode225 = ( _Fresnel_Bias_01 + _Fresnel_Scale_01 * pow( 1.0 - fresnelNdotV225, _Fresnel_power_01 ) );
			float fresnelNdotV232 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode232 = ( _Fresnel_Bias_02 + _Fresnel_Scale_02 * pow( 1.0 - fresnelNdotV232, _Fresnel_power_02 ) );
			float clampResult242 = clamp( ( fresnelNode232 * _Float0 ) , 0.0 , 1.0 );
			float mulTime245 = _Time.y * 0.18;
			float2 uv_TexCoord251 = i.uv_texcoord * float2( 1,1 );
			float2 panner243 = ( mulTime245 * float2( 0,-1 ) + uv_TexCoord251);
			float4 tex2DNode248 = tex2D( _Noise34, panner243 );
			o.Alpha = saturate( ( saturate( ( 1.0 - distanceDepth264 ) ) + ( ( fresnelNode225 + clampResult242 ) + ( tex2DNode248 * _Lines_Opacity ) ) ) ).r;
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
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
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
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;167;6761.225,-463.8236;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_Hologram_Planet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FresnelNode;225;5212.599,-480.8658;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;4885.599,-477.8658;Inherit;False;Property;_Fresnel_Bias_01;Fresnel_Bias_01;0;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;4895.599,-330.8658;Inherit;False;Property;_Fresnel_Scale_01;Fresnel_Scale_01;3;0;Create;True;0;0;0;False;0;False;1;581.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;4881.599,-193.8658;Inherit;False;Property;_Fresnel_power_01;Fresnel_power_01;5;0;Create;True;0;0;0;False;0;False;5;29.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;4746.599,73.4677;Inherit;False;Property;_Fresnel_Scale_02;Fresnel_Scale_02;4;0;Create;True;0;0;0;False;0;False;1;3.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;4732.599,210.4676;Inherit;False;Property;_Fresnel_power_02;Fresnel_power_02;6;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;232;5099.599,-76.86573;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;4736.599,-73.5323;Inherit;False;Property;_Fresnel_Bias_02;Fresnel_Bias_02;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;5379.599,-13.86573;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;240;5227.599,141.1342;Inherit;False;Property;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.7;0.69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;242;5554.599,-45.86573;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;246;6110.966,681.0652;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;247;5921.966,744.0652;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;250;5792.966,875.0652;Inherit;False;Constant;_Float3;Float 1;7;0;Create;True;0;0;0;False;0;False;0.79;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;252;6134.476,-173.0015;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;243;4840.966,632.0651;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;245;4663.966,846.0651;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;249;4417.966,851.0651;Inherit;False;Constant;_Float1;Float 0;7;0;Create;True;0;0;0;False;0;False;0.18;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;251;4574.966,507.0652;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;244;4490.548,672.4833;Inherit;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;0;False;0;False;0,-1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;258;5328.893,477.2095;Inherit;False;Constant;_Vector2;Vector 1;7;0;Create;True;0;0;0;False;0;False;0.1,0.1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;241;6427.134,-216.1933;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;259;4261.549,495.4295;Inherit;False;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;248;5269.966,656.0651;Inherit;True;Property;_Noise34;Noise34;7;0;Create;True;0;0;0;False;0;False;-1;3621a62e8758f344e9fddf5a650946ab;3621a62e8758f344e9fddf5a650946ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;231;5811.065,-282.2986;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;5841.052,336.4933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;262;5743.052,527.4933;Inherit;False;Property;_Lines_Opacity;Lines_Opacity;9;0;Create;True;0;0;0;False;0;False;0.3;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;264;5921.951,-456.8066;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;266;6200.848,-415.3566;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;268;5702.948,-415.3566;Inherit;False;Property;_Depth_Fade;Depth_Fade;10;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;224;6494.899,-720.8654;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;0,0.953454,1,0;0,0.7654505,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;267;6446.54,-325.6564;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;269;6390.649,-419.2567;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;167;2;224;0
WireConnection;167;9;241;0
WireConnection;225;1;226;0
WireConnection;225;2;229;0
WireConnection;225;3;230;0
WireConnection;232;1;236;0
WireConnection;232;2;237;0
WireConnection;232;3;238;0
WireConnection;239;0;232;0
WireConnection;239;1;240;0
WireConnection;242;0;239;0
WireConnection;246;0;248;0
WireConnection;246;1;247;0
WireConnection;246;2;250;0
WireConnection;252;0;231;0
WireConnection;252;1;260;0
WireConnection;243;0;251;0
WireConnection;243;2;244;0
WireConnection;243;1;245;0
WireConnection;245;0;249;0
WireConnection;251;0;259;0
WireConnection;241;0;267;0
WireConnection;248;1;243;0
WireConnection;231;0;225;0
WireConnection;231;1;242;0
WireConnection;260;0;248;0
WireConnection;260;1;262;0
WireConnection;264;0;268;0
WireConnection;266;0;264;0
WireConnection;267;0;269;0
WireConnection;267;1;252;0
WireConnection;269;0;266;0
ASEEND*/
//CHKSM=C85F3E602F70635ABE7470DEBA2CDCF15503CED4