struct VSInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VSOutput {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
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