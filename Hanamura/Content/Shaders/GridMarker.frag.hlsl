struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

float4 main(VSOutput input) : SV_Target {
    // Get pixel size in UV space for proper anti-aliasing
    float2 pixelSize = fwidth(input.TexCoord);
    
    // Calculate distance from all edges
    float2 distFromEdge = min(input.TexCoord, 1.0 - input.TexCoord);
    
    // Border thickness
    float borderThickness = 0.05;
    
    // Anti-aliasing width based on pixel size
    float2 aaWidth = pixelSize * 1.0; // Adjust multiplier as needed
    
    // Calculate inner and outer edges with pixel-relative anti-aliasing
    float2 outerEdge = float2(borderThickness, borderThickness);
    float2 innerEdge = outerEdge - aaWidth;
    
    // Create separate masks for horizontal and vertical borders with proper AA
    float horizontalBorder = smoothstep(outerEdge.y, innerEdge.y, distFromEdge.y);
    float verticalBorder = smoothstep(outerEdge.x, innerEdge.x, distFromEdge.x);
    
    // Combine the borders
    float borderMask = max(horizontalBorder, verticalBorder);
    
    return lerp(float4(0, 0, 0, 0), float4(1, 1, 1, 1), borderMask);
}