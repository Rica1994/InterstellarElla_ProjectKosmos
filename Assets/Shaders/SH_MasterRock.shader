// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterRock"
{
	Properties
	{
		_TopTexture0("Top Texture 0", 2D) = "white" {}
		_Color0("Color 0", Color) = (0,0,0,0)
		_Color1("Color 1", Color) = (0,0,0,0)
		_TextureWorldScale("TextureWorldScale", Vector) = (1,1,0,0)
		_Texture0("Texture 0", 2D) = "white" {}
		_AOIntensity("AOIntensity", Float) = 0
		_HeightColor("HeightColor", Color) = (0,0,0,0)
		_HeightMaskDistance("HeightMaskDistance", Float) = 1
		[Toggle(_HEIGHTMASKON1_ON)] _HeightMaskOn1("HeightMaskOn", Float) = 1
		[Toggle(_HASVERTEXMASKS_ON)] _HasVertexMasks("HasVertexMasks", Float) = 1
		[Toggle(_OBJECTSPACE_ON)] _ObjectSpace("ObjectSpace", Float) = 1
		_GrungeScale("GrungeScale", Float) = 0
		_HighlightIntensity("HighlightIntensity", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_SunColorInfluence("SunColorInfluence", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 2.0
		#pragma shader_feature_local _HEIGHTMASKON1_ON
		#pragma shader_feature_local _HASVERTEXMASKS_ON
		#pragma shader_feature_local _OBJECTSPACE_ON
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
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform float4 _HeightColor;
		uniform float4 _Color0;
		uniform float4 _Color1;
		uniform sampler2D _Texture0;
		uniform float2 _TextureWorldScale;
		sampler2D _TopTexture0;
		uniform float _GrungeScale;
		uniform float _HighlightIntensity;
		uniform float _HeightMaskDistance;
		uniform float3 SunColor;
		uniform float _SunColorInfluence;
		uniform float _Smoothness;
		uniform float _AOIntensity;


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
			float3 worldToObj43 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			#ifdef _OBJECTSPACE_ON
				float3 staticSwitch42 = worldToObj43;
			#else
				float3 staticSwitch42 = ase_worldPos;
			#endif
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float lerpResult21 = lerp( tex2D( _Texture0, ( (staticSwitch42).xy / _TextureWorldScale ) ).r , tex2D( _Texture0, ( (staticSwitch42).zy / _TextureWorldScale ) ).r , ase_worldNormal.x);
			float4 lerpResult51 = lerp( _Color0 , _Color1 , lerpResult21);
			float4 transform60 = mul(unity_WorldToObject,float4( ase_worldPos , 0.0 ));
			float4 triplanar45 = TriplanarSampling45( _TopTexture0, ( (transform60).xyz / _GrungeScale ), ase_worldNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			float saferPower54 = abs( i.vertexColor.b );
			#ifdef _HASVERTEXMASKS_ON
				float4 staticSwitch65 = ( ( lerpResult51 * ( 1.0 - ( triplanar45.x * ( 1.0 - i.vertexColor.g ) ) ) ) + ( triplanar45.r * pow( saferPower54 , 2.0 ) * _HighlightIntensity ) );
			#else
				float4 staticSwitch65 = lerpResult51;
			#endif
			float saferPower79 = abs( ase_worldNormal.y );
			float temp_output_38_0 = saturate( ( ( 1.0 - pow( saferPower79 , 10.0 ) ) * ( ase_worldPos.y / _HeightMaskDistance ) ) );
			float4 lerpResult33 = lerp( _HeightColor , staticSwitch65 , temp_output_38_0);
			#ifdef _HEIGHTMASKON1_ON
				float4 staticSwitch73 = ( staticSwitch65 * temp_output_38_0 );
			#else
				float4 staticSwitch73 = lerpResult33;
			#endif
			o.Albedo = staticSwitch73.rgb;
			float3 desaturateInitialColor71 = SunColor;
			float desaturateDot71 = dot( desaturateInitialColor71, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar71 = lerp( desaturateInitialColor71, desaturateDot71.xxx, 1.0 );
			#ifdef _HEIGHTMASKON1_ON
				float4 staticSwitch41 = ( float4( ( desaturateVar71 * _SunColorInfluence ) , 0.0 ) * lerpResult33 );
			#else
				float4 staticSwitch41 = staticSwitch65;
			#endif
			o.Emission = staticSwitch41.rgb;
			o.Smoothness = _Smoothness;
			o.Occlusion = ( 1.0 - saturate( ( ( 1.0 - i.vertexColor.g ) * _AOIntensity ) ) );
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
			#pragma target 2.0
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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
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
Node;AmplifyShaderEditor.NoiseGeneratorNode;7;-16.13641,-4.654737;Inherit;False;Gradient;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-281.5478,166.4868;Inherit;False;Property;_NoiseScale;NoiseScale;3;0;Create;True;0;0;0;False;0;False;0;8.06;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;15;-610.4049,-836.7469;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;c58e15a333b15c641868f6d8610d083f;c58e15a333b15c641868f6d8610d083f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-603.5526,-636.5854;Inherit;True;Property;_TextureSample1;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;c58e15a333b15c641868f6d8610d083f;c58e15a333b15c641868f6d8610d083f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;-222.5526,-777.5854;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;24;-1255.214,-881.5645;Inherit;False;Property;_TextureWorldScale;TextureWorldScale;4;0;Create;True;0;0;0;False;0;False;1,1;6.76,13.86;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;30;79.94397,553.1959;Inherit;False;Property;_AOIntensity;AOIntensity;6;0;Create;True;0;0;0;False;0;False;0;1.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;458.3715,410.1927;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;31;300.9556,397.7234;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;197.0455,-366.5313;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;34;77.20947,-898.7714;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;35;59.86292,-1053.501;Inherit;False;Property;_HeightColor;HeightColor;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8980392,0.4901961,0.05490196,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;36;324.5849,-752.1266;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;25;-508.8825,-422.4622;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;18;-930.1923,-815.668;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;-770.0392,-789.6548;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;19;-935.457,-602.9855;Inherit;False;FLOAT2;2;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-767.4725,-598.8261;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;37;76.15769,-734.1645;Inherit;False;Property;_HeightMaskDistance;HeightMaskDistance;8;0;Create;True;0;0;0;False;0;False;1;4.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;852.9382,391.395;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;29;636.0026,410.9156;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;17;-1569.529,-784.6849;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TexturePropertyNode;26;-916.9692,-1056.647;Inherit;True;Property;_Texture0;Texture 0;5;0;Create;True;0;0;0;False;0;False;None;380818323d81c45409b9bfd5a45e1d4f;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TransformPositionNode;43;-1411.629,-456.0239;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;42;-1188.958,-702.9111;Inherit;False;Property;_ObjectSpace;ObjectSpace;11;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleRemainderNode;5;-243.0334,15.16025;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;10;-244.8598,-311.9153;Inherit;False;Property;_Color1;Color 1;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.5,0.2448698,0.1116352,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;-251.1781,-485.0373;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1320755,0.06323656,0.02948854,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;424.8674,-463.9261;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;51;238.269,-550.5657;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;49;539.4814,227.1218;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-702.1882,-270.7118;Inherit;False;Property;_GrungeScale;GrungeScale;12;0;Create;True;0;0;0;False;0;False;0;59.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1178.841,-402.5768;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;1;-1512.951,38.29028;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldToObjectTransfNode;60;-953.7543,-140.3521;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;63;-1140.37,-126.4111;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;6;-669.7887,112.8799;Inherit;False;Constant;_Float1;Float 0;0;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;3;-487.6667,2.166702;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;10;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-695.92,32.24852;Inherit;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;0;False;0;False;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;12;-697.0623,-50.45597;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;61;-681.169,-113.6081;Inherit;False;FLOAT3;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;62;-246.5996,-103.7301;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TriplanarNode;45;260.2693,-125.5695;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;844.194,19.84075;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;50;1001.674,15.87058;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;14;55.96402,167.1426;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;52;1426.321,-400.4734;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;54;387.7563,129.8587;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;678.1155,-273.2088;Inherit;False;Property;_HighlightIntensity;HighlightIntensity;13;0;Create;True;0;0;0;False;0;False;0;0.625;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;1001.805,-140.0956;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;65;1607.08,-538.1284;Inherit;False;Property;_HasVertexMasks;HasVertexMasks;10;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;1942.489,-358.7971;Inherit;False;Property;_Smoothness;Smoothness;14;0;Create;True;0;0;0;False;0;False;0;0.104;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;2159.95,-888.6023;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;2017.386,-967.4671;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;69;1745.086,-869.8683;Inherit;False;Property;_SunColorInfluence;SunColorInfluence;15;0;Create;True;0;0;0;False;0;False;0;0.346;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;70;1516.773,-1135.911;Inherit;False;Global;SunColor;SunColor;28;0;Create;True;0;0;0;True;0;False;0,0,0;2.3,1.507363,1.186163;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DesaturateOpNode;71;1741.687,-1127.36;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;2080.007,-635.1364;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;38;1172.349,-714.6348;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;33;1940.316,-767.8974;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;41;2707.774,-598.7592;Inherit;False;Property;_HeightMaskOn;HeightMaskOn;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Reference;73;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;75;467.3046,-883.8181;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;78;808.004,-842.7937;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;79;671.5,-904.0199;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;1048.268,-726.4346;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;73;2705.195,-874.3543;Inherit;False;Property;_HeightMaskOn1;HeightMaskOn;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3118.667,-665.9858;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;SH_MasterRock;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.NormalVertexDataNode;81;2130.12,-1211.872;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;83;2214.2,-1043.103;Inherit;False;Constant;_Vector0;Vector 0;16;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;80;3095.187,-1129.929;Inherit;False;Property;_HeightMaskOn2;HeightMaskOn;9;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Reference;73;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;85;3368.137,-1080.946;Inherit;False;World;Tangent;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;87;2137.978,-1380.254;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ConditionalIfNode;86;2893.979,-1083.002;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;82;2445.756,-1375.558;Inherit;False;Tangent;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
WireConnection;7;0;3;0
WireConnection;7;1;11;0
WireConnection;15;0;26;0
WireConnection;15;1;22;0
WireConnection;20;0;26;0
WireConnection;20;1;39;0
WireConnection;21;0;15;1
WireConnection;21;1;20;1
WireConnection;21;2;25;1
WireConnection;28;0;31;0
WireConnection;28;1;30;0
WireConnection;31;0;14;2
WireConnection;8;0;9;0
WireConnection;8;1;10;0
WireConnection;8;2;7;0
WireConnection;36;0;34;2
WireConnection;36;1;37;0
WireConnection;18;0;42;0
WireConnection;22;0;18;0
WireConnection;22;1;24;0
WireConnection;19;0;42;0
WireConnection;39;0;19;0
WireConnection;39;1;24;0
WireConnection;32;0;29;0
WireConnection;29;0;28;0
WireConnection;43;0;17;0
WireConnection;42;1;17;0
WireConnection;42;0;43;0
WireConnection;5;0;3;0
WireConnection;5;1;6;0
WireConnection;16;0;21;0
WireConnection;16;1;8;0
WireConnection;51;0;9;0
WireConnection;51;1;10;0
WireConnection;51;2;21;0
WireConnection;49;0;14;2
WireConnection;47;0;51;0
WireConnection;47;1;50;0
WireConnection;60;0;63;0
WireConnection;3;0;12;0
WireConnection;3;1;4;0
WireConnection;12;0;60;0
WireConnection;61;0;60;0
WireConnection;62;0;61;0
WireConnection;62;1;56;0
WireConnection;45;9;62;0
WireConnection;48;0;45;1
WireConnection;48;1;49;0
WireConnection;50;0;48;0
WireConnection;52;0;47;0
WireConnection;52;1;53;0
WireConnection;54;0;14;3
WireConnection;53;0;45;1
WireConnection;53;1;54;0
WireConnection;53;2;64;0
WireConnection;65;1;51;0
WireConnection;65;0;52;0
WireConnection;67;0;68;0
WireConnection;67;1;33;0
WireConnection;68;0;71;0
WireConnection;68;1;69;0
WireConnection;71;0;70;0
WireConnection;72;0;65;0
WireConnection;72;1;38;0
WireConnection;38;0;77;0
WireConnection;33;0;35;0
WireConnection;33;1;65;0
WireConnection;33;2;38;0
WireConnection;41;1;65;0
WireConnection;41;0;67;0
WireConnection;78;0;79;0
WireConnection;79;0;75;2
WireConnection;77;0;78;0
WireConnection;77;1;36;0
WireConnection;73;1;33;0
WireConnection;73;0;72;0
WireConnection;0;0;73;0
WireConnection;0;2;41;0
WireConnection;0;4;66;0
WireConnection;0;5;32;0
WireConnection;80;1;81;0
WireConnection;80;0;86;0
WireConnection;85;0;80;0
WireConnection;86;0;38;0
WireConnection;86;2;81;0
WireConnection;86;4;83;0
WireConnection;82;0;87;0
ASEEND*/
//CHKSM=272D2AD7EA8C886614934FB6598957D4C18617E0