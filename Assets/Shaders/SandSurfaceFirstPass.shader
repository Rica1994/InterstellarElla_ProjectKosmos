// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SandSurface"
{
	Properties
	{
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_SandShadePower("SandShadePower", Float) = 0
		_SandShadeScale("SandShadeScale", Float) = 0
		_SandShadeBias("SandShadeBias", Float) = 0
		_DetailFadeDistance("DetailFadeDistance", Range( 0 , 1000)) = 0
		_DetailScale("DetailScale", Float) = 0
		_Sand("Sand", Color) = (0,0,0,0)
		_SandShade("SandShade", Color) = (0,0,0,0)
		_Detail("Detail", 2D) = "white" {}
		_SandTextureInfluence("SandTextureInfluence", Float) = 0
		_Roughness("Roughness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float eyeDepth;
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
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform sampler2D _Normal1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform sampler2D _Normal2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform sampler2D _Normal3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float4 _Sand;
		uniform float4 _SandShade;
		uniform float3 SunDirection;
		uniform float _SandShadeBias;
		uniform float _SandShadeScale;
		uniform float _SandShadePower;
		uniform sampler2D _Detail;
		uniform float _DetailScale;
		uniform float _SandTextureInfluence;
		uniform float _DetailFadeDistance;
		uniform sampler2D _TerrainHolesTexture;
		uniform float4 _TerrainHolesTexture_ST;
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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			ApplyMeshModification(v);;
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g2 = tex2D( _Control, uv_Control );
			float dotResult20_g2 = dot( tex2DNode5_g2 , float4(1,1,1,1) );
			float SplatWeight22_g2 = dotResult20_g2;
			float localSplatClip74_g2 = ( SplatWeight22_g2 );
			float SplatWeight74_g2 = SplatWeight22_g2;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g2 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g2 = ( tex2DNode5_g2 / ( localSplatClip74_g2 + 0.001 ) );
			float4 temp_output_59_0_g2 = SplatControl26_g2;
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar8_g2 = temp_output_59_0_g2;
			float4 weightedBlend8_g2 = ( weightedBlendVar8_g2.x*tex2D( _Normal0, uv_Splat0 ) + weightedBlendVar8_g2.y*tex2D( _Normal1, uv_Splat1 ) + weightedBlendVar8_g2.z*tex2D( _Normal2, uv_Splat2 ) + weightedBlendVar8_g2.w*tex2D( _Normal3, uv_Splat3 ) );
			float3 temp_output_61_0_g2 = UnpackNormal( weightedBlend8_g2 );
			o.Normal = temp_output_61_0_g2;
			float3 normalizeResult72 = normalize( SunDirection );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV53 = dot( ase_worldNormal, normalizeResult72 );
			float fresnelNode53 = ( _SandShadeBias + _SandShadeScale * pow( 1.0 - fresnelNdotV53, _SandShadePower ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 temp_output_8_0 = (ase_vertex3Pos).xz;
			float2 temp_output_26_0 = ( temp_output_8_0 / _DetailScale );
			float4 tex2DNode24 = tex2D( _Detail, temp_output_26_0 );
			float cameraDepthFade49 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / _DetailFadeDistance);
			float temp_output_42_0 = ( saturate( fresnelNode53 ) + ( (-1.0 + (( tex2DNode24.r * _SandTextureInfluence ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * saturate( ( 1.0 - cameraDepthFade49 ) ) ) );
			float3 lerpResult56 = lerp( (_Sand).rgb , (_SandShade).rgb , temp_output_42_0);
			float4 temp_output_60_0_g2 = float4( lerpResult56 , 0.0 );
			float4 localClipHoles100_g2 = ( temp_output_60_0_g2 );
			float2 uv_TerrainHolesTexture = i.uv_texcoord * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
			float holeClipValue99_g2 = tex2D( _TerrainHolesTexture, uv_TerrainHolesTexture ).r;
			float Hole100_g2 = holeClipValue99_g2;
			{
			#ifdef _ALPHATEST_ON
				clip(Hole100_g2 == 0.0f ? -1 : 1);
			#endif
			}
			o.Albedo = localClipHoles100_g2.xyz;
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
Node;AmplifyShaderEditor.SwizzleNode;5;-547.4905,-75.47815;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;8;-627.4905,72.52185;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-326.4905,-56.47815;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-130.9279,-33.03984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-389.5453,363.6378;Inherit;False;Property;_HeightMask;HeightMask;37;0;Create;True;0;0;0;False;0;False;0;16.94;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;18;-199.7454,328.0374;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;15;-238.7665,221.4061;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-543.3899,0.6226678;Inherit;False;Property;_DisplaceIntensity;DisplaceIntensity;33;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-579.6667,184.8333;Inherit;False;Property;_NoiseScale;NoiseScale;3;0;Create;True;0;0;0;False;0;False;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;2;-896.6009,92.21461;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;-64.56705,-392.9241;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;589.7986,67.7207;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;1188.112,441.2654;Inherit;False;Property;_NormalStrenght;NormalStrenght;42;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;30;1389.944,308.3299;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;34;-191.2981,-143.6254;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;-1,-1;False;2;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;29;972.8574,256.8563;Inherit;False;Property;_MaxClamp;MaxClamp;41;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;786.101,311.8508;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;28;1156.645,180.6286;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-280.6794,-264.3275;Inherit;False;Property;_DetailScale;DetailScale;32;0;Create;True;0;0;0;False;0;False;0;16.83;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;794.7351,182.4791;Inherit;False;3;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;261.3197,-676.5705;Inherit;True;Property;_SandDetail;SandDetail;40;0;Create;True;0;0;0;False;0;False;-1;None;ff023cd47bb523a4cbc64a19883de5d9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;242.3322,56.20728;Inherit;True;Property;_RockErosion;RockErosion;38;0;Create;True;0;0;0;False;0;False;-1;None;98b64212b777b704896163bb7d44c346;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;4;-915.8932,-87.8022;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;22;104.6589,81.06124;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;48;-909.1143,237.4039;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;7;106.4799,-26.42063;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;47;2283.26,-295.4053;Inherit;False;Four Splats First Pass Terrain;4;;2;37452fdfb732e1443b7e39720d05b708;2,102,1,85,0;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1607.038,-481.031;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;24;504.8871,-512.6872;Inherit;True;Property;_Detail;Detail;39;0;Create;True;0;0;0;False;0;False;-1;None;b9ecc0d2e1ddf6b43a2f2fe5161a9e62;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;53;1373.873,-1211.683;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;43;1246.723,-30.57225;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;1244.406,-134.1043;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;45;1415.953,96.84206;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;1504.459,-96.7815;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;13;912.9432,-97.07572;Inherit;False;Property;_Rock;Rock;34;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;776.8362,-259.6991;Inherit;False;Property;_DetailFadeDistance;DetailFadeDistance;31;0;Create;True;0;0;0;False;0;False;0;55;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;49;1058.287,-274.461;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;52;1293.228,-276.868;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;51;1431.913,-279.4477;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;1300.804,-387.7133;Inherit;False;Property;_SandTextureInfluence;SandTextureInfluence;43;0;Create;True;0;0;0;False;0;False;0;0.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;960.6357,-882.728;Inherit;False;Property;_Sand;Sand;35;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.6028376,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;55;957.5776,-720.9463;Inherit;False;Property;_SandShade;SandShade;36;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4528302,0.03275183,0.03275183,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;57;1269.457,-883.6312;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;58;1277.378,-728.1987;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1064.269,-1118.785;Inherit;False;Property;_SandShadeBias;SandShadeBias;30;0;Create;True;0;0;0;False;0;False;0;-1.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1058.16,-1035.298;Inherit;False;Property;_SandShadeScale;SandShadeScale;29;0;Create;True;0;0;0;False;0;False;0;3.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;1143.821,-963.0393;Inherit;False;Property;_SandShadePower;SandShadePower;28;0;Create;True;0;0;0;False;0;False;0;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;62;1689.195,-1154.517;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2778.391,-293.2437;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SandSurface;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;-100;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;True;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;46;1869.587,-70.49183;Inherit;False;Property;_Roughness;Roughness;44;0;Create;True;0;0;0;False;0;False;0;0.218;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;64;1748.822,-480.939;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;1952.072,-324.6912;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;69;2117.139,-724.6046;Inherit;False;Sample Lightmap;0;;3;6976f0f966a01684ca0a6dde441141c2;6,209,0,195,0,196,0,238,0,191,0,249,0;2;71;FLOAT3;0,0,0;False;169;FLOAT3;0,0,0;False;2;COLOR;0;COLOR;178
Node;AmplifyShaderEditor.Vector3Node;71;1090.356,-1324.154;Inherit;False;Global;SunDirection;SunDirection;28;0;Create;True;0;0;0;True;0;False;0,0,0;17.97597,145.6897,172.8721;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;72;1373.287,-1325.301;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;2132.941,-483.5656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;56;2306.612,-590.0513;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
WireConnection;5;0;4;0
WireConnection;8;0;48;0
WireConnection;9;0;5;0
WireConnection;9;1;10;0
WireConnection;11;0;34;0
WireConnection;18;0;48;2
WireConnection;18;1;19;0
WireConnection;15;0;4;2
WireConnection;26;0;8;0
WireConnection;26;1;25;0
WireConnection;27;0;24;1
WireConnection;27;1;21;1
WireConnection;30;20;32;0
WireConnection;30;110;31;0
WireConnection;34;0;9;0
WireConnection;32;0;21;1
WireConnection;32;1;15;0
WireConnection;32;2;18;0
WireConnection;28;0;16;0
WireConnection;28;2;29;0
WireConnection;16;0;27;0
WireConnection;16;1;15;0
WireConnection;16;2;18;0
WireConnection;40;1;26;0
WireConnection;21;1;22;0
WireConnection;22;0;8;0
WireConnection;22;1;3;0
WireConnection;7;0;8;0
WireConnection;7;1;11;0
WireConnection;47;60;56;0
WireConnection;47;58;46;0
WireConnection;39;0;24;1
WireConnection;39;1;41;0
WireConnection;24;1;26;0
WireConnection;53;4;72;0
WireConnection;53;1;59;0
WireConnection;53;2;60;0
WireConnection;53;3;61;0
WireConnection;43;0;27;0
WireConnection;44;0;13;0
WireConnection;44;1;28;0
WireConnection;45;0;16;0
WireConnection;12;0;42;0
WireConnection;12;1;44;0
WireConnection;12;2;45;0
WireConnection;49;0;50;0
WireConnection;52;0;49;0
WireConnection;51;0;52;0
WireConnection;57;0;14;0
WireConnection;58;0;55;0
WireConnection;62;0;53;0
WireConnection;0;0;47;0
WireConnection;0;1;47;14
WireConnection;0;4;47;45
WireConnection;64;0;39;0
WireConnection;65;0;64;0
WireConnection;65;1;51;0
WireConnection;72;0;71;0
WireConnection;42;0;62;0
WireConnection;42;1;65;0
WireConnection;56;0;57;0
WireConnection;56;1;58;0
WireConnection;56;2;42;0
ASEEND*/
//CHKSM=0C1424F7FFE0F97AC7E1664120C5D2C84A9787DB