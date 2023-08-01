//------------------------------------
//             OmniShade
//     Copyright© 2023 OmniShade     
//------------------------------------

//////////////////////////////////////////////////////////////////////////////////////////
// Mapping from Built-In to URP
//////////////////////////////////////////////////////////////////////////////////////////
#if URP
// Vertex input structure
struct appdata_full {
	float4 vertex : POSITION;
	float4 tangent : TANGENT;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 texcoord1 : TEXCOORD1;
	float4 texcoord2 : TEXCOORD2;
	float4 texcoord3 : TEXCOORD3;
	half4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Function renames
#define UnityObjectToClipPos TransformObjectToHClip
#define UnityObjectToWorldNormal TransformObjectToWorldNormal
#define UnityObjectToWorldDir TransformObjectToWorldDir
#define ShadeSH9(x) SampleSH(x.rgb)
#define unity_LightData unity_LightData.rgb
#define unity_ShadowColor _SubtractiveShadowColor

// Macro definitions
#define UNITY_INITIALIZE_OUTPUT ZERO_INITIALIZE
#define UNITY_TRANSFER_FOG(o, outpos) o.fogCoord = ComputeFogFactor(outpos.z);
#define UNITY_APPLY_FOG(coord, col) col = MixFog(col, coord);
#define UNITY_LIGHT_ATTENUATION(atten, i, pos_world) half atten = UnityLightAttenuation(pos_world);
#define TRANSFER_SHADOW_CAST(o, v) o.pos = GetShadowPositionClip(v.vertex.xyz, v.normal);
#define UNITY_LIGHTDATA

// Functions missing from built-in
float3 UnityObjectToViewPos(float3 pos ) {
    return TransformWorldToView(TransformObjectToWorld(pos));
}

#if SHADOW_CASTER
float4 GetShadowPositionClip(float3 vertex, float3 normal) {
	float3 positionWS = TransformObjectToWorld(vertex);
	float3 normalWS = TransformObjectToWorldNormal(normal);
	#if _CASTING_PUNCTUAL_LIGHT_SHADOW
		float3 lightDirectionWS = normalize(_LightPosition - positionWS);
	#else
		float3 lightDirectionWS = _LightDirection;
	#endif
	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
	#if UNITY_REVERSED_Z
		positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
	#else
		positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
	#endif
	return positionCS;
}
#endif

// Custom functions
#if SHADOWS_ENABLED
half UnityLightAttenuation(float3 pos_world) {
	VertexPositionInputs vi = (VertexPositionInputs)0;
	vi.positionWS = pos_world;
	float4 shadowCoord = GetShadowCoord(vi);
	half atten = MainLightRealtimeShadow(shadowCoord);
	#if UNITY_VERSION >= 202102
		half shadowFade = GetMainLightShadowFade(pos_world);
		atten = lerp(atten, 1, shadowFade);
	#endif
	return atten;
}
#endif

uint GetMeshRenderingLightLayerCustom() {
    #if (_LIGHT_LAYERS && UNITY_VERSION >= 202102)
		return (asuint(unity_RenderingLayer.x) & RENDERING_LIGHT_LAYERS_MASK) >> RENDERING_LIGHT_LAYERS_MASK_SHIFT;
    #else
		return 0xFF;
    #endif
}

#else
//////////////////////////////////////////////////////////////////////////////////////////
// Buit-In helper functions
//////////////////////////////////////////////////////////////////////////////////////////
#define TransformObjectToWorld(pos) mul(unity_ObjectToWorld, float4(pos, 1))
#define UNITY_LIGHTDATA half3 unity_LightData = half3(0, 0, dot(_WorldSpaceLightPos0, 1) != 0);
#define TRANSFER_SHADOW_CAST(o, v) o.pos = UnityClipSpaceShadowCasterPos(v.vertex.xyz, v.normal); o.pos = UnityApplyLinearShadowBias(o.pos);

uint GetMeshRenderingLightLayerCustom() {
	return 0xFF;
}

#endif
