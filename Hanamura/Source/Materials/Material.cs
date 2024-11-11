using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public abstract class Material
    {

        public GraphicsPipeline? GraphicsPipeline { get; private set; }
        protected Material(AssetRef vertexShader, AssetRef fragmentShader, AssetStore assetStore, Window window, GraphicsDevice graphicsDevice)
        {
            BuildPipeline(window, graphicsDevice, assetStore, vertexShader, fragmentShader);
            assetStore.OnShaderReloadCallback(vertexShader, _ => BuildPipeline(window, graphicsDevice, assetStore, vertexShader, fragmentShader));
            assetStore.OnShaderReloadCallback(fragmentShader, _ => BuildPipeline(window, graphicsDevice, assetStore, vertexShader, fragmentShader));
        }
        
        private void BuildPipeline(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore, AssetRef vertexShaderAsset, AssetRef fragmentShaderAsset)
        {
            GraphicsPipeline?.Dispose();
            
            var vertexShader = assetStore.GetShader(vertexShaderAsset);
            var fragmentShader = assetStore.GetShader(fragmentShaderAsset);
            var pipelineCreateInfo = HanaGraphics.GetStandardGraphicsPipelineCreateInfo(
                window.SwapchainFormat,
                vertexShader,
                fragmentShader
            );
            GraphicsPipeline = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
        }
    }
}