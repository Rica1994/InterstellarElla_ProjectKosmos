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
		_Noise13("Noise13", 2D) = "white" {}
		_Ring_Cloud_Power("Ring_Cloud_Power", Float) = 10
		_MinCloud("Min Cloud", Float) = 0
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
		};

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
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode230 = tex2D( _TextureSample0, uv_TextureSample0 );
			float2 uv_TexCoord250 = i.uv_texcoord * _Meteor_Tiling;
			float2 panner252 = ( 1.0 * _Time.y * _MeteoriteSpeed + uv_TexCoord250);
			float4 break246 = tex2D( _ColorNoise3, panner252 );
			float temp_output_244_0 = saturate( pow( ( break246.r + break246.g + break246.b ) , 20.0 ) );
			float4 lerpResult227 = lerp( tex2DNode230 , _Meteorites , temp_output_244_0);
			o.Albedo = lerpResult227.rgb;
			float2 uv_TexCoord269 = i.uv_texcoord * _Cloud_Tiling;
			float2 panner268 = ( 1.0 * _Time.y * _CloudSpeed + uv_TexCoord269);
			float4 temp_cast_1 = (_Ring_Cloud_Power).xxxx;
			float lerpResult272 = lerp( _Opacity , _MinCloud , ( _Opacity * saturate( ( pow( tex2D( _Noise13, panner268 ) , temp_cast_1 ) * _Rings_Cloud_Strength ) ) ).r);
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
Node;AmplifyShaderEditor.SamplerNode;223;3975.265,-121.3712;Inherit;True;Property;_T_Saturnus_Rings_Alpha;T_Saturnus_Rings_Alpha;0;0;Create;True;0;0;0;False;0;False;-1;2da157de37eec2b45a9a2a4ac6855d70;2da157de37eec2b45a9a2a4ac6855d70;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;229;4920.824,274.0036;Inherit;True;Property;_Noise111;Noise111;2;0;Create;True;0;0;0;False;0;False;-1;23dd6a3538a730149a4ed6954ddfa46d;23dd6a3538a730149a4ed6954ddfa46d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;228;5031.224,59.60361;Inherit;True;Property;_Noise53;Noise53;1;0;Create;True;0;0;0;False;0;False;-1;9eaeca867a34e7348bfb235ee6800c76;9eaeca867a34e7348bfb235ee6800c76;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;245;4842.423,50.91217;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;244;4585.267,295.8237;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;249;4438.6,-161.9804;Inherit;True;Property;_T_Dots_alpha;T_Dots_alpha;6;0;Create;True;0;0;0;False;0;False;-1;cf93c7c9932c23c4caaf0c76c0d0c8b9;cf93c7c9932c23c4caaf0c76c0d0c8b9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;241;4375.695,330.5568;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;246;4017.03,321.1842;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;247;4253.907,317.483;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;4260.4,456.4079;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;231;3715.133,315.2677;Inherit;True;Property;_ColorNoise3;ColorNoise3;5;0;Create;True;0;0;0;False;0;False;-1;64e6ce3f003f04d40975bcd409397c27;64e6ce3f003f04d40975bcd409397c27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;254;3143.98,833.522;Inherit;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;240;5658.225,-489.8236;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterRings;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;258;4881.046,-137.0542;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;252;3415.054,535.9617;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;250;3195.39,280.4648;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;251;2933.66,300.7178;Inherit;False;Property;_Meteor_Tiling;Meteor_Tiling;7;0;Create;True;0;0;0;False;0;False;1,1;23.41,1.79;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;253;3112.822,579.583;Inherit;False;Property;_MeteoriteSpeed;Meteorite Speed;9;0;Create;True;0;0;0;False;0;False;1,0;0.11,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;226;3703.51,-1298.869;Inherit;False;Property;_Meteorites;Meteorites;3;0;Create;True;0;0;0;False;0;False;1,0,0,0;0.1886792,0.1181915,0.03648984,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;4272.179,-1232.598;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;230;3797.66,-1558.069;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;2775cbd356bb68646b40a49bdc3b507a;2775cbd356bb68646b40a49bdc3b507a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;227;4529.182,-1505.468;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;3834.208,-568.8588;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;264;4144.231,-575.0897;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;262;3612.981,-423.9734;Inherit;False;Property;_Rings_Cloud_Strength;Rings_Cloud_Strength;12;0;Create;True;0;0;0;False;0;False;10;5.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;265;2962.38,-713.1586;Inherit;True;Property;_Noise13;Noise13;13;0;Create;True;0;0;0;False;0;False;-1;2320b94939e4b6541865eb6cbe65f192;2320b94939e4b6541865eb6cbe65f192;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;266;3534.616,-694.5798;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;267;3272.651,-501.3579;Inherit;False;Property;_Ring_Cloud_Power;Ring_Cloud_Power;14;0;Create;True;0;0;0;False;0;False;10;6.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;268;2646.542,-413.7299;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;269;2426.877,-669.2265;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;270;2167.005,-648.9736;Inherit;False;Property;_Cloud_Tiling;Cloud_Tiling;8;0;Create;True;0;0;0;False;0;False;1,1;0.56,-0.4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;271;2344.31,-370.1085;Inherit;False;Property;_CloudSpeed;Cloud Speed;10;0;Create;True;0;0;0;False;0;False;1,0;0.01,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;255;4359.651,-625.699;Inherit;False;Property;_Opacity;Opacity;11;0;Create;True;0;0;0;False;0;False;0.4;0.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;257;5178.607,-112.1277;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;272;4637.425,-588.6082;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;261;4445.152,-422.8394;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;273;4419.417,-498.5342;Inherit;False;Property;_MinCloud;Min Cloud;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
WireConnection;244;0;241;0
WireConnection;241;0;247;0
WireConnection;241;1;243;0
WireConnection;246;0;231;0
WireConnection;247;0;246;0
WireConnection;247;1;246;1
WireConnection;247;2;246;2
WireConnection;231;1;252;0
WireConnection;240;0;227;0
WireConnection;240;9;257;0
WireConnection;252;0;250;0
WireConnection;252;2;253;0
WireConnection;250;0;251;0
WireConnection;224;0;230;0
WireConnection;224;1;223;0
WireConnection;227;0;230;0
WireConnection;227;1;226;0
WireConnection;227;2;244;0
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
ASEEND*/
//CHKSM=DDD4D2D9A1B230723E6711FFA9ADBB6193957625