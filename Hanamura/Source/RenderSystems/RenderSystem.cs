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
        
        private readonly MainRenderSystem _mainRenderSystem;
        private readonly UIRenderSystem _uiRenderSystem;

        public RenderSystem(World world, AssetStore assetStore, GraphicsDevice graphicsDevice, Window window) :
            base(world)
        {
            _mainRenderSystem = new MainRenderSystem(world, window, graphicsDevice, assetStore);
            _uiRenderSystem = new UIRenderSystem(world, window, graphicsDevice, assetStore);
            
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
                _mainRenderSystem.Render(alpha, cmdBuf, swapchainTexture, _renderTarget, _depthTexture);
                _uiRenderSystem.Render(alpha, _window, cmdBuf, swapchainTexture);
            }
            _graphicsDevice.Submit(cmdBuf);
        }
    }
}