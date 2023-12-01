// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterRings"
{
	Properties
	{
		_ColorNoise3("ColorNoise3", 2D) = "white" {}
		_Meteor_Tiling("Meteor_Tiling", Vector) = (1,1,0,0)
		_MeteoriteSpeed("Meteorite Speed", Vector) = (1,0,0,0)
		_TopTexture0("Top Texture 0", 2D) = "white" {}
		_CloudStrength("CloudStrength", Float) = 0.5
		_HologramLineScale1("Hologram Line Scale", Float) = 0.5
		_HologramLineSpeed1("Hologram Line Speed", Vector) = (0,2,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred novertexlights nolightmap  nodirlightmap nofog nometa 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			half3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _ColorNoise3;
		uniform half2 _MeteoriteSpeed;
		uniform half2 _Meteor_Tiling;
		uniform half _CloudStrength;
		sampler2D _TopTexture0;
		uniform half _HologramLineScale1;
		uniform half2 _HologramLineSpeed1;


		inline float4 TriplanarSampling297( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
			half4 color276 = IsGammaSpace() ? half4(0.751,0.5274572,0.191505,0) : half4(0.5240808,0.240341,0.03051939,0);
			float2 uv_TexCoord250 = i.uv_texcoord * _Meteor_Tiling;
			half2 panner252 = ( 1.0 * _Time.y * _MeteoriteSpeed + uv_TexCoord250);
			half4 break246 = tex2D( _ColorNoise3, panner252 );
			half temp_output_244_0 = saturate( pow( ( break246.r + break246.g + break246.b ) , 20.0 ) );
			half4 lerpResult315 = lerp( float4( 1,1,1,0 ) , ( color276 * temp_output_244_0 ) , temp_output_244_0);
			o.Albedo = lerpResult315.rgb;
			float3 ase_worldPos = i.worldPos;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			half2 panner295 = ( 1.0 * _Time.y * _HologramLineSpeed1 + float2( 0,0 ));
			float4 triplanar297 = TriplanarSampling297( _TopTexture0, (ase_worldPos*_HologramLineScale1 + half3( panner295 ,  0.0 )), ase_vertexNormal, 1.0, float2( 1,1 ), 1.0, 0 );
			o.Alpha = ( temp_output_244_0 + ( _CloudStrength * triplanar297 ) ).x;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.SaturateNode;244;4590.413,-1258.17;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;241;4380.84,-1223.437;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;246;4022.175,-1232.809;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;247;4259.053,-1236.511;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;243;4265.545,-1097.586;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;231;3720.279,-1238.726;Inherit;True;Property;_ColorNoise3;ColorNoise3;0;0;Create;True;0;0;0;False;0;False;-1;64e6ce3f003f04d40975bcd409397c27;64e6ce3f003f04d40975bcd409397c27;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;253;3313.35,-1135.517;Inherit;False;Property;_MeteoriteSpeed;Meteorite Speed;2;0;Create;True;0;0;0;False;0;False;1,0;0.11,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;250;3287.373,-1265.532;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;252;3534.458,-1208.846;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;251;3099.91,-1246.421;Inherit;False;Property;_Meteor_Tiling;Meteor_Tiling;1;0;Create;True;0;0;0;False;0;False;1,1;23.41,1.79;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;315;5061.169,-1314.851;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;4882.68,-1377.672;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;276;4586.545,-1520.351;Inherit;False;Constant;_Color0;Color 0;10;0;Create;True;0;0;0;False;0;False;0.751,0.5274572,0.191505,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;295;3849.709,-529.0193;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1.63;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;296;3607.555,-529.8854;Inherit;False;Property;_HologramLineSpeed1;Hologram Line Speed;6;0;Create;True;0;0;0;False;0;False;0,2;0,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TriplanarNode;297;4271.516,-658.9587;Inherit;True;Spherical;Object;False;Top Texture 0;_TopTexture0;white;3;Assets/Plugins/AllIn1VfxToolkit/Demo & Assets/Textures/Noise/Noise111.png;Mid Texture 1;_MidTexture1;white;1;None;Bot Texture 1;_BotTexture1;white;4;None;Hologram Lines;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;298;3682.514,-629.3728;Inherit;False;Property;_HologramLineScale1;Hologram Line Scale;5;0;Create;True;0;0;0;False;0;False;0.5;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;292;4057.705,-647.6348;Inherit;False;3;0;FLOAT3;1,0,0;False;1;FLOAT;5.8;False;2;FLOAT3;5.44,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;294;3665.594,-832.9895;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;4714.364,-738.3127;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;305;4903.398,-757.3533;Inherit;True;2;2;0;FLOAT;1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;300;4458.986,-805.288;Inherit;False;Property;_CloudStrength;CloudStrength;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;338;5412.033,-1089.691;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterRings;False;False;False;False;False;True;True;False;True;True;True;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;244;0;241;0
WireConnection;241;0;247;0
WireConnection;241;1;243;0
WireConnection;246;0;231;0
WireConnection;247;0;246;0
WireConnection;247;1;246;1
WireConnection;247;2;246;2
WireConnection;231;1;252;0
WireConnection;250;0;251;0
WireConnection;252;0;250;0
WireConnection;252;2;253;0
WireConnection;315;1;310;0
WireConnection;315;2;244;0
WireConnection;310;0;276;0
WireConnection;310;1;244;0
WireConnection;295;2;296;0
WireConnection;297;9;292;0
WireConnection;292;0;294;0
WireConnection;292;1;298;0
WireConnection;292;2;295;0
WireConnection;299;0;300;0
WireConnection;299;1;297;0
WireConnection;305;0;244;0
WireConnection;305;1;299;0
WireConnection;338;0;315;0
WireConnection;338;9;305;0
ASEEND*/
//CHKSM=3F61D9C2BC412C5283EBA9E998A88994FBE57628