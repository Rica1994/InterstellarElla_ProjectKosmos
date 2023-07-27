// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VenusTerrainFirstPass"
{
	Properties
	{
		_NoiseScale("NoiseScale", Float) = 0
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_DetailScale("DetailScale", Float) = 0
		_DisplaceIntensity("DisplaceIntensity", Float) = 0
		_Rock("Rock", Color) = (0,0,0,0)
		_Sand("Sand", Color) = (0,0,0,0)
		_HeightMask("HeightMask", Float) = 0
		_RockErosion("RockErosion", 2D) = "white" {}
		_Detail("Detail", 2D) = "white" {}
		_MaxClamp("MaxClamp", Float) = 0
		_NormalStrenght("NormalStrenght", Float) = 0
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
			float2 temp_output_8_0 = (ase_worldPos).xz;
			float2 temp_output_9_0 = ( (ase_worldNormal).xz * _DisplaceIntensity );
			float4 tex2DNode21 = tex2D( _RockErosion, ( ( temp_output_8_0 + temp_output_9_0 ) / _NoiseScale ) );
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
			float3 temp_output_61_0_g2 = worldToTangentDir42_g1;
			o.Normal = temp_output_61_0_g2;
			float4 tex2DNode24 = tex2D( _Detail, ( temp_output_8_0 / _DetailScale ) );
			float temp_output_16_0 = ( ( tex2DNode24.r + tex2DNode21.r ) * temp_output_15_0 * temp_output_18_0 * _MaskIntensity );
			float clampResult28 = clamp( temp_output_16_0 , 0.0 , _MaxClamp );
			float4 lerpResult12 = lerp( ( _Sand + ( tex2DNode24.r * _SandTextureInfluence ) ) , ( _Rock * clampResult28 ) , saturate( temp_output_16_0 ));
			float4 temp_output_60_0_g2 = lerpResult12;
			o.Albedo = temp_output_60_0_g2.xyz;
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
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-123.6903,81.12186;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-389.5453,363.6378;Inherit;False;Property;_HeightMask;HeightMask;29;0;Create;True;0;0;0;False;0;False;0;0.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-199.7454,328.0374;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-238.7665,221.4061;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;49.24084,79.56345;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-579.6667,184.8333;Inherit;False;Property;_NoiseScale;NoiseScale;0;0;Create;True;0;0;0;False;0;False;0;149.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-896.6009,92.21461;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-64.56705,-392.9241;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;1188.112,441.2654;Inherit;False;Property;_NormalStrenght;NormalStrenght;33;0;Create;True;0;0;0;False;0;False;0;3.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;30;1389.944,308.3299;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;29;972.8574,256.8563;Inherit;False;Property;_MaxClamp;MaxClamp;32;0;Create;True;0;0;0;False;0;False;0;7.69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;786.101,311.8508;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;821.2969,-423.8521;Inherit;False;Property;_Rock;Rock;27;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1411765,0.06500428,0.007843133,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;28;1156.645,180.6286;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-280.6794,-264.3275;Inherit;False;Property;_DetailScale;DetailScale;25;0;Create;True;0;0;0;False;0;False;0;19.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;849.4469,-506.8412;Inherit;False;Property;_SandTextureInfluence;SandTextureInfluence;34;0;Create;True;0;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;764.54,-844.6609;Inherit;False;Property;_Sand;Sand;28;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2392157,0.1215686,0.02745098,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1130.16,-607.7336;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1256.391,-667.9691;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;12;1659.4,-440.1513;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;794.7351,182.4791;Inherit;False;4;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;24;238.7069,-452.4796;Inherit;True;Property;_Detail;Detail;31;0;Create;True;0;0;0;False;0;False;-1;None;c9303dd8a46f0544db290a096bb6f29a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;242.3322,56.20728;Inherit;True;Property;_RockErosion;RockErosion;30;0;Create;True;0;0;0;False;0;False;-1;None;bd246906629c7c1408106be5962f745f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;1213.257,-287.4567;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;45;1384.803,-56.51038;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-414.5034,-123.3232;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-633.6307,-28.34357;Inherit;False;Property;_DisplaceIntensity;DisplaceIntensity;26;0;Create;True;0;0;0;False;0;False;0;8.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;34;-191.2981,-143.6254;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-130.9279,-33.03984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-956.6667,-119.1667;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;5;-624.8123,-129.4202;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;48;953.1205,370.3125;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;617.5385,-168.0698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2574.425,-432.5554;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;SH_VenusTerrainFirstPass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;True;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;46;1734.123,-321.1575;Inherit;False;Property;_Roughness;Roughness;35;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;49;2072.053,-411.862;Inherit;False;Four Splats First Pass Terrain;1;;2;37452fdfb732e1443b7e39720d05b708;2,102,0,85,0;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.RangedFloatNode;47;393.5628,421.9427;Inherit;False;Property;_MaskIntensity;MaskIntensity;36;0;Create;True;0;0;0;False;0;False;0;10;0;100;0;1;FLOAT;0
WireConnection;8;0;2;0
WireConnection;7;0;8;0
WireConnection;7;1;9;0
WireConnection;18;0;2;2
WireConnection;18;1;19;0
WireConnection;15;0;4;2
WireConnection;22;0;7;0
WireConnection;22;1;3;0
WireConnection;26;0;8;0
WireConnection;26;1;25;0
WireConnection;30;20;32;0
WireConnection;30;110;31;0
WireConnection;32;0;21;1
WireConnection;32;1;15;0
WireConnection;32;2;18;0
WireConnection;32;3;47;0
WireConnection;28;0;16;0
WireConnection;28;2;29;0
WireConnection;39;0;24;1
WireConnection;39;1;41;0
WireConnection;42;0;14;0
WireConnection;42;1;39;0
WireConnection;12;0;42;0
WireConnection;12;1;44;0
WireConnection;12;2;45;0
WireConnection;16;0;27;0
WireConnection;16;1;15;0
WireConnection;16;2;18;0
WireConnection;16;3;47;0
WireConnection;24;1;26;0
WireConnection;21;1;22;0
WireConnection;44;0;13;0
WireConnection;44;1;28;0
WireConnection;45;0;16;0
WireConnection;9;0;5;0
WireConnection;9;1;10;0
WireConnection;34;0;9;0
WireConnection;11;0;9;0
WireConnection;5;0;4;0
WireConnection;48;0;32;0
WireConnection;27;0;24;1
WireConnection;27;1;21;1
WireConnection;0;0;49;0
WireConnection;0;1;49;14
WireConnection;0;4;49;45
WireConnection;49;60;12;0
WireConnection;49;61;30;40
WireConnection;49;58;46;0
ASEEND*/
//CHKSM=8A6B3470EABD6E219BE537F0B33DAF4DFCD4C222