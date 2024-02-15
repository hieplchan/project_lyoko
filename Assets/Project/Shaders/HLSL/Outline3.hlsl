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

void Outline3_float(float2 UV, float OutlineThickness, float DepthSensitivity,
    float NormalsSensitivity, float ColorSensitivity, float4 OutlineColor,
    float DepthEdgeStrength, float NormalEdgeStrength, out float4 Out)
{
    // Test Texture
    Out.w = 1;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV).xyz;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV).xyz;
    // Out.xyz = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV).xyz;
    // Out.xyz = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV));

    // Calculate neighbor UV
    float2 Texel = float2(_CameraColorTexture_TexelSize.x, _CameraColorTexture_TexelSize.y);
    float2 uvSamples[4];

    // float halfScaleFloor = floor(OutlineThickness * 0.5);    
    // float halfScaleCeil = ceil(OutlineThickness * 0.5);
    // uvSamples[0] = UV - float2(Texel.x, Texel.y) * halfScaleFloor; // bot-left
    // uvSamples[1] = UV + float2(Texel.x, Texel.y) * halfScaleCeil;  // top-right
    // uvSamples[2] = UV + float2(Texel.x * halfScaleCeil, -Texel.y * halfScaleFloor); // bot-right
    // uvSamples[3] = UV + float2(-Texel.x * halfScaleFloor, Texel.y * halfScaleCeil); // top-left

    uvSamples[0] = UV + float2(1, 0) * Texel;
    uvSamples[1] = UV + float2(-1, 0) * Texel;
    uvSamples[2] = UV + float2(0, 1) * Texel;
    uvSamples[3] = UV + float2(0, -1) * Texel;

    // Sample Texture
    float depthOrgigin, depthNeighbors[4];
    float3 normalOrigin, normalNeighbors[4], colorSamples[4];

    depthOrgigin = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV).r;;
    normalOrigin = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV));
    for (int i = 0; i < 4; i++)
    {
        depthNeighbors[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;;
        normalNeighbors[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamples[i]));
        colorSamples[i] = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uvSamples[i]).r;;
    }
    
    // Depth
    float depthEdgeIndicator = 0.0;
    for (int i = 0; i < 4; i++)
    {
        depthEdgeIndicator += clamp(depthNeighbors[i] - depthOrgigin, 0.0, 1.0);
    }
    depthEdgeIndicator = floor(smoothstep(0.005, 0.02, depthEdgeIndicator) * 2.0) / 2.0;

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
    DepthEdgeStrength = 0.25;
    NormalEdgeStrength = 0.8;
    float strength = depthEdgeIndicator > 0.0 ?
        (1.0 - DepthEdgeStrength * depthEdgeIndicator) :
        (1.0 + NormalEdgeStrength * normalEdgeIndicator);
    
    // Normals
    // float3 normalFiniteDifference0 = normalNeighbors[1] - normalNeighbors[0];
    // float3 normalFiniteDifference1 = normalNeighbors[3] - normalNeighbors[2];
    // float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
    // edgeNormal = edgeNormal > NormalsSensitivity ? 1 : 0;

    // Color
    // float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
    // float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
    // float edgeColor = sqrt(dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1));
    // edgeColor = edgeColor > ColorSensitivity ? 1 : 0;

    // float edge = max(edgeDepth, max(edgeNormal, edgeColor));
    // float edge = max(edgeDepth, edgeNormal);
    
    float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV);
    // original.xyz = OutlineColor.xyz;
    Out.xyz = original.xyz * strength;
    // Out.xyz = pow(original.xyz, 1/strength);
    // float4 original = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV);
    // original.xyz = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV));
    // Out = ((1 - edge) * original) + (edge * lerp(original, OutlineColor,  OutlineColor.a));
}

#endif
