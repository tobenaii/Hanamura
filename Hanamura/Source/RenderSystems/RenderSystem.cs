using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class RenderSystem : Renderer
    {
        private readonly MainRenderSystem _mainRenderSystem;
        private readonly UIRenderSystem _uiRenderSystem;

        public RenderSystem(World world) :
            base(world)
        {
            _mainRenderSystem = new MainRenderSystem(world);
            _uiRenderSystem = new UIRenderSystem(world);
        }

        public void Draw(double alpha, Window window, GraphicsDevice graphicsDevice, AssetStore assetStore)
        {
            var cmdBuf = graphicsDevice.AcquireCommandBuffer();
            var swapchainTexture = cmdBuf.AcquireSwapchainTexture(window);
            if (swapchainTexture != null)
            {
                _mainRenderSystem.Render(alpha, cmdBuf, swapchainTexture, assetStore.RenderTarget, assetStore.DepthTexture, assetStore);
                _uiRenderSystem.Render(alpha, cmdBuf, swapchainTexture, assetStore);
            }
            graphicsDevice.Submit(cmdBuf);
        }
    }
}