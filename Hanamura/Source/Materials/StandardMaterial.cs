using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class StandardMaterial
    {
        public readonly GraphicsPipeline GraphicsPipeline;
        public readonly Sampler Sampler;

        public StandardMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore)
        {
            var vertShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("StandardShader.vert".GetHashCode()),
                "main",
                new ShaderCross.ShaderCreateInfo
                {
                    Stage = ShaderStage.Vertex,
                    Format = ShaderCross.ShaderFormat.HLSL,
                    NumUniformBuffers = 1,
                }
            );

            var fragShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("StandardShader.frag".GetHashCode()),
                "main",
                new ShaderCross.ShaderCreateInfo
                {
                    Stage = ShaderStage.Fragment,
                    Format = ShaderCross.ShaderFormat.HLSL,
                    NumSamplers = 1,
                    NumUniformBuffers = 1
                }
            );
            
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertShader,
                fragShader
            );
            pipelineCreateInfo.MultisampleState.SampleCount = RenderSystem.DefaultSampleCount;
            
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
            Sampler = Sampler.Create(graphicsDevice, SamplerCreateInfo.LinearWrap);
        }
    }
}