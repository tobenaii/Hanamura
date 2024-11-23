#include "includes/lygia/generative/fbm.hlsl"
#include "includes/lygia/lighting/diffuse/lambert.hlsl"

struct PSInput
{
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 FragPos : TEXCOORD2;
};

cbuffer UniformBlock : register(b0, space3)
{
    float3 LightDir;
    float Time;
};

Texture2D MainTexture : register(t0, space2);
SamplerState MainSampler : register(s0, space2);

float canopyShadow(float2 worldPos, float scale)
{
    float2 movement = float2(Time * 0.02, Time * 0.015);
    worldPos = (worldPos + movement) * scale;
    float shadow = fbm(worldPos);
    return shadow;
}

float4 main(PSInput input) : SV_TARGET
{
    float3 lightColor = float3(0.7, 0.65, 0.6);
    float ambientStrength = 0.3;
    
    float3 norm = normalize(input.Normal);
    float3 lightDir = -LightDir;
    float diff = diffuseLambert(norm, lightDir);
    
    float2 worldPos = input.FragPos.xz;
    float shadowPattern = canopyShadow(worldPos, 0.25);
    
    float shadowStrength = 2.0;
    float shadowMask = lerp(1.0, shadowPattern, shadowStrength);
    
    float3 shadowTint = float3(0.8, 0.85, 1.0);
    float3 diffuse = lightColor * diff * lerp(shadowTint, float3(1,1,1), shadowMask);
    float3 ambient = lightColor * ambientStrength;
    
    float3 objectColor = MainTexture.Sample(MainSampler, input.TexCoord).xyz;
    float3 result = (ambient + diffuse) * objectColor;
    
    return float4(result, 1.0);
}