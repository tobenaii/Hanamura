struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

#define SHADOW_SOFTNESS 0.4     // Soft edge falloff
#define SHADOW_INTENSITY 0.5    // Overall opacity

float4 main(VSOutput input) : SV_Target {
    // Convert UV coordinates to centered -1 to 1 space
    float2 centeredUV = (input.TexCoord - 0.5) * 2.0;
    float distance = length(centeredUV);
    
    // Calculate shadow intensity with smooth falloff
    // Now using 1.0 as the outer radius (edge of quad)
    float smoothWidth = SHADOW_SOFTNESS;
    float innerRadius = 1.0 - smoothWidth * 2;
    float outerRadius = 1.0;
    float shadowStrength = 1.0 - smoothstep(innerRadius, outerRadius, distance);
    
    // Apply anti-aliasing using fwidth
    float2 dx = ddx(centeredUV);
    float2 dy = ddy(centeredUV);
    float filterWidth = length(float2(length(dx), length(dy)));
    shadowStrength = lerp(shadowStrength, shadowStrength * (1.0 - filterWidth), 0.5);
    
    // Apply final shadow color
    return float4(0, 0, 0, shadowStrength * SHADOW_INTENSITY);
}