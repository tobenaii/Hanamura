struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

float4 main(VSOutput input) : SV_Target {
    float2 pixelSize = fwidth(input.TexCoord);
    float2 distFromEdge = min(input.TexCoord, 1.0 - input.TexCoord);
    float borderThickness = 0.05;
    float2 aaWidth = pixelSize * 1.0;
    
    float2 outerEdge = float2(borderThickness, borderThickness);
    float2 innerEdge = outerEdge - aaWidth;
    
    float horizontalBorder = smoothstep(outerEdge.y, innerEdge.y, distFromEdge.y);
    float verticalBorder = smoothstep(outerEdge.x, innerEdge.x, distFromEdge.x);
    
    float borderMask = max(horizontalBorder, verticalBorder);
    
    return lerp(float4(0, 0, 0, 0), float4(1, 1, 1, 1), borderMask);
}