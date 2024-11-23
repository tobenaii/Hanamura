struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

#define SHADOW_SOFTNESS 0.4
#define SHADOW_INTENSITY 0.5

float4 main(VSOutput input) : SV_Target {
    float2 centeredUV = (input.TexCoord - 0.5) * 2.0;
    float distance = length(centeredUV);
    
    float smoothWidth = SHADOW_SOFTNESS;
    float innerRadius = 1.0 - smoothWidth * 2;
    float outerRadius = 1.0;
    float shadowStrength = 1.0 - smoothstep(innerRadius, outerRadius, distance);
    
    float2 dx = ddx(centeredUV);
    float2 dy = ddy(centeredUV);
    float filterWidth = length(float2(length(dx), length(dy)));
    shadowStrength = lerp(shadowStrength, shadowStrength * (1.0 - filterWidth), 0.5);
    
    return float4(0, 0, 0, shadowStrength * SHADOW_INTENSITY);
}