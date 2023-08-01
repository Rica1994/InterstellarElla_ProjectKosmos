// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterRock"
{
	Properties
	{
		[Header(Global)]_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Header(Rock)]_Rock("Rock", 2D) = "white" {}
		[Toggle(_OBJECTSPACE_ON)] _ObjectSpace("ObjectSpace", Float) = 1
		_RockColor0("RockColor0", Color) = (0,0,0,0)
		_RockColor1("RockColor1", Color) = (0,0,0,0)
		_TextureWorldScale("TextureWorldScale", Vector) = (1,1,0,0)
		[Header(Sand)]_SandMaskHeight("SandMaskHeight", Float) = 1
		[Toggle(_SANDMASKON_ON)] _SandMaskOn("SandMaskOn", Float) = 1
		_Sand("Sand", Color) = (0,0,0,0)
		_SandShade("SandShade", Color) = (0,0,0,0)
		_SunColorInfluence1("SunColorInfluence", Range( 0 , 1)) = 0.32
		_SandShadePower("SandShadePower", Float) = 0
		_SandShadeScale("SandShadeScale", Float) = 0
		_SandShadeBias("SandShadeBias", Float) = 0
		[Header(Variation)]_VariationIntensity("VariationIntensity", Range( 0 , 1)) = 0
		[Toggle(_COLORVARIATION_ON)] _ColorVariation("ColorVariation", Float) = 1
		[KeywordEnum(None,Dirt,EdgeHighlight,Both)] _VertexColorMasks("VertexColorMasks", Float) = 1
		_GrungeScale("GrungeScale", Float) = 0
		[Header(Vertex Color Masks)]_Grunge("Grunge", 2D) = "white" {}
		[Toggle(_USEUV_ON)] _UseUV("UseUV", Float) = 0
		_HighlightIntensity("HighlightIntensity", Range( 0 , 1)) = 0
		_AOIntensity("AOIntensity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _SANDMASKON_ON
		#pragma shader_feature_local _VERTEXCOLORMASKS_NONE _VERTEXCOLORMASKS_DIRT _VERTEXCOLORMASKS_EDGEHIGHLIGHT _VERTEXCOLORMASKS_BOTH
		#pragma shader_feature_local _COLORVARIATION_ON
		#pragma shader_feature_local _OBJECTSPACE_ON
		#pragma shader_feature_local _USEUV_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			half3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform half3 SunDirection;
		uniform half _SandMaskHeight;
		uniform sampler2D _Grunge;
		uniform half _VariationIntensity;
		uniform half3 SunColor;
		uniform half _SunColorInfluence1;
		uniform half4 _RockColor0;
		uniform half4 _RockColor1;
		uniform sampler2D _Rock;
		uniform half2 _TextureWorldScale;
		uniform half _GrungeScale;
		uniform half _HighlightIntensity;
		uniform half4 _Sand;
		uniform half4 _SandShade;
		uniform half _SandShadeBias;
		uniform half _SandShadeScale;
		uniform half _SandShadePower;
		uniform half _Smoothness;
		uniform half _AOIntensity;


		//https://www.shadertoy.com/view/XdXGW8
		float2 GradientNoiseDir( float2 x )
		{
			const float2 k = float2( 0.3183099, 0.3678794 );
			x = x * k + k.yx;
			return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
		}
		
		float GradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 i = floor( p );
			float2 f = frac( p );
			float2 u = f * f * ( 3.0 - 2.0 * f );
			return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
					lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
		}


		inline float4 TriplanarSampling45( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			half3 worldToObj43 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			#ifdef _OBJECTSPACE_ON
				half3 staticSwitch42 = worldToObj43;
			#else
				half3 staticSwitch42 = ase_worldPos;
			#endif
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			half lerpResult21 = lerp( tex2D( _Rock, ( (staticSwitch42).xy / _TextureWorldScale ) ).r , tex2D( _Rock, ( (staticSwitch42).zy / _TextureWorldScale ) ).r , ase_worldNormal.x);
			half3 lerpResult51 = lerp( (_RockColor0).rgb , (_RockColor1).rgb , lerpResult21);
			half4 transform101 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			half gradientNoise7 = GradientNoise(( (transform101).xz / 100.0 ),100.0);
			#ifdef _COLORVARIATION_ON
				half3 staticSwitch103 = ( lerpResult51 + ( gradientNoise7 * _VariationIntensity ) );
			#else
				half3 staticSwitch103 = lerpResult51;
			#endif
			half3 worldToObj166 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float4 triplanar45 = TriplanarSampling45( _Grunge, ( (worldToObj166).xyz / _GrungeScale ), ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			#ifdef _USEUV_ON
				half staticSwitch158 = tex2D( _Grunge, ( i.uv_texcoord / _GrungeScale ) ).r;
			#else
				half staticSwitch158 = triplanar45.x;
			#endif
			half temp_output_31_0 = ( 1.0 - i.vertexColor.g );
			half temp_output_50_0 = ( 1.0 - ( staticSwitch158 * temp_output_31_0 ) );
			half saferPower54 = abs( ( 1.0 - i.vertexColor.b ) );
			half3 temp_output_52_0 = ( staticSwitch103 + ( staticSwitch158 * pow( saferPower54 , 2.0 ) * _HighlightIntensity ) );
			#if defined(_VERTEXCOLORMASKS_NONE)
				half3 staticSwitch65 = staticSwitch103;
			#elif defined(_VERTEXCOLORMASKS_DIRT)
				half3 staticSwitch65 = ( staticSwitch103 * temp_output_50_0 );
			#elif defined(_VERTEXCOLORMASKS_EDGEHIGHLIGHT)
				half3 staticSwitch65 = temp_output_52_0;
			#elif defined(_VERTEXCOLORMASKS_BOTH)
				half3 staticSwitch65 = ( ( staticSwitch103 + temp_output_52_0 ) * temp_output_50_0 );
			#else
				half3 staticSwitch65 = ( staticSwitch103 * temp_output_50_0 );
			#endif
			half saferPower79 = abs( ase_worldNormal.y );
			half temp_output_38_0 = saturate( ( ( 1.0 - pow( saferPower79 , 2.0 ) ) * ( ase_worldPos.y / _SandMaskHeight ) ) );
			#ifdef _SANDMASKON_ON
				half3 staticSwitch73 = ( staticSwitch65 * temp_output_38_0 );
			#else
				half3 staticSwitch73 = staticSwitch65;
			#endif
			o.Albedo = staticSwitch73;
			half3 desaturateInitialColor7_g2 = SunColor;
			half desaturateDot7_g2 = dot( desaturateInitialColor7_g2, float3( 0.299, 0.587, 0.114 ));
			half3 desaturateVar7_g2 = lerp( desaturateInitialColor7_g2, desaturateDot7_g2.xxx, 1.0 );
			half3 normalizeResult127 = normalize( SunDirection );
			half fresnelNdotV128 = dot( ase_worldNormal, normalizeResult127 );
			half fresnelNode128 = ( _SandShadeBias + _SandShadeScale * pow( 1.0 - fresnelNdotV128, _SandShadePower ) );
			half3 lerpResult138 = lerp( (_Sand).rgb , (_SandShade).rgb , saturate( fresnelNode128 ));
			#ifdef _SANDMASKON_ON
				half3 staticSwitch41 = ( ( desaturateVar7_g2 * _SunColorInfluence1 ) * ( 1.0 - temp_output_38_0 ) * lerpResult138 );
			#else
				half3 staticSwitch41 = float3( 0,0,0 );
			#endif
			o.Emission = staticSwitch41;
			o.Smoothness = _Smoothness;
			o.Occlusion = ( 1.0 - saturate( ( temp_output_31_0 * _AOIntensity ) ) );
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
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				half4 color : COLOR0;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
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
Node;AmplifyShaderEditor.CommentaryNode;164;-1246.489,-178.9118;Inherit;False;1070.829;369.4333;UV version;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;163;-1460.402,-132.7734;Inherit;False;1453.932;658.4261;Comment;5;147;159;161;156;157;Grunge;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;154;963.123,-1624.416;Inherit;False;1856.965;760.2531;;9;126;127;128;129;130;131;132;137;138;Sand;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;153;1183.171,147.0582;Inherit;False;420.8657;185.0417;;2;50;48;Dirt;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;152;653.6863,-135.4626;Inherit;False;536.6156;343.3145;comment;3;53;54;64;Highlight;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;151;1.96402,139.1426;Inherit;False;243.3333;252;Height AO Edge;1;14;vert mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;147;-1410.402,181.5698;Inherit;False;1353.932;281.612;Triplanar Version;6;45;63;61;56;62;166;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;146;-1153.747,-488.1415;Inherit;False;1572.938;281.4622;Comment;8;3;101;11;7;4;12;105;104;Variance;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;145;-1760.304,-1473.505;Inherit;False;1871.143;921.6866;Comment;17;15;20;21;24;25;18;22;39;26;42;9;10;43;17;19;143;144;Rock;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;125;341.4611,305.619;Inherit;False;772.7253;320.4726;AO;3;30;28;29;AO;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;108;2759.244,75.16057;Inherit;False;650.2245;286.6192;SandLayer;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;107;1240.635,389.5427;Inherit;False;1271.02;329.6448;SandMask;8;38;77;34;36;75;78;79;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;2220.907,566.3166;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;34;1291.687,439.5427;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;1539.062,586.1877;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;75;1681.782,454.496;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;78;2035.481,499.5205;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;79;1871.978,500.2943;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;391.4612,511.0916;Inherit;False;Property;_AOIntensity;AOIntensity;23;0;Create;True;0;0;0;False;0;False;0;0.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;769.8888,368.0883;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;29;947.52,368.8112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;126;1013.123,-1283.673;Inherit;False;554.3773;419.5102;SandColors;4;136;135;134;133;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;38;2389.214,567.9104;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2980.36,-490.3256;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;100;2592.33,-578.2858;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;1245.63,-140.7366;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;140;1706.356,-156.6879;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;1924.671,-155.5907;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;41;3546.12,-451.5664;Inherit;False;Property;_HeightMaskOn;HeightMaskOn;7;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Reference;73;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;73;3542.455,-605.9673;Inherit;False;Property;_SandMaskOn;SandMaskOn;7;0;Create;True;0;0;0;True;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;15;-711.2805,-1203.605;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;c58e15a333b15c641868f6d8610d083f;c58e15a333b15c641868f6d8610d083f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-704.4282,-1003.443;Inherit;True;Property;_TextureSample1;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;c58e15a333b15c641868f6d8610d083f;c58e15a333b15c641868f6d8610d083f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;-323.4285,-1144.443;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;24;-1356.09,-1248.422;Inherit;False;Property;_TextureWorldScale;TextureWorldScale;5;0;Create;True;0;0;0;False;0;False;1,1;350.7,62.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldNormalVector;25;-609.7581,-789.32;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;18;-1031.069,-1182.526;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;-870.9148,-1156.513;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-868.3481,-965.6838;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;42;-1289.834,-1069.769;Inherit;False;Property;_ObjectSpace;ObjectSpace;2;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;9;-341.5219,-919.3012;Inherit;False;Property;_RockColor0;RockColor0;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4528302,0.2058319,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-328.8841,-758.818;Inherit;False;Property;_RockColor1;RockColor1;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2924528,0.1093156,0.01011626,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;43;-1495.706,-992.9756;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;17;-1710.304,-1061.246;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;19;-1036.333,-969.8432;Inherit;False;FLOAT2;2;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;143;-128.8516,-906.2729;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;144;-113.8279,-781.4599;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;3;-624.5831,-438.1415;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;101;-1103.747,-423.4401;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;7;-379.1253,-437.6703;Inherit;False;Gradient;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-870.6643,-349.2158;Inherit;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;12;-867.6039,-431.9203;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;255.8568,-436.8028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;51;245.5153,-981.9226;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;149;616.1636,152.0631;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;14;51.96402,189.1426;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;102;556.6486,-462.8217;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;54;724.4746,-18.27821;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;703.6863,92.85184;Inherit;False;Property;_HighlightIntensity;HighlightIntensity;22;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1630.592,-375.623;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;50;1424.036,197.0582;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;1233.171,199.0999;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;3709.691,-156.1245;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;127;1894.089,-1574.416;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;128;2137.525,-1462.52;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;129;2463.18,-1417.411;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;1564.261,-1420.726;Inherit;False;Property;_SandShadeBias;SandShadeBias;15;0;Create;True;0;0;0;False;0;False;0;-2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;131;1558.152,-1337.239;Inherit;False;Property;_SandShadeScale;SandShadeScale;14;0;Create;True;0;0;0;False;0;False;0;2.76;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;1643.813,-1264.98;Inherit;False;Property;_SandShadePower;SandShadePower;13;0;Create;True;0;0;0;False;0;False;0;1.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;133;1064.565,-1071.163;Inherit;False;Property;_SandShade;SandShade;9;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.7075472,0.3979487,0.238074,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;134;1063.122,-1233.673;Inherit;False;Property;_Sand;Sand;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.6531551,0.2893081,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;135;1342.193,-1091.945;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;136;1342.833,-1197.72;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;137;1611.158,-1573.269;Inherit;False;Global;SunDirection;SunDirection;28;0;Create;True;0;0;0;True;0;False;0,0,0;12.72169,146.6568,176.5209;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;138;2637.421,-1158.167;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;103;781.6638,-499.134;Inherit;False;Property;_ColorVariation;ColorVariation;17;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;65;2182.748,-494.7237;Inherit;False;Property;_VertexColorMasks;VertexColorMasks;18;0;Create;True;0;0;0;False;0;False;0;1;3;True;;KeywordEnum;4;None;Dirt;EdgeHighlight;Both;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;37;1290.635,604.1499;Inherit;False;Property;_SandMaskHeight;SandMaskHeight;6;1;[Header];Create;True;1;Sand;0;0;True;0;False;1;31.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;3577.309,-289.3418;Inherit;False;Property;_Smoothness;Smoothness;0;1;[Header];Create;True;1;Global;0;0;False;0;False;0;0.104;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;26;-1017.845,-1423.505;Inherit;True;Property;_Rock;Rock;1;1;[Header];Create;True;1;Rock;0;0;False;0;False;e955a59092a218e4e9861f0d837f8f8f;e955a59092a218e4e9861f0d837f8f8f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.OneMinusNode;31;718.2939,240.0302;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;49;385.6066,125.6157;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;1026.969,-85.46264;Inherit;False;3;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;158;55.09453,-96.09966;Inherit;False;Property;_UseUV;UseUV;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;45;-466.8011,231.57;Inherit;True;Spherical;World;False;Grunge Map;_GrungeMap;white;19;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;-722.8834,410.6524;Inherit;False;Property;_GrungeScale;GrungeScale;19;0;Create;True;0;0;0;False;0;False;0;59.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;61;-923.1342,368.8708;Inherit;False;FLOAT3;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;62;-591.007,269.8551;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;156;-962.1843,-82.77357;Inherit;True;Property;_Grunge;Grunge;20;1;[Header];Create;True;1;Vertex Color Masks;0;0;True;0;False;e955a59092a218e4e9861f0d837f8f8f;e955a59092a218e4e9861f0d837f8f8f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexCoordVertexDataNode;157;-1190.951,67.74112;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;159;-459.4146,-49.46236;Inherit;True;Property;_TextureSample2;Texture Sample 2;23;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;161;-593.9039,67.72868;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-673.8721,-317.6793;Inherit;False;Constant;_NoiseScale;NoiseScale;15;0;Create;True;0;0;0;True;0;False;100;9.49;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-174.3104,-324.8015;Inherit;False;Property;_VariationIntensity;VariationIntensity;16;1;[Header];Create;True;1;Variation;0;0;True;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;63;-1360.401,250.7913;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;166;-1168.758,257.1519;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;155;3076.986,137.8533;Inherit;True;SF_SandLayer;10;;2;d0d5cc302ad77b64c94b6d1ec53a441b;0;3;11;FLOAT;0;False;12;FLOAT3;0,0,0;False;10;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;167;4258.147,-502.1713;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterRock;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;78;0
WireConnection;77;1;36;0
WireConnection;36;0;34;2
WireConnection;36;1;37;0
WireConnection;78;0;79;0
WireConnection;79;0;75;2
WireConnection;28;0;31;0
WireConnection;28;1;30;0
WireConnection;29;0;28;0
WireConnection;38;0;77;0
WireConnection;72;0;65;0
WireConnection;72;1;38;0
WireConnection;100;0;65;0
WireConnection;52;0;103;0
WireConnection;52;1;53;0
WireConnection;140;0;103;0
WireConnection;140;1;52;0
WireConnection;141;0;140;0
WireConnection;141;1;50;0
WireConnection;41;0;155;0
WireConnection;73;1;100;0
WireConnection;73;0;72;0
WireConnection;15;0;26;0
WireConnection;15;1;22;0
WireConnection;20;0;26;0
WireConnection;20;1;39;0
WireConnection;21;0;15;1
WireConnection;21;1;20;1
WireConnection;21;2;25;1
WireConnection;18;0;42;0
WireConnection;22;0;18;0
WireConnection;22;1;24;0
WireConnection;39;0;19;0
WireConnection;39;1;24;0
WireConnection;42;1;17;0
WireConnection;42;0;43;0
WireConnection;43;0;17;0
WireConnection;19;0;42;0
WireConnection;143;0;9;0
WireConnection;144;0;10;0
WireConnection;3;0;12;0
WireConnection;3;1;4;0
WireConnection;7;0;3;0
WireConnection;7;1;11;0
WireConnection;12;0;101;0
WireConnection;104;0;7;0
WireConnection;104;1;105;0
WireConnection;51;0;143;0
WireConnection;51;1;144;0
WireConnection;51;2;21;0
WireConnection;149;0;158;0
WireConnection;102;0;51;0
WireConnection;102;1;104;0
WireConnection;54;0;49;0
WireConnection;47;0;103;0
WireConnection;47;1;50;0
WireConnection;50;0;48;0
WireConnection;48;0;149;0
WireConnection;48;1;31;0
WireConnection;32;0;29;0
WireConnection;127;0;137;0
WireConnection;128;4;127;0
WireConnection;128;1;130;0
WireConnection;128;2;131;0
WireConnection;128;3;132;0
WireConnection;129;0;128;0
WireConnection;135;0;133;0
WireConnection;136;0;134;0
WireConnection;138;0;136;0
WireConnection;138;1;135;0
WireConnection;138;2;129;0
WireConnection;103;1;51;0
WireConnection;103;0;102;0
WireConnection;65;1;103;0
WireConnection;65;0;47;0
WireConnection;65;2;52;0
WireConnection;65;3;141;0
WireConnection;31;0;14;2
WireConnection;49;0;14;3
WireConnection;53;0;158;0
WireConnection;53;1;54;0
WireConnection;53;2;64;0
WireConnection;158;1;45;1
WireConnection;158;0;159;1
WireConnection;45;0;156;0
WireConnection;45;9;62;0
WireConnection;61;0;166;0
WireConnection;62;0;61;0
WireConnection;62;1;56;0
WireConnection;159;0;156;0
WireConnection;159;1;161;0
WireConnection;161;0;157;0
WireConnection;161;1;56;0
WireConnection;166;0;63;0
WireConnection;155;12;138;0
WireConnection;155;10;38;0
WireConnection;167;0;73;0
WireConnection;167;2;41;0
WireConnection;167;4;66;0
WireConnection;167;5;32;0
ASEEND*/
//CHKSM=14D7DEC3B0B1592ED9E649CF33264B8A4D84A04D