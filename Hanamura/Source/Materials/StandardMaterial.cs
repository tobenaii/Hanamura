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
                ShaderCross.ShaderFormat.HLSL,
                ShaderStage.Vertex,
                new ShaderCross.ShaderResourceInfo()
                {
                    NumUniformBuffers = 1,
                }
            );

            var fragShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("StandardShader.frag".GetHashCode()),
                "main",
                ShaderCross.ShaderFormat.HLSL,
                ShaderStage.Fragment,
                new ShaderCross.ShaderResourceInfo
                {

                    NumSamplers = 1,
                    NumUniformBuffers = 1
                }
            );
            
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertShader,
                fragShader
            );
            
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
            Sampler = Sampler.Create(graphicsDevice, SamplerCreateInfo.LinearWrap);
        }
    }
}