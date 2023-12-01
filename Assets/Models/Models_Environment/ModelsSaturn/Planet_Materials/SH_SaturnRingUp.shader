// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_MasterRings"
{
	Properties
	{
		_CloudStrength("CloudStrength", Float) = 0.5
		_Min("Min", Float) = 0
		_Max("Max", Float) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_XY("XY", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half _CloudStrength;
		uniform sampler2D _TextureSample0;
		uniform half2 _XY;
		uniform half _Min;
		uniform half _Max;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			half4 color276 = IsGammaSpace() ? half4(0.9179938,0.740566,1,1) : half4(0.82349,0.5079454,1,1);
			o.Albedo = color276.rgb;
			float2 uv_TexCoord344 = i.uv_texcoord * _XY;
			half4 temp_cast_1 = (_Min).xxxx;
			half4 temp_cast_2 = (_Max).xxxx;
			half4 clampResult339 = clamp( ( _CloudStrength * tex2D( _TextureSample0, uv_TexCoord344 ) ) , temp_cast_1 , temp_cast_2 );
			o.Alpha = clampResult339.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;4714.364,-738.3127;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;300;4458.986,-805.288;Inherit;False;Property;_CloudStrength;CloudStrength;0;0;Create;True;0;0;0;False;0;False;0.5;0.96;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;338;5412.033,-1089.691;Half;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SH_MasterRings;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ColorNode;276;4900.688,-1211.866;Inherit;False;Constant;_Color0;Color 0;10;0;Create;True;0;0;0;False;0;False;0.9179938,0.740566,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;340;4763.446,-601.4578;Inherit;False;Property;_Min;Min;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;341;4778.347,-494.442;Inherit;False;Property;_Max;Max;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;339;4999.323,-716.4437;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;342;4327.162,-375.7989;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;bca012a566034e047a45fbfeff3cb093;bca012a566034e047a45fbfeff3cb093;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;344;4078.423,-333.5407;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;345;3833.873,-305.9039;Inherit;False;Property;_XY;XY;4;0;Create;True;0;0;0;False;0;False;0,0;5,7.64;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
WireConnection;299;0;300;0
WireConnection;299;1;342;0
WireConnection;338;0;276;0
WireConnection;338;9;339;0
WireConnection;339;0;299;0
WireConnection;339;1;340;0
WireConnection;339;2;341;0
WireConnection;342;1;344;0
WireConnection;344;0;345;0
ASEEND*/
//CHKSM=8BEE829AF792F4650C7D2FFA3C2CBA069B42B77B