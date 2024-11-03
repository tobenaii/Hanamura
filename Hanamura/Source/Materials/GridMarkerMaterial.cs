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
                new ShaderCross.ShaderCreateInfo
                {
                    Stage = ShaderStage.Vertex,
                    Format = ShaderCross.ShaderFormat.HLSL,
                    NumUniformBuffers = 1,
                }
            );

            var fragShader = ShaderCross.Create(
                graphicsDevice,
                assetStore.GetShader("GridMarker.frag".GetHashCode()),
                "main",
                new ShaderCross.ShaderCreateInfo
                {
                    Stage = ShaderStage.Fragment,
                    Format = ShaderCross.ShaderFormat.HLSL,
                }
            );
            
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertShader,
                fragShader
            );
            pipelineCreateInfo.MultisampleState.SampleCount = RenderSystem.DefaultSampleCount;
            
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
        }
    }
}