// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_SandTerrainFirstPass"
{
	Properties
	{
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_ShinyParticleInfluence("ShinyParticleInfluence", Float) = 0
		_ShinyParticleScale("ShinyParticleScale", Float) = 0
		_ShinyParticles("ShinyParticles", 2D) = "white" {}
		_SunColorInfluence("SunColorInfluence", Range( 0 , 1)) = 0
		_DetailTextureScale("DetailTextureScale", Vector) = (0,0,0,0)
		_SandShadePower("SandShadePower", Float) = 0
		_SandShadeScale("SandShadeScale", Float) = 0
		_SandShadeBias("SandShadeBias", Float) = 0
		_DetailFadeDistance("DetailFadeDistance", Range( 0 , 1000)) = 0
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
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
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
			float eyeDepth;
			float2 uv_texcoord;
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
		uniform sampler2D _Detail;
		uniform float2 _DetailTextureScale;
		uniform float _SandTextureInfluence;
		uniform float _DetailFadeDistance;
		uniform float3 SunColor;
		uniform float _SunColorInfluence;
		uniform float4 _Sand;
		uniform float4 _SandShade;
		uniform float3 SunDirection;
		uniform float _SandShadeBias;
		uniform float _SandShadeScale;
		uniform float _SandShadePower;
		uniform float _Roughness;
		uniform sampler2D _ShinyParticles;
		uniform float _ShinyParticleScale;
		uniform float _ShinyParticleInfluence;


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


		float3 PerturbNormal107_g88( float3 surf_pos, float3 surf_norm, float height, float scale )
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
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g88 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g88 = ase_worldNormal;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 tex2DNode24 = tex2D( _Detail, ( (ase_vertex3Pos).xz / _DetailTextureScale ) );
			float cameraDepthFade49 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / _DetailFadeDistance);
			float temp_output_51_0 = saturate( ( 1.0 - cameraDepthFade49 ) );
			float height107_g88 = ( (-1.0 + (( tex2DNode24.b * _SandTextureInfluence ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * temp_output_51_0 );
			float scale107_g88 = 1.0;
			float3 localPerturbNormal107_g88 = PerturbNormal107_g88( surf_pos107_g88 , surf_norm107_g88 , height107_g88 , scale107_g88 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g88 = mul( ase_worldToTangent, localPerturbNormal107_g88);
			float3 temp_output_61_0_g89 = worldToTangentDir42_g88;
			o.Normal = temp_output_61_0_g89;
			float3 desaturateInitialColor81 = SunColor;
			float desaturateDot81 = dot( desaturateInitialColor81, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar81 = lerp( desaturateInitialColor81, desaturateDot81.xxx, 1.0 );
			float3 normalizeResult72 = normalize( SunDirection );
			float fresnelNdotV53 = dot( ase_worldNormal, normalizeResult72 );
			float fresnelNode53 = ( _SandShadeBias + _SandShadeScale * pow( 1.0 - fresnelNdotV53, _SandShadePower ) );
			float3 lerpResult56 = lerp( (_Sand).rgb , (_SandShade).rgb , saturate( fresnelNode53 ));
			float4 temp_output_60_0_g89 = float4( lerpResult56 , 0.0 );
			o.Emission = ( float4( ( desaturateVar81 * _SunColorInfluence ) , 0.0 ) * temp_output_60_0_g89 ).xyz;
			o.Smoothness = ( _Roughness + ( tex2D( _ShinyParticles, ( i.uv_texcoord / _ShinyParticleScale ) ).r * _ShinyParticleInfluence ) );
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
Node;AmplifyShaderEditor.CommentaryNode;96;534.8562,-266.7217;Inherit;False;871.7434;210.9867;Camera distance mask;4;50;49;52;51;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;95;1275.338,-3.008278;Inherit;False;1228.862;523.7977;Shiny PArticles;8;46;87;83;84;88;85;82;89;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;14;960.6357,-882.728;Inherit;False;Property;_Sand;Sand;33;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8962264,0.4884031,0.05636634,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;55;957.5776,-720.9463;Inherit;False;Property;_SandShade;SandShade;34;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8207547,0.3868169,0.2064791,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;57;1269.457,-883.6312;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;58;1277.378,-728.1987;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;72;1373.287,-1325.301;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;56;2306.612,-590.0513;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;53;1616.723,-1213.405;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;71;1090.356,-1324.154;Inherit;False;Global;SunDirection;SunDirection;28;0;Create;True;0;0;0;True;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;77;2365.151,-1043.634;Inherit;False;Global;SunColor;SunColor;28;0;Create;True;0;0;0;True;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DesaturateOpNode;81;2590.065,-1035.083;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;26;318.8853,-497.2329;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;8;120.1552,-498.4067;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PosVertexDataNode;48;-241.8911,-576.5784;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;62;1942.378,-1168.296;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;73;12.89317,-365.2007;Inherit;False;Property;_DetailTextureScale;DetailTextureScale;28;0;Create;True;0;0;0;False;0;False;0,0;5.5,32.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;24;504.8871,-512.6872;Inherit;True;Property;_Detail;Detail;35;0;Create;True;0;0;0;False;0;False;-1;None;98b64212b777b704896163bb7d44c346;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;92;843.61,-548.5641;Inherit;True;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;94;1189.18,-606.261;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;91;1067.683,-419.1671;Inherit;False;Constant;_Vector6;Vector 6;16;0;Create;True;0;0;0;False;0;False;0.5,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;41;1267.819,-367.8964;Inherit;False;Property;_SandTextureInfluence;SandTextureInfluence;36;0;Create;True;0;0;0;False;0;False;0;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;1597.051,-442.3283;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;75;2203.696,-312.1063;Inherit;False;Normal From Height;-1;;88;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;2041.88,-302.6973;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;64;1823.153,-416.6229;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1043.459,-1171.611;Inherit;False;Property;_SandShadeBias;SandShadeBias;31;0;Create;True;0;0;0;False;0;False;0;-1.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1037.35,-1088.124;Inherit;False;Property;_SandShadeScale;SandShadeScale;30;0;Create;True;0;0;0;False;0;False;0;4.66;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;1123.011,-1015.865;Inherit;False;Property;_SandShadePower;SandShadePower;29;0;Create;True;0;0;0;False;0;False;0;1.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;90;1468.096,-543.9743;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;46;1943.156,69.97663;Inherit;False;Property;_Roughness;Roughness;37;0;Create;True;0;0;0;False;0;False;0;0.17;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;2352.2,46.99172;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;83;1325.338,191.7793;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;1667.026,188.4437;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;2199.967,203.0082;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;1391.522,335.4064;Inherit;False;Property;_ShinyParticleScale;ShinyParticleScale;25;0;Create;True;0;0;0;False;0;False;0;0.005;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;1815.932,182.5197;Inherit;True;Property;_ShinyParticles;ShinyParticles;26;0;Create;True;0;0;0;False;0;False;-1;None;42249e2b82201a544b388419e993a1f0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;89;1868.637,405.7895;Inherit;False;Property;_ShinyParticleInfluence;ShinyParticleInfluence;24;0;Create;True;0;0;0;False;0;False;0;0.73;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;47;2642.459,-593.6083;Inherit;False;Four Splats First Pass Terrain;0;;89;37452fdfb732e1443b7e39720d05b708;2,102,0,85,0;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3713.264,-685.4211;Float;False;True;-1;0;ASEMaterialInspector;0;0;Standard;SH_SandTerrainFirstPass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;False;-100;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;True;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;3292.277,-794.6146;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;3021.424,-923.0844;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;80;2586.622,-827.1965;Inherit;False;Property;_SunColorInfluence;SunColorInfluence;27;0;Create;True;0;0;0;False;0;False;0;0.346;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;584.856,-196.9731;Inherit;False;Property;_DetailFadeDistance;DetailFadeDistance;32;0;Create;True;0;0;0;False;0;False;0;57;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;49;866.3068,-211.735;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;52;1101.248,-214.142;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;51;1239.933,-216.7217;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;57;0;14;0
WireConnection;58;0;55;0
WireConnection;72;0;71;0
WireConnection;56;0;57;0
WireConnection;56;1;58;0
WireConnection;56;2;62;0
WireConnection;53;4;72;0
WireConnection;53;1;59;0
WireConnection;53;2;60;0
WireConnection;53;3;61;0
WireConnection;81;0;77;0
WireConnection;26;0;8;0
WireConnection;26;1;73;0
WireConnection;8;0;48;0
WireConnection;62;0;53;0
WireConnection;24;1;26;0
WireConnection;92;0;24;0
WireConnection;94;0;92;0
WireConnection;39;0;24;3
WireConnection;39;1;41;0
WireConnection;75;20;65;0
WireConnection;65;0;64;0
WireConnection;65;1;51;0
WireConnection;64;0;39;0
WireConnection;90;0;91;0
WireConnection;90;1;94;0
WireConnection;90;2;51;0
WireConnection;87;0;46;0
WireConnection;87;1;88;0
WireConnection;84;0;83;0
WireConnection;84;1;85;0
WireConnection;88;0;82;1
WireConnection;88;1;89;0
WireConnection;82;1;84;0
WireConnection;47;60;56;0
WireConnection;47;61;75;40
WireConnection;47;57;90;0
WireConnection;47;58;87;0
WireConnection;0;1;47;14
WireConnection;0;2;78;0
WireConnection;0;4;47;45
WireConnection;78;0;79;0
WireConnection;78;1;47;0
WireConnection;79;0;81;0
WireConnection;79;1;80;0
WireConnection;49;0;50;0
WireConnection;52;0;49;0
WireConnection;51;0;52;0
ASEEND*/
//CHKSM=091B1EF69AF989FA044AA4DE1BF5F488852AABAA