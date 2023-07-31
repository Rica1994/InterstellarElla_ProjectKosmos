// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_VenusTerrainFirstPass"
{
	Properties
	{
		_NoiseScale("NoiseScale", Float) = 0
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_RemapMax("RemapMax", Float) = 0
		_RemapMin("RemapMin", Float) = 0
		_DetailScale("DetailScale", Float) = 0
		_DisplaceIntensity("DisplaceIntensity", Float) = 0
		_Rock("Rock", Color) = (0,0,0,0)
		_Sand("Sand", Color) = (0,0,0,0)
		_Gravel("Gravel", Color) = (0,0,0,0)
		_HeightMask("HeightMask", Float) = 0
		_RockErosion("RockErosion", 2D) = "white" {}
		_Detail("Detail", 2D) = "white" {}
		_MaxClamp("MaxClamp", Float) = 0
		_NormalStrenght("NormalStrenght", Float) = 0
		_SandTextureInfluence("SandTextureInfluence", Float) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0
		_MaskIntensity("MaskIntensity", Range( 0 , 100)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "SplatCount"="2" }
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
			half3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform sampler2D _Mask2;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask1;
		uniform sampler2D _Mask3;
		uniform half4 _MaskMapRemapScale0;
		uniform half4 _MaskMapRemapOffset2;
		uniform half4 _MaskMapRemapScale2;
		uniform half4 _MaskMapRemapScale1;
		uniform half4 _MaskMapRemapOffset1;
		uniform half4 _MaskMapRemapScale3;
		uniform half4 _MaskMapRemapOffset3;
		uniform half4 _MaskMapRemapOffset0;
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
		uniform half _DisplaceIntensity;
		uniform half _NoiseScale;
		uniform half _HeightMask;
		uniform half _MaskIntensity;
		uniform half _NormalStrenght;
		uniform half4 _Gravel;
		uniform half4 _Sand;
		uniform sampler2D _Control;
		uniform half4 _Control_ST;
		uniform float _Smoothness0;
		uniform sampler2D _Splat0;
		uniform half4 _Splat0_ST;
		uniform float _Smoothness1;
		uniform sampler2D _Splat1;
		uniform half4 _Splat1_ST;
		uniform float _Smoothness2;
		uniform sampler2D _Splat2;
		uniform half4 _Splat2_ST;
		uniform float _Smoothness3;
		uniform sampler2D _Splat3;
		uniform half4 _Splat3_ST;
		uniform half _RemapMin;
		uniform half _RemapMax;
		uniform sampler2D _Detail;
		uniform half _DetailScale;
		uniform half _SandTextureInfluence;
		uniform half4 _Rock;
		uniform half _MaxClamp;
		uniform half _Roughness;


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


		half3 PerturbNormal107_g1( half3 surf_pos, half3 surf_norm, half height, half scale )
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
			half3 surf_pos107_g1 = ase_worldPos;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			half3 surf_norm107_g1 = ase_worldNormal;
			half2 temp_output_8_0 = (ase_worldPos).xz;
			half2 temp_output_9_0 = ( (ase_worldNormal).xz * _DisplaceIntensity );
			half4 tex2DNode21 = tex2D( _RockErosion, ( ( temp_output_8_0 + temp_output_9_0 ) / _NoiseScale ) );
			half temp_output_15_0 = ( 1.0 - ase_worldNormal.y );
			half temp_output_18_0 = ( ase_worldPos.y / _HeightMask );
			half height107_g1 = ( tex2DNode21.r * temp_output_15_0 * temp_output_18_0 * _MaskIntensity );
			half scale107_g1 = _NormalStrenght;
			half3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
			half3 ase_worldTangent = WorldNormalVector( i, half3( 1, 0, 0 ) );
			half3 ase_worldBitangent = WorldNormalVector( i, half3( 0, 1, 0 ) );
			half3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			half3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
			o.Normal = worldToTangentDir42_g1;
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			half4 tex2DNode5_g2 = tex2D( _Control, uv_Control );
			half dotResult20_g2 = dot( tex2DNode5_g2 , float4(1,1,1,1) );
			float SplatWeight22_g2 = dotResult20_g2;
			float localSplatClip74_g2 = ( SplatWeight22_g2 );
			float SplatWeight74_g2 = SplatWeight22_g2;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g2 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g2 = ( tex2DNode5_g2 / ( localSplatClip74_g2 + 0.001 ) );
			half4 temp_output_59_0_g2 = SplatControl26_g2;
			half4 appendResult33_g2 = (half4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			half4 tex2DNode4_g2 = tex2D( _Splat0, uv_Splat0 );
			half3 _Vector1 = half3(1,1,1);
			half4 appendResult258_g2 = (half4(_Vector1 , 1.0));
			half4 tintLayer0253_g2 = appendResult258_g2;
			half4 appendResult36_g2 = (half4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			half4 tex2DNode3_g2 = tex2D( _Splat1, uv_Splat1 );
			half3 _Vector2 = half3(1,1,1);
			half4 appendResult261_g2 = (half4(_Vector2 , 1.0));
			half4 tintLayer1254_g2 = appendResult261_g2;
			half4 appendResult39_g2 = (half4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			half4 tex2DNode6_g2 = tex2D( _Splat2, uv_Splat2 );
			half3 _Vector3 = half3(1,1,1);
			half4 appendResult263_g2 = (half4(_Vector3 , 1.0));
			half4 tintLayer2255_g2 = appendResult263_g2;
			half4 appendResult42_g2 = (half4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			half4 tex2DNode7_g2 = tex2D( _Splat3, uv_Splat3 );
			half3 _Vector4 = half3(1,1,1);
			half4 appendResult265_g2 = (half4(_Vector4 , 1.0));
			half4 tintLayer3256_g2 = appendResult265_g2;
			half4 weightedBlendVar9_g2 = temp_output_59_0_g2;
			half4 weightedBlend9_g2 = ( weightedBlendVar9_g2.x*( appendResult33_g2 * tex2DNode4_g2 * tintLayer0253_g2 ) + weightedBlendVar9_g2.y*( appendResult36_g2 * tex2DNode3_g2 * tintLayer1254_g2 ) + weightedBlendVar9_g2.z*( appendResult39_g2 * tex2DNode6_g2 * tintLayer2255_g2 ) + weightedBlendVar9_g2.w*( appendResult42_g2 * tex2DNode7_g2 * tintLayer3256_g2 ) );
			float4 MixDiffuse28_g2 = weightedBlend9_g2;
			half4 temp_output_60_0_g2 = MixDiffuse28_g2;
			half4 temp_output_49_0 = temp_output_60_0_g2;
			half4 temp_cast_6 = (_RemapMin).xxxx;
			half4 temp_cast_7 = (_RemapMax).xxxx;
			half4 temp_output_75_0 = saturate( (temp_cast_6 + (temp_output_49_0 - float4( 0,0,0,0 )) * (temp_cast_7 - temp_cast_6) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 ))) );
			half3 lerpResult57 = lerp( (_Gravel).rgb , (_Sand).rgb , temp_output_75_0.xyz);
			half4 tex2DNode24 = tex2D( _Detail, ( temp_output_8_0 / _DetailScale ) );
			half temp_output_16_0 = ( ( tex2DNode24.r + tex2DNode21.r ) * temp_output_15_0 * temp_output_18_0 * _MaskIntensity );
			half clampResult28 = clamp( temp_output_16_0 , 0.0 , _MaxClamp );
			half3 lerpResult12 = lerp( ( lerpResult57 + ( tex2DNode24.r * _SandTextureInfluence ) ) , ( (_Rock).rgb * clampResult28 ) , saturate( temp_output_16_0 ));
			o.Albedo = lerpResult12;
			half4 appendResult205_g2 = (half4(_Smoothness0 , _Smoothness1 , _Smoothness2 , _Smoothness3));
			half4 appendResult206_g2 = (half4(tex2DNode4_g2.a , tex2DNode3_g2.a , tex2DNode6_g2.a , tex2DNode7_g2.a));
			half4 defaultSmoothness210_g2 = ( appendResult205_g2 * appendResult206_g2 );
			half dotResult216_g2 = dot( defaultSmoothness210_g2 , SplatControl26_g2 );
			half4 temp_cast_10 = (_Roughness).xxxx;
			half4 lerpResult69 = lerp( ( temp_output_49_0 * dotResult216_g2 ) , temp_cast_10 , saturate( temp_output_16_0 ));
			o.Smoothness = lerpResult69.x;
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
Node;AmplifyShaderEditor.RangedFloatNode;19;-389.5453,363.6378;Inherit;False;Property;_HeightMask;HeightMask;32;0;Create;True;0;0;0;False;0;False;0;0.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-199.7454,328.0374;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-238.7665,221.4061;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;49.24084,79.56345;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-579.6667,184.8333;Inherit;False;Property;_NoiseScale;NoiseScale;0;0;Create;True;0;0;0;False;0;False;0;181.67;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-896.6009,92.21461;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-64.56705,-392.9241;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;1188.112,441.2654;Inherit;False;Property;_NormalStrenght;NormalStrenght;37;0;Create;True;0;0;0;False;0;False;0;-2.84;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;30;1389.944,308.3299;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-280.6794,-264.3275;Inherit;False;Property;_DetailScale;DetailScale;27;0;Create;True;0;0;0;False;0;False;0;23.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1130.16,-607.7336;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;794.7351,182.4791;Inherit;False;4;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;21;242.3322,56.20728;Inherit;True;Property;_RockErosion;RockErosion;34;0;Create;True;0;0;0;False;0;False;-1;None;16c5810879a024645affd0126bd2e126;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-414.5034,-123.3232;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-633.6307,-28.34357;Inherit;False;Property;_DisplaceIntensity;DisplaceIntensity;28;0;Create;True;0;0;0;False;0;False;0;84.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;34;-191.2981,-143.6254;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-130.9279,-33.03984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-956.6667,-119.1667;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SwizzleNode;5;-624.8123,-129.4202;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;617.5385,-168.0698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;934.8384,-382.851;Inherit;False;Property;_Rock;Rock;29;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1132075,0.03991507,0.007119957,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;1386.723,-347.3814;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;24;238.7069,-411.6242;Inherit;True;Property;_Detail;Detail;35;0;Create;True;0;0;0;False;0;False;-1;None;e955a59092a218e4e9861f0d837f8f8f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;41;673.5737,-581.8458;Inherit;False;Property;_SandTextureInfluence;SandTextureInfluence;38;0;Create;True;0;0;0;False;0;False;0;-0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;56;1186.005,-354.1193;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3206.634,-332.8741;Half;False;True;-1;0;ASEMaterialInspector;0;0;Standard;SH_VenusTerrainFirstPass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;1;SplatCount=2;False;0;0;False;;-1;0;False;;0;0;0;True;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;29;1027.981,-149.3203;Inherit;False;Property;_MaxClamp;MaxClamp;36;0;Create;True;0;0;0;False;0;False;0;3.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;28;1211.769,-225.548;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;45;1096.522,178.5516;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;2843.07,-644.4824;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;59;1817.455,-1638.059;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;58;1489.752,-1662.836;Inherit;False;Property;_Gravel;Gravel;31;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.9954984,0.9874213,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;1432.99,-1360.851;Inherit;False;Property;_Sand;Sand;30;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.5159505,0.1666666,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;53;1780.929,-1517.443;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;49;1633.056,-1179.476;Inherit;False;Four Splats First Pass Terrain;1;;2;37452fdfb732e1443b7e39720d05b708;2,102,0,85,0;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.CommentaryNode;62;1022.848,-943.306;Inherit;False;871.7434;210.9867;Camera distance mask;4;66;65;64;63;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CameraDepthFade;64;1354.299,-888.3193;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;65;1589.24,-890.7263;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;66;1727.925,-893.306;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;1072.848,-873.5574;Inherit;False;Property;_DetailFadeDistance;DetailFadeDistance;33;0;Create;True;0;0;0;False;0;False;0;419;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;381.1336,466.6878;Inherit;False;Property;_MaskIntensity;MaskIntensity;40;0;Create;True;0;0;0;False;0;False;0;0.1;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;788.5869,316.8225;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;2155.699,-1024.889;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;12;2900.288,-344.6419;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;69;2487.147,-1014.599;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;46;2130.927,-891.8794;Inherit;False;Property;_Roughness;Roughness;39;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;70;2191.466,-86.32886;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;74;2089.596,-1342.702;Inherit;False;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;75;2317.804,-1296.423;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;73;1862.988,-1334.724;Inherit;False;Property;_RemapMin;RemapMin;26;0;Create;True;0;0;0;False;0;False;0;0.29;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;1827.878,-1259.719;Inherit;False;Property;_RemapMax;RemapMax;25;0;Create;True;0;0;0;False;0;False;0;-1.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;57;2886.602,-1445.333;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;2571.019,-1302.327;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
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
WireConnection;39;0;24;1
WireConnection;39;1;41;0
WireConnection;16;0;27;0
WireConnection;16;1;15;0
WireConnection;16;2;18;0
WireConnection;16;3;47;0
WireConnection;21;1;22;0
WireConnection;9;0;5;0
WireConnection;9;1;10;0
WireConnection;34;0;9;0
WireConnection;11;0;9;0
WireConnection;5;0;4;0
WireConnection;27;0;24;1
WireConnection;27;1;21;1
WireConnection;44;0;56;0
WireConnection;44;1;28;0
WireConnection;24;1;26;0
WireConnection;56;0;13;0
WireConnection;0;0;12;0
WireConnection;0;1;30;40
WireConnection;0;4;69;0
WireConnection;28;0;16;0
WireConnection;28;2;29;0
WireConnection;45;0;16;0
WireConnection;42;0;57;0
WireConnection;42;1;39;0
WireConnection;59;0;58;0
WireConnection;53;0;14;0
WireConnection;64;0;63;0
WireConnection;65;0;64;0
WireConnection;66;0;65;0
WireConnection;32;0;21;1
WireConnection;32;1;15;0
WireConnection;32;2;18;0
WireConnection;32;3;47;0
WireConnection;68;0;49;0
WireConnection;68;1;49;45
WireConnection;12;0;42;0
WireConnection;12;1;44;0
WireConnection;12;2;70;0
WireConnection;69;0;68;0
WireConnection;69;1;46;0
WireConnection;69;2;70;0
WireConnection;70;0;45;0
WireConnection;74;0;49;0
WireConnection;74;3;73;0
WireConnection;74;4;76;0
WireConnection;75;0;74;0
WireConnection;57;0;59;0
WireConnection;57;1;53;0
WireConnection;57;2;75;0
WireConnection;61;0;75;0
WireConnection;61;1;66;0
ASEEND*/
//CHKSM=94574636B28C5EE36E4C7519A43F90842135AFD2