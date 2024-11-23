struct VSInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput {
    float2 TexCoord : TEXCOORD0;
    float4 Position : SV_POSITION;
};

cbuffer UniformBlock : register(b0, space1)
{
    matrix MatrixTransform;
    matrix ModelMatrix;
};

VSOutput main(VSInput input) {
    VSOutput output;
    output.Position = mul(MatrixTransform, float4(input.Position, 1.0));
    output.TexCoord = input.TexCoord;
    return output;
}