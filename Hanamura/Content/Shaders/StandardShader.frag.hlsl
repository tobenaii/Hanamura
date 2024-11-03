struct PSInput
{
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 FragPos : TEXCOORD2;
};

cbuffer UniformBlock : register(b0, space3)
{
    float3 LightDir;
};

Texture2D MainTexture : register(t0, space2);
SamplerState MainSampler : register(s0, space2);

float4 main(PSInput input) : SV_TARGET
{
    float3 lightColor = float3(0.7, 0.65, 0.6);
    float ambientStrength = 0.3;
    
    float3 norm = normalize(input.Normal);
    float3 lightDir = -LightDir;
    float diff = max(dot(norm, lightDir), 0.0);
    
    float3 diffuse = lightColor * diff;
    float3 ambient = lightColor * ambientStrength;
    float3 objectColor = MainTexture.Sample(MainSampler, input.TexCoord).xyz;
    float3 result = (ambient + diffuse) * objectColor;
    
    return float4(result, 1.0);
}