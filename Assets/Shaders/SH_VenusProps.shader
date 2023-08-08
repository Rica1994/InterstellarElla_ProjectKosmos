// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VenusProps"
{
	Properties
	{
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_NoiseScale("NoiseScale", Float) = 0
		_DetailScale("DetailScale", Float) = 0
		_Sand_Mask_SCale("Sand_Mask_SCale", Float) = 0
		_DisplaceIntensity("DisplaceIntensity", Float) = 0
		_Rock("Rock", Color) = (0,0,0,0)
		_Sand("Sand", Color) = (0,0,0,0)
		_HeightMask("HeightMask", Float) = 0
		_Sand_Mask("Sand_Mask", 2D) = "white" {}
		_RockErosion("RockErosion", 2D) = "white" {}
		_Detail("Detail", 2D) = "white" {}
		_MaxClamp("MaxClamp", Float) = 0
		_NormalStrenght("NormalStrenght", Float) = 0
		_Sand_Mask_Srength("Sand_Mask_Srength", Float) = 0
		_SandTextureInfluence("SandTextureInfluence", Float) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0
		_MaskIntensity("MaskIntensity", Range( 0 , 100)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 2.0
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float3 SunDirection;
		uniform float3 SunColor;
		uniform sampler2D _Mask2;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask1;
		uniform sampler2D _Mask3;
		uniform float4 _MaskMapRemapScale0;
		uniform float4 _MaskMapRemapOffset2;
		uniform float4 _MaskMapRemapScale2;
		uniform float4 _MaskMapRemapScale1;
		uniform float4 _MaskMapRemapOffset1;
		uniform float4 _MaskMapRemapScale3;
		uniform float4 _MaskMapRemapOffset3;
		uniform float4 _MaskMapRemapOffset0;
		#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
			sampler2D _TerrainHeightmapTexture;//ASE Terrain Instancing
			sampler2D _TerrainNormalmapTexture;//ASE Terrain Instancing
		#endif//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_START( Terrain )//ASE Terrain Instancing
			UNITY_DEFINE_INSTANCED_PROP( float4, _TerrainPatchInstanceData )//ASE Terrain Instancing
		UNITY_INSTANCING_BUFFER_END( Terrain)//ASE Terrain Instancing
		CBUFFER_START( UnityTerrain)//ASE Terrain Instancing
			#ifdef UNITY_INSTANCING_ENABLED//ASE Terrain Instancing
				float4 _TerrainHeightmapRecipSize;//ASE Terrain Instancing
				float4 _TerrainHeightmapScale;//ASE Terrain Instancing
			#endif//ASE Terrain Instancing
		CBUFFER_END//ASE Terrain Instancing
		uniform sampler2D _RockErosion;
		uniform float _DisplaceIntensity;
		uniform float _NoiseScale;
		uniform float _HeightMask;
		uniform float _MaskIntensity;
		uniform float _NormalStrenght;
		uniform float4 _Sand;
		uniform sampler2D _Detail;
		uniform float _DetailScale;
		uniform float _SandTextureInfluence;
		uniform float4 _Rock;
		uniform float _MaxClamp;
		uniform sampler2D _Sand_Mask;
		uniform float _Sand_Mask_SCale;
		uniform float _Sand_Mask_Srength;
		uniform float _Roughness;


		void ApplyMeshModification( inout appdata_full v )
		{
			#if defined(UNITY_INSTANCING_ENABLED) && !defined(SHADER_API_D3D11_9X)
				float2 patchVertex = v.vertex.xy;
				float4 instanceData = UNITY_ACCESS_INSTANCED_PROP(Terrain, _TerrainPatchInstanceData);
				
				float4 uvscale = instanceData.z * _TerrainHeightmapRecipSize;
				float4 uvoffset = instanceData.xyxy * uvscale;
				uvoffset.xy += 0.5f * _TerrainHeightmapRecipSize.xy;
				float2 sampleCoords = (patchVertex.xy * uvscale.xy + uvoffset.xy);
				
				float hm = UnpackHeightmap(tex2Dlod(_TerrainHeightmapTexture, float4(sampleCoords, 0, 0)));
				v.vertex.xz = (patchVertex.xy + instanceData.xy) * _TerrainHeightmapScale.xz * instanceData.z;
				v.vertex.y = hm * _TerrainHeightmapScale.y;
				v.vertex.w = 1.0f;
				
				v.texcoord.xy = (patchVertex.xy * uvscale.zw + uvoffset.zw);
				v.texcoord3 = v.texcoord2 = v.texcoord1 = v.texcoord;
				
				#ifdef TERRAIN_INSTANCED_PERPIXEL_NORMAL
					v.normal = float3(0, 1, 0);
					//data.tc.zw = sampleCoords;
				#else
					float3 nor = tex2Dlod(_TerrainNormalmapTexture, float4(sampleCoords, 0, 0)).xyz;
					v.normal = 2.0f * nor - 1.0f;
				#endif
			#endif
		}


		float3 PerturbNormal107_g1( float3 surf_pos, float3 surf_norm, float height, float scale )
		{
			// "Bump Mapping Unparametrized Surfaces on the GPU" by Morten S. Mikkelsen
			float3 vSigmaS = ddx( surf_pos );
			float3 vSigmaT = ddy( surf_pos );
			float3 vN = surf_norm;
			float3 vR1 = cross( vSigmaT , vN );
			float3 vR2 = cross( vN , vSigmaS );
			float fDet = dot( vSigmaS , vR1 );
			float dBs = ddx( height );
			float dBt = ddy( height );
			float3 vSurfGrad = scale * 0.05 * sign( fDet ) * ( dBs * vR1 + dBt * vR2 );
			return normalize ( abs( fDet ) * vN - vSurfGrad );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			ApplyMeshModification(v);;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g1 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g1 = ase_worldNormal;
			float2 temp_output_5_0 = (ase_worldNormal).xz;
			float2 temp_output_9_0 = ( temp_output_5_0 * _DisplaceIntensity );
			float4 tex2DNode21 = tex2D( _RockErosion, ( ( (ase_worldPos).xz + temp_output_9_0 ) / _NoiseScale ) );
			float temp_output_15_0 = ( 1.0 - ase_worldNormal.y );
			float temp_output_18_0 = ( ase_worldPos.y / _HeightMask );
			float temp_output_32_0 = ( tex2DNode21.r * temp_output_15_0 * temp_output_18_0 * _MaskIntensity );
			float height107_g1 = temp_output_32_0;
			float scale107_g1 = _NormalStrenght;
			float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
			o.Normal = worldToTangentDir42_g1;
			float4 tex2DNode24 = tex2D( _Detail, ( ase_worldPos / _DetailScale ).xy );
			float temp_output_16_0 = ( ( tex2DNode24.r + tex2DNode21.r ) * temp_output_15_0 * temp_output_18_0 * _MaskIntensity );
			float clampResult28 = clamp( temp_output_16_0 , 0.0 , _MaxClamp );
			float3 temp_output_44_0 = ( (_Rock).rgb * clampResult28 );
			float3 lerpResult12 = lerp( ( (_Sand).rgb + ( tex2DNode24.r * _SandTextureInfluence ) ) , temp_output_44_0 , saturate( temp_output_16_0 ));
			float clampResult131 = clamp( ( tex2D( _Sand_Mask, ( temp_output_5_0 / _Sand_Mask_SCale ) ).g * _Sand_Mask_Srength ) , 0.0 , 1.0 );
			float3 lerpResult117 = lerp( lerpResult12 , temp_output_44_0 , ( 1.0 - clampResult131 ));
			o.Albedo = lerpResult117;
			o.Smoothness = _Roughness;
			o.Alpha = 1;
		}

		ENDCG
		UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
		UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.SwizzleNode;8;-627.4905,72.52185;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-199.7454,328.0374;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-238.7665,221.4061;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;49.24084,79.56345;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-579.6667,184.8333;Inherit;False;Property;_NoiseScale;NoiseScale;24;0;Create;True;0;0;0;False;0;False;0;181.67;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-896.6009,92.21461;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;31;1188.112,441.2654;Inherit;False;Property;_NormalStrenght;NormalStrenght;37;0;Create;True;0;0;0;False;0;False;0;0.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;30;1389.944,308.3299;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;29;972.8574,256.8563;Inherit;False;Property;_MaxClamp;MaxClamp;36;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;786.101,311.8508;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;821.2969,-423.8521;Inherit;False;Property;_Rock;Rock;28;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1226415,0.1039368,0.06652723,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;28;1156.645,180.6286;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;764.54,-844.6609;Inherit;False;Property;_Sand;Sand;29;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4716981,0.2701285,0.1320161,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1256.391,-667.9691;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;24;238.7069,-452.4796;Inherit;True;Property;_Detail;Detail;35;0;Create;True;0;0;0;False;0;False;-1;None;f42d6e9d0e9241543a6e0fe665ef757d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;1213.257,-287.4567;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;45;1384.803,-56.51038;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-414.5034,-123.3232;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-633.6307,-28.34357;Inherit;False;Property;_DisplaceIntensity;DisplaceIntensity;27;0;Create;True;0;0;0;False;0;False;0;84.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-956.6667,-119.1667;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;5;-624.8123,-129.4202;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;48;953.1205,370.3125;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;393.5628,421.9427;Inherit;False;Property;_MaskIntensity;MaskIntensity;41;0;Create;True;0;0;0;False;0;False;0;0.1;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;50;1019.922,-750.802;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;51;-950.4348,-280.7003;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-156.0176,48.79455;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ClampOpNode;34;-223.6255,-175.9527;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-163.2552,-65.36714;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;54;-1244.391,-394.3097;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TransformPositionNode;52;-1081.375,-556.2369;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;53;-1079.675,-709.8849;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;55;-886.1505,2074.727;Inherit;False;871.7434;210.9867;Camera distance mask;4;97;96;95;94;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;56;-145.6686,2338.44;Inherit;False;1228.862;523.7977;Shiny PArticles;8;91;90;89;88;87;86;85;57;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;57;447.6303,2747.238;Inherit;False;Property;_ShinyParticleInfluence;ShinyParticleInfluence;42;0;Create;True;0;0;0;False;0;False;0;0.73;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;1871.271,1546.834;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;59;-460.3709,1458.721;Inherit;False;Property;_Sand1;Sand;51;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8962264,0.4884031,0.05636634,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;60;-463.429,1620.503;Inherit;False;Property;_SandShade;SandShade;52;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8207547,0.3868169,0.2064791,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;61;-151.5496,1457.818;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;62;-143.6286,1613.25;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;63;-47.71965,1016.148;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;64;195.7164,1128.044;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;65;-330.6507,1017.295;Inherit;False;Global;SunDirection;SunDirection;28;0;Create;True;0;0;0;True;0;False;0,0,0;12.72169,146.6568,176.5209;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;66;944.1443,1297.815;Inherit;False;Global;SunColor;SunColor;28;0;Create;True;0;0;0;True;0;False;0,0,0;2.3,1.507363,1.186163;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DesaturateOpNode;67;1169.059,1306.366;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;68;-1102.121,1844.216;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;69;-1300.851,1843.042;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;70;-1662.897,1764.871;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;71;521.3715,1173.153;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;72;-1408.113,1976.248;Inherit;False;Property;_DetailTextureScale;DetailTextureScale;46;0;Create;True;0;0;0;False;0;False;0,0;5.5,32.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ComponentMaskNode;73;-577.3967,1792.885;Inherit;True;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;74;-231.8266,1735.188;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;75;-353.3236,1922.282;Inherit;False;Constant;_Vector6;Vector 6;16;0;Create;True;0;0;0;False;0;False;0.5,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;76;-153.1877,1973.552;Inherit;False;Property;_SandTextureInfluence1;SandTextureInfluence;54;0;Create;True;0;0;0;False;0;False;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;176.0444,1899.121;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;78;782.6894,2029.342;Inherit;False;Normal From Height;-1;;88;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;620.8734,2038.751;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;80;402.1463,1924.826;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-377.5476,1169.838;Inherit;False;Property;_SandShadeBias;SandShadeBias;49;0;Create;True;0;0;0;False;0;False;0;-2.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-383.6566,1253.325;Inherit;False;Property;_SandShadeScale;SandShadeScale;48;0;Create;True;0;0;0;False;0;False;0;4.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-297.9956,1325.584;Inherit;False;Property;_SandShadePower;SandShadePower;47;0;Create;True;0;0;0;False;0;False;0;0.52;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;84;47.08932,1797.475;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;85;522.1494,2411.425;Inherit;False;Property;_Roughness1;Roughness;55;0;Create;True;0;0;0;False;0;False;0;0.17;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;931.1934,2388.44;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-95.66863,2533.228;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;88;246.0193,2529.892;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;778.9604,2544.457;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-29.48467,2676.855;Inherit;False;Property;_ShinyParticleScale;ShinyParticleScale;43;0;Create;True;0;0;0;False;0;False;0;0.005;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;91;394.9254,2523.968;Inherit;True;Property;_ShinyParticles;ShinyParticles;44;0;Create;True;0;0;0;False;0;False;-1;None;42249e2b82201a544b388419e993a1f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;1600.418,1418.365;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;93;1165.616,1514.253;Inherit;False;Property;_SunColorInfluence;SunColorInfluence;45;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-836.1506,2144.475;Inherit;False;Property;_DetailFadeDistance;DetailFadeDistance;50;0;Create;True;0;0;0;False;0;False;0;53.55;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;95;-554.6998,2129.713;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;96;-319.7586,2127.306;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-181.0737,2124.727;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;98;-916.1196,1828.762;Inherit;True;Property;_Detail1;Detail;53;0;Create;True;0;0;0;False;0;False;-1;None;98b64212b777b704896163bb7d44c346;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;99;1230.453,1743.841;Inherit;False;Four Splats First Pass Terrain;0;;89;37452fdfb732e1443b7e39720d05b708;2,102,0,85,0;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.LerpOp;100;856.6054,1674.398;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;49;1063.233,-419.8251;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-389.5453,363.6378;Inherit;False;Property;_HeightMask;HeightMask;30;0;Create;True;0;0;0;False;0;False;0;0.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;102;659.3658,726.4973;Inherit;True;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;1329.944,870.2004;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;105;1899.739,430.4676;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;108;1604.151,603.4073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;101;336.6469,666.6747;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;109;233.5733,817.2938;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;103;1044.241,590.5853;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;110;884.5733,841.2938;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;665.5733,1014.294;Inherit;False;Property;_powermask;powermask;32;0;Create;True;0;0;0;False;0;False;0;19.53;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;853.5975,1119.388;Inherit;False;Property;_topmask;topmask;31;0;Create;True;0;0;0;False;0;False;0;198.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;242.3322,56.20728;Inherit;True;Property;_RockErosion;RockErosion;34;0;Create;True;0;0;0;False;0;False;-1;None;16c5810879a024645affd0126bd2e126;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;27;617.5385,-168.0698;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;776.5353,126.579;Inherit;True;4;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3866.021,-353.0703;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;SH_VenusProps;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;True;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;46;3375.03,-91.3186;Inherit;False;Property;_Roughness;Roughness;40;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;2568.842,-688.4973;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;117;3078.449,-618.5317;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;112;1125.778,-1468.716;Inherit;True;Property;_Sand_Mask;Sand_Mask;33;0;Create;True;0;0;0;False;0;False;-1;9eaeca867a34e7348bfb235ee6800c76;e92591d2af0e500419c0943ff6ba43ed;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-64.56705,-392.9241;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-280.6794,-264.3275;Inherit;False;Property;_DetailScale;DetailScale;25;0;Create;True;0;0;0;False;0;False;0;19.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;118;923.0652,-1292.591;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;119;706.9528,-1163.994;Inherit;False;Property;_Sand_Mask_SCale;Sand_Mask_SCale;26;0;Create;True;0;0;0;False;0;False;0;0.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1130.16,-607.7336;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;849.4469,-506.8412;Inherit;False;Property;_SandTextureInfluence;SandTextureInfluence;39;0;Create;True;0;0;0;False;0;False;0;-0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;1519.081,-1059.29;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;131;1979.273,-1118.618;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;1149.267,-983.9358;Inherit;False;Property;_Sand_Mask_Srength;Sand_Mask_Srength;38;0;Create;True;0;0;0;False;0;False;0;2.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;132;2476.344,-1020.805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;8;0;2;0
WireConnection;18;0;2;2
WireConnection;18;1;19;0
WireConnection;15;0;4;2
WireConnection;22;0;7;0
WireConnection;22;1;3;0
WireConnection;30;20;32;0
WireConnection;30;110;31;0
WireConnection;32;0;21;1
WireConnection;32;1;15;0
WireConnection;32;2;18;0
WireConnection;32;3;47;0
WireConnection;28;0;16;0
WireConnection;28;2;29;0
WireConnection;42;0;50;0
WireConnection;42;1;39;0
WireConnection;24;1;26;0
WireConnection;44;0;49;0
WireConnection;44;1;28;0
WireConnection;45;0;16;0
WireConnection;9;0;5;0
WireConnection;9;1;10;0
WireConnection;5;0;4;0
WireConnection;48;0;32;0
WireConnection;50;0;14;0
WireConnection;7;0;8;0
WireConnection;7;1;9;0
WireConnection;34;0;9;0
WireConnection;11;0;9;0
WireConnection;58;0;92;0
WireConnection;58;1;100;0
WireConnection;61;0;59;0
WireConnection;62;0;60;0
WireConnection;63;0;65;0
WireConnection;64;4;63;0
WireConnection;64;1;81;0
WireConnection;64;2;82;0
WireConnection;64;3;83;0
WireConnection;67;0;66;0
WireConnection;68;0;69;0
WireConnection;68;1;72;0
WireConnection;69;0;70;0
WireConnection;71;0;64;0
WireConnection;73;0;98;0
WireConnection;74;0;73;0
WireConnection;77;0;98;3
WireConnection;77;1;76;0
WireConnection;78;20;79;0
WireConnection;79;0;80;0
WireConnection;79;1;97;0
WireConnection;80;0;77;0
WireConnection;84;0;75;0
WireConnection;84;1;74;0
WireConnection;84;2;97;0
WireConnection;86;0;85;0
WireConnection;86;1;89;0
WireConnection;88;0;87;0
WireConnection;88;1;90;0
WireConnection;89;0;91;1
WireConnection;89;1;57;0
WireConnection;91;1;88;0
WireConnection;92;0;67;0
WireConnection;92;1;93;0
WireConnection;95;0;94;0
WireConnection;96;0;95;0
WireConnection;97;0;96;0
WireConnection;98;1;68;0
WireConnection;99;60;100;0
WireConnection;99;61;78;40
WireConnection;99;57;84;0
WireConnection;99;58;86;0
WireConnection;100;0;61;0
WireConnection;100;1;62;0
WireConnection;100;2;71;0
WireConnection;49;0;13;0
WireConnection;102;0;109;2
WireConnection;106;0;110;0
WireConnection;106;1;107;0
WireConnection;105;0;13;0
WireConnection;105;1;99;0
WireConnection;105;2;108;0
WireConnection;108;0;106;0
WireConnection;103;0;102;0
WireConnection;110;0;102;0
WireConnection;110;1;111;0
WireConnection;21;1;22;0
WireConnection;27;0;24;1
WireConnection;27;1;21;1
WireConnection;16;0;27;0
WireConnection;16;1;15;0
WireConnection;16;2;18;0
WireConnection;16;3;47;0
WireConnection;0;0;117;0
WireConnection;0;1;30;40
WireConnection;0;4;46;0
WireConnection;12;0;42;0
WireConnection;12;1;44;0
WireConnection;12;2;45;0
WireConnection;117;0;12;0
WireConnection;117;1;44;0
WireConnection;117;2;132;0
WireConnection;112;1;118;0
WireConnection;26;0;2;0
WireConnection;26;1;25;0
WireConnection;118;0;5;0
WireConnection;118;1;119;0
WireConnection;39;0;24;1
WireConnection;39;1;41;0
WireConnection;130;0;112;2
WireConnection;130;1;121;0
WireConnection;131;0;130;0
WireConnection;132;0;131;0
ASEEND*/
//CHKSM=4450F988234B9F770AE58966DAB5305CE8877AEE