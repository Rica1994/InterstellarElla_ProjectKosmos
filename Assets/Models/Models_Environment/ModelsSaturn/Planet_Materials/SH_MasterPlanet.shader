// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterPlanet"
{
	Properties
	{
		_T_Saturnus_color("T_Saturnus_color", 2D) = "white" {}
		_Noise63("Noise63", 2D) = "white" {}
		_Noise64("Noise63", 2D) = "white" {}
		_Color0("Color 0", Color) = (1,1,1,0)
		_Color1("Color 0", Color) = (0.3726415,0.6517627,1,0)
		_colorhue("color hue", Color) = (0.3726415,0.6517627,1,0)
		_tiling("tiling", Vector) = (4,8,0,0)
		_Float4("Float 4", Float) = 0
		_Float5("Float 5", Float) = 1.07
		_Float6("Float 6", Float) = 2.47
		_Cloudopacity("Cloud opacity", Float) = 1
		_SaturnusTurnspeed("Saturnus Turn speed", Vector) = (0.1,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
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

		uniform float4 _colorhue;
		uniform sampler2D _T_Saturnus_color;
		uniform float2 _SaturnusTurnspeed;
		uniform float2 _tiling;
		uniform float4 _Color1;
		uniform float _Float4;
		uniform float _Float5;
		uniform float _Float6;
		uniform float4 _Color0;
		uniform sampler2D _Noise63;
		uniform sampler2D _Noise64;
		uniform float _Cloudopacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord194 = i.uv_texcoord * _tiling;
			float2 panner222 = ( 1.0 * _Time.y * _SaturnusTurnspeed + uv_TexCoord194);
			float4 tex2DNode168 = tex2D( _T_Saturnus_color, panner222 );
			float4 lerpResult221 = lerp( _colorhue , tex2DNode168 , tex2DNode168);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV185 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode185 = ( _Float4 + _Float5 * pow( 1.0 - fresnelNdotV185, _Float6 ) );
			float4 lerpResult187 = lerp( lerpResult221 , _Color1 , fresnelNode185);
			float2 panner170 = ( 1.0 * _Time.y * float2( 0.1,0.05 ) + i.uv_texcoord);
			float2 uv_TexCoord196 = i.uv_texcoord * float2( 4,16 );
			float4 temp_cast_0 = (4.7).xxxx;
			float2 panner203 = ( 1.0 * _Time.y * float2( 0.15,-0.03 ) + i.uv_texcoord);
			float2 uv_TexCoord206 = i.uv_texcoord * float2( 1,4 );
			float4 temp_cast_1 = (10.2).xxxx;
			float4 lerpResult177 = lerp( lerpResult187 , _Color0 , ( ( saturate( ( pow( tex2D( _Noise63, ( panner170 + uv_TexCoord196 ) ) , temp_cast_0 ) * 3.0 ) ) * saturate( ( pow( tex2D( _Noise64, ( panner203 + uv_TexCoord206 ) ) , temp_cast_1 ) * 10.75 ) ) ) * _Cloudopacity ));
			o.Albedo = lerpResult177.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
Node;AmplifyShaderEditor.RangedFloatNode;173;2268.991,-321.1325;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;4640.486,376.0795;Inherit;False;Property;_Cloudopacity;Cloud opacity;12;0;Create;True;0;0;0;False;0;False;1;0.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;180;4206.282,170.9794;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;3934.284,234.9795;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;181;3726.285,346.9793;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;170;2605.07,126.9616;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;174;2161.07,312.962;Inherit;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;0;False;0;False;0.1,0.05;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;172;2222.07,-4.038598;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;196;2831.844,411.4147;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;169;3358.285,266.9796;Inherit;True;Property;_Noise63;Noise63;1;0;Create;True;0;0;0;False;0;False;-1;ff023cd47bb523a4cbc64a19883de5d9;ff023cd47bb523a4cbc64a19883de5d9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;182;3390.481,596.8244;Inherit;False;Constant;_Float2;Float 1;2;0;Create;True;0;0;0;False;0;False;4.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;179;3934.284,442.979;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;199;3110.499,234.7739;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;200;4294.615,831.5521;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;202;3814.618,1007.552;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;203;2693.403,787.5344;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;205;2310.403,656.5342;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;206;2920.177,1071.987;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;3022.943,808.4177;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;209;3446.618,927.5524;Inherit;True;Property;_Noise64;Noise63;2;0;Create;True;0;0;0;False;0;False;-1;ff023cd47bb523a4cbc64a19883de5d9;ff023cd47bb523a4cbc64a19883de5d9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;212;3198.832,895.3467;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;204;2249.403,973.5348;Inherit;False;Constant;_Vector3;Vector 0;2;0;Create;True;0;0;0;False;0;False;0.15,-0.03;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;208;2759.875,1083.053;Inherit;False;Constant;_Vector4;Vector 1;4;0;Create;True;0;0;0;False;0;False;1,4;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;4361.65,558.3955;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;4022.617,895.5522;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;210;3478.814,1257.397;Inherit;False;Constant;_Float3;Float 1;2;0;Create;True;0;0;0;False;0;False;10.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;211;4022.617,1103.552;Inherit;False;Constant;_Float7;Float 1;2;0;Create;True;0;0;0;False;0;False;10.75;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;187;4826.236,-476.7646;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;185;4382.459,-250.2906;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;193;4047.366,-372.3459;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;192;4197.46,-57.29066;Inherit;False;Property;_Float6;Float 6;11;0;Create;True;0;0;0;False;0;False;2.47;2.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;4166.46,-220.2906;Inherit;False;Property;_Float4;Float 4;9;0;Create;True;0;0;0;False;0;False;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;191;4162.46,-157.2906;Inherit;False;Property;_Float5;Float 5;10;0;Create;True;0;0;0;False;0;False;1.07;3.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;197;2671.542,422.48;Inherit;False;Constant;_Vector2;Vector 1;4;0;Create;True;0;0;0;False;0;False;4,16;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;214;4782.554,946.1674;Inherit;True;Property;_Noise96;Noise96;13;0;Create;True;0;0;0;False;0;False;-1;bca012a566034e047a45fbfeff3cb093;bca012a566034e047a45fbfeff3cb093;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;216;4809.315,530.351;Inherit;False;Property;_cloud1;cloud 1;6;0;Create;True;0;0;0;False;0;False;0.8039216,0.6392157,0.3568628,1;1,0.6631513,0.5330188,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;217;4811.242,748.1162;Inherit;False;Property;_cloud2;cloud 2;7;0;Create;True;0;0;0;False;0;False;0.2901961,0.1490196,0.09019608,1;1,0.6631513,0.5330188,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;4901.488,260.1794;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;167;6527.225,-455.8236;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterPlanet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LerpOp;177;5656.555,-422.5358;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;175;5252.476,-334.0173;Inherit;False;Property;_Color0;Color 0;3;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0.8972746,0.7688679,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;215;5472.182,295.3959;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;218;5489.84,168.9668;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;168;3286.991,-735.1325;Inherit;True;Property;_T_Saturnus_color;T_Saturnus_color;0;0;Create;True;0;0;0;False;0;False;-1;927af5e12706a0343917dce48dc26247;5360efb900143194684b041401647029;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;189;4337.894,-493.6943;Inherit;False;Property;_Color1;Color 0;4;0;Create;True;0;0;0;False;0;False;0.3726415,0.6517627,1,0;0.8679245,0.6048495,0.3725525,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;220;3334.147,-979.2654;Inherit;False;Property;_colorhue;color hue;5;0;Create;True;0;0;0;False;0;False;0.3726415,0.6517627,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;3778.083,-860.6573;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;221;4062.739,-803.0468;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;2967.126,-20.8349;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;194;2647.228,-861.8907;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;195;2388.43,-856.3425;Inherit;False;Property;_tiling;tiling;8;0;Create;True;0;0;0;False;0;False;4,8;4,10;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;222;2934.09,-586.1786;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;223;2625.184,-551.6296;Inherit;False;Property;_SaturnusTurnspeed;Saturnus Turn speed;14;0;Create;True;0;0;0;False;0;False;0.1,0;0.02,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
WireConnection;180;0;178;0
WireConnection;178;0;181;0
WireConnection;178;1;179;0
WireConnection;181;0;169;0
WireConnection;181;1;182;0
WireConnection;170;0;172;0
WireConnection;170;2;174;0
WireConnection;196;0;197;0
WireConnection;169;1;199;0
WireConnection;199;0;170;0
WireConnection;199;1;196;0
WireConnection;200;0;201;0
WireConnection;202;0;209;0
WireConnection;202;1;210;0
WireConnection;203;0;205;0
WireConnection;203;2;204;0
WireConnection;206;0;208;0
WireConnection;207;0;203;0
WireConnection;207;1;206;0
WireConnection;209;1;212;0
WireConnection;212;0;203;0
WireConnection;212;1;206;0
WireConnection;213;0;180;0
WireConnection;213;1;200;0
WireConnection;201;0;202;0
WireConnection;201;1;211;0
WireConnection;187;0;221;0
WireConnection;187;1;189;0
WireConnection;187;2;185;0
WireConnection;185;1;190;0
WireConnection;185;2;191;0
WireConnection;185;3;192;0
WireConnection;183;0;213;0
WireConnection;183;1;184;0
WireConnection;167;0;177;0
WireConnection;177;0;187;0
WireConnection;177;1;175;0
WireConnection;177;2;183;0
WireConnection;215;0;216;0
WireConnection;215;1;217;0
WireConnection;215;2;214;1
WireConnection;218;0;216;0
WireConnection;218;1;217;0
WireConnection;218;2;214;0
WireConnection;168;1;222;0
WireConnection;219;0;220;0
WireConnection;219;1;168;0
WireConnection;221;0;220;0
WireConnection;221;1;168;0
WireConnection;221;2;168;0
WireConnection;198;0;170;0
WireConnection;198;1;196;0
WireConnection;194;0;195;0
WireConnection;222;0;194;0
WireConnection;222;2;223;0
ASEEND*/
//CHKSM=30DE39AFF03272AE55702E9DB172D42A0BB96195