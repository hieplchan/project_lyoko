void WorldToScreenPos_float(float3 worldVector, out float3 screenVector)
{
    screenVector = mul(UNITY_MATRIX_MV, float4(worldVector, 1.0)).xyz;    
}