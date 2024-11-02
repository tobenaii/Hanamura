using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class RenderSystem : Renderer
    {
        public const SampleCount DefaultSampleCount = SampleCount.Eight;

        private readonly Texture _renderTarget;
        private readonly Texture _depthTexture;
        private readonly Window _window;
        private readonly GraphicsDevice _graphicsDevice;
        
        private readonly StandardMaterialRenderSystem _standardMaterialRenderSystem;
        private readonly FontMaterialRenderSystem _fontMaterialRenderSystem;

        public RenderSystem(World world, AssetStore assetStore, GraphicsDevice graphicsDevice, Window window) :
            base(world)
        {
            _standardMaterialRenderSystem = new StandardMaterialRenderSystem(world, window, graphicsDevice, assetStore);
            _fontMaterialRenderSystem = new FontMaterialRenderSystem(world, window, graphicsDevice, assetStore);
            _graphicsDevice = graphicsDevice;
            _window = window;
            _renderTarget = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                window.SwapchainFormat,
                TextureUsageFlags.ColorTarget,
                1,
                DefaultSampleCount
            );
            _depthTexture = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                TextureFormat.D32Float,
                TextureUsageFlags.DepthStencilTarget | TextureUsageFlags.Sampler,
                1,
                DefaultSampleCount
            );
        }

        public void Draw(double alpha)
        {
            var cmdBuf = _graphicsDevice.AcquireCommandBuffer();
            var swapchainTexture = cmdBuf.AcquireSwapchainTexture(_window);
            if (swapchainTexture != null)
            {
                _standardMaterialRenderSystem.Render(alpha, cmdBuf, swapchainTexture, _renderTarget, _depthTexture);
                _fontMaterialRenderSystem.Draw(alpha, _window, cmdBuf, swapchainTexture);
            }
            _graphicsDevice.Submit(cmdBuf);
        }
    }
}