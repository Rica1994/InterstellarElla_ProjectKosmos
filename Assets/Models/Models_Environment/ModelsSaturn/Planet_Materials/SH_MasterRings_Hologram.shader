// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterRings"
{
	Properties
	{
		_Meteorites("Meteorites", Color) = (1,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_ColorNoise3("ColorNoise3", 2D) = "white" {}
		_Meteor_Tiling("Meteor_Tiling", Vector) = (1,1,0,0)
		_Cloud_Tiling("Cloud_Tiling", Vector) = (1,1,0,0)
		_MeteoriteSpeed("Meteorite Speed", Vector) = (1,0,0,0)
		_CloudSpeed("Cloud Speed", Vector) = (1,0,0,0)
		_Opacity("Opacity", Float) = 0.4
		_Rings_Cloud_Strength("Rings_Cloud_Strength", Float) = 10
		_HologramPattern("Hologram Pattern", 2D) = "white" {}
		_Noise13("Noise13", 2D) = "white" {}
		_FresnelColor("Fresnel Color", Color) = (0,0.8832197,1,0)
		_FresnelScale("Fresnel Scale", Float) = 1
		_Ring_Cloud_Power("Ring_Cloud_Power", Float) = 10
		_MinCloud("Min Cloud", Float) = 0
		_FresnelPower("Fresnel Power", Float) = 5
		_HologramPatternScale("Hologram Pattern Scale", Float) = 1
		_HologramPatternPower("Hologram Pattern Power", Float) = 0.01
		_HologramPatternSpeed("Hologram Pattern Speed", Vector) = (0,0,0,0)
		_HologramPattern2Scale("Hologram Pattern 2 Scale", Float) = 1
		_HologramPattern2("Hologram Pattern 2", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _FresnelColor;
		uniform sampler2D _HologramPattern2;
		uniform float _HologramPattern2Scale;
		uniform sampler2D _HologramPattern;
		uniform float _HologramPatternScale;
		uniform float2 _HologramPatternSpeed;
		uniform float _HologramPatternPower;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _Meteorites;
		uniform sampler2D _ColorNoise3;
		uniform float2 _MeteoriteSpeed;
		uniform float2 _Meteor_Tiling;
		uniform float _Opacity;
		uniform float _MinCloud;
		uniform sampler2D _Noise13;
		uniform float2 _CloudSpeed;
		uniform float2 _Cloud_Tiling;
		uniform float _Ring_Cloud_Power;
		uniform float _Rings_Cloud_Strength;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_HologramPattern2Scale).xx;
			float2 uv_TexCoord276 = i.uv_texcoord * temp_cast_0;
			float2 temp_cast_1 = (_HologramPatternScale).xx;
			float2 panner279 = ( 1.0 * _Time.y * _HologramPatternSpeed + float2( 0,0 ));
			float2 uv_TexCoord278 = i.uv_texcoord * temp_cast_1 + panner279;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV287 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode287 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV287, _FresnelPower ) );
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float2 uv_TexCoord250 = i.uv_texcoord * _Meteor_Tiling;
			float2 panner252 = ( 1.0 * _Time.y * _MeteoriteSpeed + uv_TexCoord250);
			float4 break246 = tex2D( _ColorNoise3, panner252 );
			float temp_output_244_0 = saturate( pow( ( break246.r + break246.g + break246.b ) , 20.0 ) );
			float4 lerpResult227 = lerp( tex2D( _TextureSample0, uv_TextureSample0 ) , _Meteorites , temp_output_244_0);
			o.Albedo = ( ( _FresnelColor * ( ( ( tex2D( _HologramPattern2, uv_TexCoord276 ) * tex2D( _HologramPattern, uv_TexCoord278 ) ) * _HologramPatternPower ) + fresnelNode287 ) ) + lerpResult227 ).rgb;
			float2 uv_TexCoord269 = i.uv_texcoord * _Cloud_Tiling;
			float2 panner268 = ( 1.0 * _Time.y * _CloudSpeed + uv_TexCoord269);
			float4 temp_cast_3 = (_Ring_Cloud_Power).xxxx;
			float lerpResult272 = lerp( _Opacity , _MinCloud , ( _Opacity * saturate( ( pow( tex2D( _Noise13, panner268 ) , temp_cast_3 ) * _Rings_Cloud_Strength ) ) ).r);
			float lerpResult257 = lerp( lerpResult272 , 1.0 , temp_output_244_0);
			o.Alpha = lerpResult257;
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
				float3 worldNormal : TEXCOORD3;
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
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;240;5658.225,-489.8236;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterRings;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SaturateNode;244;3467.48,221.3045;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;241;3257.907,256.0376;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;246;2899.242,246.6649;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;247;3136.119,242.9638;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;3142.612,381.8886;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;231;2597.345,240.7485;Inherit;True;Property;_ColorNoise3;ColorNoise3;2;0;Create;True;0;0;0;False;0;False;-1;64e6ce3f003f04d40975bcd409397c27;64e6ce3f003f04d40975bcd409397c27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;258;3763.258,-211.5734;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;252;2297.266,461.4425;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;250;2077.603,205.9456;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;251;1815.872,226.1986;Inherit;False;Property;_Meteor_Tiling;Meteor_Tiling;3;0;Create;True;0;0;0;False;0;False;1,1;23.41,1.79;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;253;1995.035,505.0638;Inherit;False;Property;_MeteoriteSpeed;Meteorite Speed;5;0;Create;True;0;0;0;False;0;False;1,0;0.11,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;2716.42,-643.3779;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;264;3026.443,-649.6089;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;262;2495.193,-498.4926;Inherit;False;Property;_Rings_Cloud_Strength;Rings_Cloud_Strength;8;0;Create;True;0;0;0;False;0;False;10;5.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;265;1844.592,-787.6779;Inherit;True;Property;_Noise13;Noise13;10;0;Create;True;0;0;0;False;0;False;-1;2320b94939e4b6541865eb6cbe65f192;2320b94939e4b6541865eb6cbe65f192;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;266;2416.828,-769.099;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;267;2154.863,-575.8771;Inherit;False;Property;_Ring_Cloud_Power;Ring_Cloud_Power;13;0;Create;True;0;0;0;False;0;False;10;6.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;268;1528.754,-488.2491;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;269;1309.089,-743.7457;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;270;1049.217,-723.4928;Inherit;False;Property;_Cloud_Tiling;Cloud_Tiling;4;0;Create;True;0;0;0;False;0;False;1,1;0.56,-0.4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;271;1226.522,-444.6277;Inherit;False;Property;_CloudSpeed;Cloud Speed;6;0;Create;True;0;0;0;False;0;False;1,0;0.01,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;255;3241.864,-700.2181;Inherit;False;Property;_Opacity;Opacity;7;0;Create;True;0;0;0;False;0;False;0.4;0.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;257;4060.819,-186.6469;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;272;3519.637,-663.1274;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;3327.365,-497.3586;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;273;3301.63,-573.0533;Inherit;False;Property;_MinCloud;Min Cloud;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;226;2482.043,-1619.626;Inherit;False;Property;_Meteorites;Meteorites;0;0;Create;True;0;0;0;False;0;False;1,0,0,0;0.1886792,0.1181915,0.03648984,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;230;2576.193,-1878.826;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;2775cbd356bb68646b40a49bdc3b507a;2775cbd356bb68646b40a49bdc3b507a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;227;3307.715,-1826.225;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;274;1820.049,-2916.109;Inherit;False;2338.477;801.7822;Hologram Pattern;18;292;291;290;289;288;287;286;285;284;283;282;281;280;279;278;277;276;275;;0.0990566,0.7332056,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;275;2524.726,-2864.25;Inherit;True;Property;_HologramPattern2;Hologram Pattern 2;20;0;Create;True;0;0;0;False;0;False;-1;1fb236b5c107734489881f64fff0454b;1fb236b5c107734489881f64fff0454b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;276;2313.088,-2866.109;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;277;2073.842,-2864.802;Inherit;False;Property;_HologramPattern2Scale;Hologram Pattern 2 Scale;19;0;Create;True;0;0;0;False;0;False;1;29.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;278;2303.18,-2661.616;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;279;2107.05,-2555.665;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1.63;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;280;1870.049,-2552.665;Inherit;False;Property;_HologramPatternSpeed;Hologram Pattern Speed;18;0;Create;True;0;0;0;False;0;False;0,0;0,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;281;1870.355,-2663.084;Inherit;False;Property;_HologramPatternScale;Hologram Pattern Scale;16;0;Create;True;0;0;0;False;0;False;1;73.83;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;282;2529.51,-2660.754;Inherit;True;Property;_HologramPattern;Hologram Pattern;9;0;Create;True;0;0;0;False;0;False;-1;3621a62e8758f344e9fddf5a650946ab;3621a62e8758f344e9fddf5a650946ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;283;2923.148,-2724.696;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;284;2904.47,-2426.845;Inherit;False;Property;_HologramPatternPower;Hologram Pattern Power;17;0;Create;True;0;0;0;False;0;False;0.01;0.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;3242.557,-2559.041;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;286;3446.615,-2560.124;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;287;3152.092,-2321.327;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;2968.02,-2232.628;Inherit;False;Property;_FresnelPower;Fresnel Power;15;0;Create;True;0;0;0;False;0;False;5;3.76;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;289;2971.872,-2316.613;Inherit;False;Property;_FresnelScale;Fresnel Scale;12;0;Create;True;0;0;0;False;0;False;1;3.18;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;290;3433.738,-2762.135;Inherit;False;Property;_FresnelColor;Fresnel Color;11;0;Create;True;0;0;0;False;0;False;0,0.8832197,1,0;0,0.8832197,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;3684.415,-2618.867;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;292;3923.527,-2617.689;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;240;0;292;0
WireConnection;240;9;257;0
WireConnection;244;0;241;0
WireConnection;241;0;247;0
WireConnection;241;1;243;0
WireConnection;246;0;231;0
WireConnection;247;0;246;0
WireConnection;247;1;246;1
WireConnection;247;2;246;2
WireConnection;231;1;252;0
WireConnection;252;0;250;0
WireConnection;252;2;253;0
WireConnection;250;0;251;0
WireConnection;263;0;266;0
WireConnection;263;1;262;0
WireConnection;264;0;263;0
WireConnection;265;1;268;0
WireConnection;266;0;265;0
WireConnection;266;1;267;0
WireConnection;268;0;269;0
WireConnection;268;2;271;0
WireConnection;269;0;270;0
WireConnection;257;0;272;0
WireConnection;257;1;258;0
WireConnection;257;2;244;0
WireConnection;272;0;255;0
WireConnection;272;1;273;0
WireConnection;272;2;261;0
WireConnection;261;0;255;0
WireConnection;261;1;264;0
WireConnection;227;0;230;0
WireConnection;227;1;226;0
WireConnection;227;2;244;0
WireConnection;275;1;276;0
WireConnection;276;0;277;0
WireConnection;278;0;281;0
WireConnection;278;1;279;0
WireConnection;279;2;280;0
WireConnection;282;1;278;0
WireConnection;283;0;275;0
WireConnection;283;1;282;0
WireConnection;285;0;283;0
WireConnection;285;1;284;0
WireConnection;286;0;285;0
WireConnection;286;1;287;0
WireConnection;287;2;289;0
WireConnection;287;3;288;0
WireConnection;291;0;290;0
WireConnection;291;1;286;0
WireConnection;292;0;291;0
WireConnection;292;1;227;0
ASEEND*/
//CHKSM=649853475A34654DB50DF0A2758F36B72108409C