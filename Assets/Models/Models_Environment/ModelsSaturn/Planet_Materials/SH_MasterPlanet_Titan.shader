// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterPlanet_Titan"
{
	Properties
	{
		_T_Titan_Alpha("T_Titan_Alpha", 2D) = "white" {}
		_Noise76("Noise76", 2D) = "white" {}
		_Noise78("Noise76", 2D) = "white" {}
		_Noise77("Noise76", 2D) = "white" {}
		_Color_Water("Color_Water", Color) = (0,0,0,0)
		_Color_Water1("Color_Water", Color) = (0,0,0,0)
		_Color_Sand("Color_Sand", Color) = (0,0,0,0)
		_Color_Sand1("Color_Sand", Color) = (0,0,0,0)
		_Tiling_Planet("Tiling_Planet", Vector) = (0,0,0,0)
		_Tiling_Sand("Tiling_Sand", Vector) = (3,3,0,0)
		_Tiling_Water("Tiling_Water", Vector) = (3,3,0,0)
		_Tiling_Clouds_Moving("Tiling_Clouds_Moving", Vector) = (3,3,0,0)
		_Tiling_Clouds("Tiling_Clouds", Vector) = (3,3,0,0)
		_power_Water("power_Water", Float) = 10
		_power_Sand("power_Sand", Float) = 10
		_power_clouds("power_clouds", Float) = 10
		_Multiply_Water("Multiply_Water", Float) = 3.36
		_Cloud_Color("Cloud_Color", Color) = (1,1,1,0)
		_Multiply_clouds("Multiply_clouds", Float) = 3.36
		_Multiply_Sand("Multiply_Sand", Float) = 3.36
		_Sand_Cloud_intensity("Sand_Cloud_intensity", Float) = 2
		_Noise63("Noise63", 2D) = "white" {}
		_Noise37("Noise37", 2D) = "white" {}
		_Cloud_Movement("Cloud_Movement", Vector) = (0.05,0.02,0,0)
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
			float2 uv_texcoord;
		};

		uniform sampler2D _Noise76;
		uniform float2 _Tiling_Sand;
		uniform float4 _Color_Sand;
		uniform float4 _Color_Sand1;
		uniform float _power_Sand;
		uniform float _Multiply_Sand;
		uniform sampler2D _Noise78;
		uniform float4 _Noise78_ST;
		uniform float _Sand_Cloud_intensity;
		uniform float4 _Color_Water;
		uniform float4 _Color_Water1;
		uniform sampler2D _Noise77;
		uniform float2 _Tiling_Water;
		uniform float _power_Water;
		uniform float _Multiply_Water;
		uniform sampler2D _T_Titan_Alpha;
		uniform float2 _Tiling_Planet;
		uniform float4 _Cloud_Color;
		uniform sampler2D _Noise37;
		uniform float2 _Tiling_Clouds;
		uniform float _power_clouds;
		uniform float _Multiply_clouds;
		uniform sampler2D _Noise63;
		uniform float2 _Cloud_Movement;
		uniform float2 _Tiling_Clouds_Moving;


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


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 surf_pos107_g1 = ase_worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 surf_norm107_g1 = ase_worldNormal;
			float2 uv_TexCoord285 = i.uv_texcoord * _Tiling_Sand;
			float4 tex2DNode225 = tex2D( _Noise76, uv_TexCoord285 );
			float height107_g1 = tex2DNode225.r;
			float scale107_g1 = 10.0;
			float3 localPerturbNormal107_g1 = PerturbNormal107_g1( surf_pos107_g1 , surf_norm107_g1 , height107_g1 , scale107_g1 );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g1 = mul( ase_worldToTangent, localPerturbNormal107_g1);
			o.Normal = worldToTangentDir42_g1;
			float4 temp_cast_1 = (_power_Sand).xxxx;
			float2 uv_Noise78 = i.uv_texcoord * _Noise78_ST.xy + _Noise78_ST.zw;
			float4 lerpResult288 = lerp( _Color_Sand , _Color_Sand1 , ( ( pow( tex2DNode225 , temp_cast_1 ) * _Multiply_Sand ) * saturate( ( tex2D( _Noise78, uv_Noise78 ) * _Sand_Cloud_intensity ) ) ));
			float2 uv_TexCoord307 = i.uv_texcoord * _Tiling_Water;
			float clampResult351 = clamp( ( _SinTime.w + 1.0 ) , 0.0 , 1.0 );
			float4 temp_cast_2 = (_power_Water).xxxx;
			float4 lerpResult302 = lerp( _Color_Water , _Color_Water1 , ( pow( ( tex2D( _Noise77, uv_TexCoord307 ) * saturate( ( 1.0 * clampResult351 ) ) ) , temp_cast_2 ) * _Multiply_Water ));
			float2 uv_TexCoord229 = i.uv_texcoord * _Tiling_Planet;
			float4 lerpResult228 = lerp( lerpResult288 , lerpResult302 , tex2D( _T_Titan_Alpha, uv_TexCoord229 ));
			float2 uv_TexCoord339 = i.uv_texcoord * _Tiling_Clouds;
			float4 temp_cast_3 = (_power_clouds).xxxx;
			float2 uv_TexCoord327 = i.uv_texcoord * _Tiling_Clouds_Moving;
			float2 panner341 = ( 1.0 * _Time.y * _Cloud_Movement + uv_TexCoord327);
			float4 tex2DNode322 = tex2D( _Noise63, panner341 );
			float4 lerpResult320 = lerp( lerpResult228 , _Cloud_Color , saturate( ( ( pow( tex2D( _Noise37, uv_TexCoord339 ) , temp_cast_3 ) * _Multiply_clouds ) * tex2DNode322 ) ));
			o.Albedo = lerpResult320.rgb;
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
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;167;6372.225,-483.8236;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterPlanet_Titan;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LerpOp;228;5173.79,-1137.294;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;288;4934.732,-1781.215;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;227;3448.591,-1067.771;Inherit;False;Property;_Color_Water;Color_Water;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.6156863,0.2705882,0.6784314,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;224;4529.868,-607.3705;Inherit;True;Property;_T_Titan_Alpha;T_Titan_Alpha;0;0;Create;True;0;0;0;False;0;False;-1;69cbee3719013fe49b8f2e01fbc1ed0e;69cbee3719013fe49b8f2e01fbc1ed0e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;229;4150.868,-607.3705;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;230;3863.869,-556.3705;Inherit;False;Property;_Tiling_Planet;Tiling_Planet;8;0;Create;True;0;0;0;False;0;False;0,0;3,3;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;231;5014.855,-681.4714;Inherit;False;Normal From Height;-1;;1;1942fe2c5f1a1f94881a33d532e4afeb;0;2;20;FLOAT;0;False;110;FLOAT;1;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;300;4897.441,-489.7085;Inherit;False;Constant;_Float0;Float 0;11;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;298;3458.219,-882.1747;Inherit;False;Property;_Color_Water1;Color_Water;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4905774,0.1780883,0.5471698,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;302;4153.408,-945.8852;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;3887.354,-786.789;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;305;3663.355,-626.789;Inherit;False;Property;_Multiply_Water;Multiply_Water;16;0;Create;True;0;0;0;False;0;False;3.36;-4.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;303;3567.354,-466.7889;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;296;3544.754,-2402.415;Inherit;False;Property;_Color_Sand1;Color_Sand;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8196079,0.3376071,0.2117647,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;283;3523.053,-2607.239;Inherit;False;Property;_Color_Sand;Color_Sand;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.5354179,0.1764706,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;285;3124.005,-1766.965;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;313;4539.714,-1556.386;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;225;3504.274,-1774.692;Inherit;True;Property;_Noise76;Noise76;1;0;Create;True;0;0;0;False;0;False;-1;b9ecc0d2e1ddf6b43a2f2fe5161a9e62;b9ecc0d2e1ddf6b43a2f2fe5161a9e62;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;3846.436,-1508.097;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;286;2924.367,-1754.382;Inherit;False;Property;_Tiling_Sand;Tiling_Sand;9;0;Create;True;0;0;0;False;0;False;3,3;15,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;315;3316.99,-1459.809;Inherit;True;Property;_Noise78;Noise76;2;0;Create;True;0;0;0;False;0;False;-1;b9ecc0d2e1ddf6b43a2f2fe5161a9e62;0e3c9860890070f409adfd08177c4402;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;317;3734.337,-1344.263;Inherit;False;Property;_Sand_Cloud_intensity;Sand_Cloud_intensity;20;0;Create;True;0;0;0;False;0;False;2;1.73;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;319;4025.791,-1440.839;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;320;5571.698,-8.150352;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;323;4787.584,711.3765;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;331;5102.703,898.1912;Inherit;False;Constant;_Float2;Float 2;18;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;330;5499.203,775.9911;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;310;3961.067,-2103.122;Inherit;False;Property;_Multiply_Sand;Multiply_Sand;19;0;Create;True;0;0;0;False;0;False;3.36;1.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;312;3569.401,-2073.619;Inherit;False;Property;_power_Sand;power_Sand;14;0;Create;True;0;0;0;False;0;False;10;4.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;309;4316.136,-1930.28;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;311;3865.067,-1943.122;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;338;4171.489,332.8926;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;333;3969.319,-92.37196;Inherit;True;Property;_Noise37;Noise37;22;0;Create;True;0;0;0;False;0;False;-1;ef2d075270368e145ab4d78fbc5565bc;ef2d075270368e145ab4d78fbc5565bc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;336;3875.824,202.3959;Inherit;False;Property;_power_clouds;power_clouds;15;0;Create;True;0;0;0;False;0;False;10;1.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;335;4267.489,172.8928;Inherit;False;Property;_Multiply_clouds;Multiply_clouds;18;0;Create;True;0;0;0;False;0;False;3.36;0.85;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;327;3310.7,656.5548;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;339;3684.2,-22.14413;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;340;3456.555,58.91105;Inherit;False;Property;_Tiling_Clouds;Tiling_Clouds;12;0;Create;True;0;0;0;False;0;False;3,3;4,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;332;5240.734,634.3574;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;328;2971.623,672.3815;Inherit;False;Property;_Tiling_Clouds_Moving;Tiling_Clouds_Moving;11;0;Create;True;0;0;0;False;0;False;3,3;4,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;341;3582.608,720.9495;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;322;3939.069,633.1192;Inherit;True;Property;_Noise63;Noise63;21;0;Create;True;0;0;0;False;0;False;-1;ff023cd47bb523a4cbc64a19883de5d9;ff023cd47bb523a4cbc64a19883de5d9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;4584.51,269.6349;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;329;5060.183,724.1826;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;344;5305.729,268.4265;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;334;5038.998,231.7071;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;342;3272.772,881.3031;Inherit;False;Property;_Cloud_Movement;Cloud_Movement;23;0;Create;True;0;0;0;False;0;False;0.05,0.02;0.05,0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;321;4814.513,-21.50508;Inherit;False;Property;_Cloud_Color;Cloud_Color;17;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.9433962,0.9433962,0.9433962,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;326;4277.981,928.4765;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;324;3697.281,1012.076;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;325;4036.239,890.7765;Inherit;False;Constant;_Float1;Float 1;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;307;2335.594,-1091.788;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;308;2107.949,-1010.732;Inherit;False;Property;_Tiling_Water;Tiling_Water;10;0;Create;True;0;0;0;False;0;False;3,3;10,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;301;2707.951,-723.1399;Inherit;True;Property;_Noise77;Noise76;3;0;Create;True;0;0;0;False;0;False;-1;b9ecc0d2e1ddf6b43a2f2fe5161a9e62;596805a25b337ce4797911d55518e143;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;348;3167.173,-715.6788;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;347;2455.788,-325.7452;Inherit;False;Constant;_Float3;Float 1;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;345;2697.53,-288.0452;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;349;2941.559,-357.3501;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;306;3311.354,-274.789;Inherit;False;Property;_power_Water;power_Water;13;0;Create;True;0;0;0;False;0;False;10;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;351;2470.423,-108.5108;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;346;2049.23,-220.0457;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;352;2220.069,-124.8377;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;353;2070.569,9.062378;Inherit;False;Constant;_Float4;Float 4;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
WireConnection;167;0;320;0
WireConnection;167;1;231;40
WireConnection;228;0;288;0
WireConnection;228;1;302;0
WireConnection;228;2;224;0
WireConnection;288;0;283;0
WireConnection;288;1;296;0
WireConnection;288;2;313;0
WireConnection;224;1;229;0
WireConnection;229;0;230;0
WireConnection;231;20;225;0
WireConnection;231;110;300;0
WireConnection;302;0;227;0
WireConnection;302;1;298;0
WireConnection;302;2;304;0
WireConnection;304;0;303;0
WireConnection;304;1;305;0
WireConnection;303;0;348;0
WireConnection;303;1;306;0
WireConnection;285;0;286;0
WireConnection;313;0;309;0
WireConnection;313;1;319;0
WireConnection;225;1;285;0
WireConnection;316;0;315;0
WireConnection;316;1;317;0
WireConnection;319;0;316;0
WireConnection;320;0;228;0
WireConnection;320;1;321;0
WireConnection;320;2;344;0
WireConnection;323;0;322;0
WireConnection;323;1;326;0
WireConnection;330;0;329;0
WireConnection;330;1;331;0
WireConnection;309;0;311;0
WireConnection;309;1;310;0
WireConnection;311;0;225;0
WireConnection;311;1;312;0
WireConnection;338;0;333;0
WireConnection;338;1;336;0
WireConnection;333;1;339;0
WireConnection;327;0;328;0
WireConnection;339;0;340;0
WireConnection;332;0;329;0
WireConnection;341;0;327;0
WireConnection;341;2;342;0
WireConnection;322;1;341;0
WireConnection;337;0;338;0
WireConnection;337;1;335;0
WireConnection;329;0;323;0
WireConnection;344;0;334;0
WireConnection;334;0;337;0
WireConnection;334;1;322;0
WireConnection;326;0;325;0
WireConnection;326;1;324;4
WireConnection;307;0;308;0
WireConnection;301;1;307;0
WireConnection;348;0;301;0
WireConnection;348;1;349;0
WireConnection;345;0;347;0
WireConnection;345;1;351;0
WireConnection;349;0;345;0
WireConnection;351;0;352;0
WireConnection;352;0;346;4
WireConnection;352;1;353;0
ASEEND*/
//CHKSM=706E19D8E8B6C5FCEFE0C25747F42C14D5ECBE18