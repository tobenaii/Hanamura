using MoonWorks.Graphics;

namespace Hanamura
{
    public class HanaGraphics
    {
        public static GraphicsPipelineCreateInfo GetStandardGraphicsPipelineCreateInfo(
            TextureFormat swapchainFormat,
            Shader vertShader,
            Shader fragShader
        )
        {
            return new GraphicsPipelineCreateInfo
            {
                TargetInfo = new GraphicsPipelineTargetInfo
                {
                    ColorTargetDescriptions =
                    [
                        new ColorTargetDescription
                        {
                            Format = swapchainFormat,
                            BlendState = ColorTargetBlendState.PremultipliedAlphaBlend
                        }
                    ],
                    DepthStencilFormat = TextureFormat.D32Float,
                    HasDepthStencilTarget = true
                },
                DepthStencilState = new DepthStencilState
                {
                    EnableDepthTest = true,
                    EnableDepthWrite = true,
                    CompareOp = CompareOp.LessOrEqual,
                },
                MultisampleState = new MultisampleState
                {
                    SampleCount = RenderSystem.DefaultSampleCount,
                },
                PrimitiveType = PrimitiveType.TriangleList,
                RasterizerState = RasterizerState.CCW_CullBack,
                VertexInputState = VertexInputState.CreateSingleBinding<VertexPositionNormalTexture>(),
                VertexShader = vertShader,
                FragmentShader = fragShader,
            };
        }
    }
}