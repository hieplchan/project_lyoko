// https://www.youtube.com/watch?v=gw31oF9qITw

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

struct SurfaceVariables
{
    float smoothness;
    float rimThreshold;
    float3 normal;
    float3 view;

    float shininess;
};

float3 CalculateCelShading(Light l, SurfaceVariables s)
{
    // Shadow
    float attenuation = l.shadowAttenuation * l.distanceAttenuation;
    
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
    
    return l.color * (diffuse + max(rim, specular));
}

void LightingCelShaded_float(
    float3 Smoothness, float RimThreshold,
    float3 Position, float3 Normal, float3 View,
    out float3 Color)
{
    SurfaceVariables s;
    s.normal = normalize(Normal);
    s.view = SafeNormalize(View);
    s.smoothness = Smoothness;
    s.shininess = exp2(10 * Smoothness + 1);
    s.rimThreshold = RimThreshold;
    
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
    for (int i = 0; i < pixelLightCount; i++)
    {
        light = GetAdditionalLight(i, Position, 1);
        Color += CalculateCelShading(light, s);
    }
}

#endif
