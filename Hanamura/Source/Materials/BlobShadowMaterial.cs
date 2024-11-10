using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class BlobShadowMaterial
    {
        public readonly GraphicsPipeline GraphicsPipeline;

        public BlobShadowMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore)
        {
            var vertShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("BlobShadow.vert".GetHashCode()),
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
                assetStore.GetShader("BlobShadow.frag".GetHashCode()),
                "main",
                ShaderCross.ShaderFormat.HLSL,
                ShaderStage.Fragment
            );
            
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertShader,
                fragShader
            );
            //disable depth writer
            pipelineCreateInfo.DepthStencilState.EnableDepthWrite = false;
            
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
        }
    }
}