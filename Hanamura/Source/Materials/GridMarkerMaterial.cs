using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class GridMarkerMaterial
    {
        public readonly GraphicsPipeline GraphicsPipeline;

        public GridMarkerMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore)
        {
            var vertShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("GridMarker.vert".GetHashCode()),
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
                assetStore.GetShader("GridMarker.frag".GetHashCode()),
                "main",
                ShaderCross.ShaderFormat.HLSL,
                ShaderStage.Fragment
            );
            
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertShader,
                fragShader
            );
            pipelineCreateInfo.DepthStencilState.EnableDepthTest = false;
            
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
        }
    }
}