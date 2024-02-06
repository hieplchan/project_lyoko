// https://www.youtube.com/watch?v=gw31oF9qITw

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

struct EdgeConstances
{
    float diffuse;
    float specular;
    float specularOffset;
    float distanceAttenuation;
    float shadowAttenuation;
    float rim;
    float rimOffset;
};

struct SurfaceVariables
{
    float smoothness;
    float rimThreshold;
    float3 normal;
    float3 view;

    float shininess;

    EdgeConstances ec;
};

float3 CalculateCelShading(Light l, SurfaceVariables s)
{
    // Shadow
    float shadowAttenuationSmoothStepped = smoothstep(0.0f, s.ec.shadowAttenuation, l.shadowAttenuation);
    float distanceAttenuationSmoothStepped = smoothstep(0.0f, s.ec.distanceAttenuation, l.distanceAttenuation);
    float attenuation = shadowAttenuationSmoothStepped * distanceAttenuationSmoothStepped;
    
    // Diffuse reflection = dot(normal, dir_from_point_to_light)
    // https://learnwebgl.brown37.net/09_lights/lights_diffuse.html
    float diffuse = saturate(dot(s.normal, l.direction));
    diffuse *= attenuation;
    
    // Specular reflection
    // Blinn–Phong: https://en.wikipedia.org/wiki/Blinn%E2%80%93Phong_reflection_model
    float3 h = SafeNormalize(l.direction + s.view);
    float specular = saturate(dot(s.normal, h));
    specular = pow(specular, s.shininess);
    specular *= diffuse * s.smoothness;

    // Rim reflection
    // https://www.roxlu.com/2014/037/opengl-rim-shader
    float rim = 1 - dot(s.view, s.normal);
    rim *= pow(diffuse, s.rimThreshold);

    // Cel Shading
    diffuse = smoothstep(0.0f, s.ec.diffuse, diffuse);
    specular = s.smoothness * smoothstep(
        (1 - s.smoothness) * s.ec.specular + s.ec.specularOffset,
        s.ec.specular + s.ec.specularOffset,
        specular
    );
    rim = s.smoothness * smoothstep(
        s.ec.rim - 0.5f * s.ec.rimOffset,
        s.ec.rim + 0.5f * s.ec.rimOffset,
        rim
    );
    
    return l.color * (diffuse + max(rim, specular));
}

void LightingCelShaded_float(
    float3 Smoothness, float RimThreshold,
    float3 Position, float3 Normal, float3 View,
    float EdgeDiffuse, float EdgeSpecular, float EdgeSpecularOffset,
    float EdgeDistanceAttenuation, float EdgeShadowAttenuation,
    float EdgeRim, float EdgeRimOffset,
    float2 AdditionalLightSourceConfig,
    out float3 Color)
{
    SurfaceVariables s;
    s.normal = normalize(Normal);
    s.view = SafeNormalize(View);
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * Smoothness + 1);
    s.rimThreshold = RimThreshold;
    
    s.ec.diffuse = EdgeDiffuse;
    s.ec.specular = EdgeSpecular;
    s.ec.specularOffset = EdgeSpecularOffset;
    s.ec.distanceAttenuation = EdgeDistanceAttenuation;
    s.ec.shadowAttenuation = EdgeShadowAttenuation;
    s.ec.rim = EdgeRim;
    s.ec.rimOffset = EdgeRimOffset;
    
    #if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(Position);
    float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
    #endif

    // Main Light
    Light light = GetMainLight(shadowCoord);
    Color = CalculateCelShading(light, s);

    // Additional Lights
    int pixelLightCount = GetAdditionalLightsCount();
    int maxAdditionalLightSource = (int)AdditionalLightSourceConfig;
    for (int i = 0; i < pixelLightCount; i++)
    // for (int i = 0; i < min(maxAdditionalLightSource, pixelLightCount); i++)
    {
        light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, s);
    }
}

#endif
