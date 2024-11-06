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

// Improved smooth noise functions
float2 smoothHash2(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * float3(.1031, .1030, .0973));
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.xx + p3.yz) * p3.zy);
}

// Smoother noise using quintic interpolation
float smoothNoise(float2 p)
{
    float2 i = floor(p);
    float2 f = frac(p);
    
    // Quintic interpolation curve
    float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    
    // Generate smooth random values at lattice points
    float2 ga = smoothHash2(i + float2(0.0, 0.0)) * 2.0 - 1.0;
    float2 gb = smoothHash2(i + float2(1.0, 0.0)) * 2.0 - 1.0;
    float2 gc = smoothHash2(i + float2(0.0, 1.0)) * 2.0 - 1.0;
    float2 gd = smoothHash2(i + float2(1.0, 1.0)) * 2.0 - 1.0;
    
    // Calculate dot products
    float va = dot(ga, f - float2(0.0, 0.0));
    float vb = dot(gb, f - float2(1.0, 0.0));
    float vc = dot(gc, f - float2(0.0, 1.0));
    float vd = dot(gd, f - float2(1.0, 1.0));
    
    // Interpolate
    return va + 
           u.x * (vb - va) + 
           u.y * (vc - va) + 
           u.x * u.y * (va - vb - vc + vd);
}

// Fractal Brownian Motion for more natural patterns
float fbm(float2 p)
{
    float value = 0.0;
    float amplitude = 0.5;
    float frequency = 1.0;
    
    // Add multiple layers of noise
    for(int i = 0; i < 4; i++)
    {
        value += amplitude * (smoothNoise(p * frequency) * 0.5 + 0.5);
        amplitude *= 0.5;
        frequency *= 2.0;
        p = mul(p, float2x2(1.6, -1.2, 1.2, 1.6));
    }
    
    return value;
}

// Improved canopy shadow pattern
float canopyShadow(float2 worldPos, float scale)
{
    // Add movement to the pattern
    float2 movement = float2(Time * 0.02, Time * 0.015);
    worldPos = (worldPos + movement) * scale;
    
    // Generate base shadow pattern
    float shadow = fbm(worldPos);
    
    // Adjust contrast and smoothness
    shadow = smoothstep(0.15, 0.85, shadow);
    
    return shadow;
}

float4 main(PSInput input) : SV_TARGET
{
    float3 lightColor = float3(0.7, 0.65, 0.6);
    float ambientStrength = 0.3;
    
    float3 norm = normalize(input.Normal);
    float3 lightDir = -LightDir;
    float diff = max(dot(norm, lightDir), 0.0);
    
    // Calculate canopy shadow with adjusted scale
    float2 worldPos = input.FragPos.xz;
    float shadowPattern = canopyShadow(worldPos, 0.4); // Reduced scale for larger patterns
    
    // Softer shadow blending
    float shadowStrength = 2.0;
    float shadowMask = lerp(1.0, shadowPattern, shadowStrength);
    
    // Adjust light color for more natural shadow tint
    float3 shadowTint = float3(0.8, 0.85, 1.0); // Slightly bluish tint for shadows
    float3 diffuse = lightColor * diff * lerp(shadowTint, float3(1,1,1), shadowMask);
    float3 ambient = lightColor * ambientStrength;
    
    float3 objectColor = MainTexture.Sample(MainSampler, input.TexCoord).xyz;
    float3 result = (ambient + diffuse) * objectColor;
    
    return float4(result, 1.0);
}