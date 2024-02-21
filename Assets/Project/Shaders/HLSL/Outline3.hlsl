// https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
// https://kyriota.com/2022/08/02/UnityPixelatedArtStyleInURP/#Edge-Detection

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED

TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D(_CameraDepthNormalsTexture);
SAMPLER(sampler_CameraDepthNormalsTexture);

TEXTURE2D(_CameraNormalsTexture);
SAMPLER(sampler_CameraNormalsTexture);

float DecodeFloatRG(float2 enc)
{
    float2 kDecodeDot = float2(1.0, 1 / 255.0);
    return dot(enc, kDecodeDot);
}

float DecodeDepth(float4 enc)
{
    return DecodeFloatRG(enc.zw);
}

float GetDepthValue(float2 uv)
{
    float maskValue = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uv).r;
    float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;

    // return depth;
    
    if (maskValue > 0)
        return depth;
    else
        return 0;
}

float3 DecodeNormal(float4 enc)
{
    float kScale = 1.7777;
    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
    float g = 2.0 / dot(nn.xyz,nn.xyz);
    float3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

// float3 WorldToScreenNormal(float3 worldNormal)
// {
//     return mul(UNITY_MATRIX_MV, float4(worldNormal, 1.0)).xyz;    
// }

void Outline3_float(float2 UV, float OutlineThickness, float DepthSensitivity,
    float NormalsSensitivity, float ColorSensitivity, float4 OutlineColor,
    float DepthEdgeStrength, float NormalEdgeStrength, out float4 Out)
{
    // Test Texture
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV).xyz;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV).xyz;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, UV).xyz;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV).xyz;
    // Out.xyz = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV));

    // Calculate neighbor UV
    float2 Texel = float2(_CameraColorTexture_TexelSize.x, _CameraColorTexture_TexelSize.y);
    float2 uvSamples[4], uvSamplesNew[4];

    uvSamples[0] = UV + float2(1, 0) * Texel;
    uvSamples[1] = UV + float2(-1, 0) * Texel;
    uvSamples[2] = UV + float2(0, 1) * Texel;
    uvSamples[3] = UV + float2(0, -1) * Texel;

    float OffsetMultiplier = 1;
    uvSamplesNew[0] = UV + float2(1, 0) * Texel * OffsetMultiplier;
    uvSamplesNew[1] = UV + float2(-1, 0) * Texel * OffsetMultiplier;
    uvSamplesNew[2] = UV + float2(0, 1) * Texel * OffsetMultiplier;
    uvSamplesNew[3] = UV + float2(0, -1) * Texel * OffsetMultiplier;

    // Sample Texture
    float depthOrgigin, depthNeighbors[4];
    float3 normalOrigin, normalNeighbors[4];

    depthOrgigin = GetDepthValue(UV);
    
    normalOrigin = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, UV).rgb;
    // normalOrigin = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV));
    for (int i = 0; i < 4; i++)
    {
        depthNeighbors[i] = GetDepthValue(uvSamplesNew[i]);
        // depthNeighbors[i] = GetDepthValue(uvSamples[i]);

        // depthNeighbors[i] = DecodeDepth(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
        normalNeighbors[i] = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uvSamples[i]);
        // normalNeighbors[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
    }
    
    // Depth
    float depthEdgeIndicator = 0.0;
    for (int i = 0; i < 4; i++)
    {
        depthEdgeIndicator += clamp(depthNeighbors[i] - depthOrgigin, 0.0, 1.0);
    }
    depthEdgeIndicator = step(0.005, depthEdgeIndicator);
    // depthEdgeIndicator = smoothstep(0.005, 0.01, depthEdgeIndicator);
    // depthEdgeIndicator = floor(smoothstep(0.01, 0.02, depthEdgeIndicator) * 2.0) / 2.0;
    
    // Normal
    float normalEdgeIndicator = 0.0;
    float3 directionVector = float3(1.0, 1.0, 1.0);
    for (int i = 0; i < 4; i++)
    {
        float normalDiff = dot(normalOrigin - normalNeighbors[i], directionVector);
        float normalIndicator = clamp(smoothstep(-.01, .01, normalDiff), 0.0, 1.0);

        float depthDiff = depthNeighbors[i] - depthOrgigin;
        float depthIndicator = clamp(sign(depthDiff * .25 + .0025), 0.0, 1.0);

        float sharpness = 1 - dot(normalOrigin, normalNeighbors[i]);
        normalIndicator = sharpness * depthIndicator * normalIndicator;

        normalEdgeIndicator += normalIndicator;
    }    
    normalEdgeIndicator = step(0.1, normalEdgeIndicator);

    // Final
    float strength = depthEdgeIndicator > 0.0 ?
        // (1.0 + DepthEdgeStrength * depthEdgeIndicator) :
        (1.0 - DepthEdgeStrength * depthEdgeIndicator) :
        (1.0 + NormalEdgeStrength * normalEdgeIndicator);
    
    float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV);    
    Out.xyz = original.xyz * strength;
    // Out.xyz = pow(original.xyz, 1/strength);
    Out.w = 1;
}

#endif
