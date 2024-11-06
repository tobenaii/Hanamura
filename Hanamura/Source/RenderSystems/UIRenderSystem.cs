using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;

namespace Hanamura
{
    public class UIRenderSystem : Renderer
    {
        private readonly FontMaterial _fontMaterial;
        private readonly TextBatch _textBatch;
        private readonly AssetStore _assetStore;
        
        public UIRenderSystem(World world, Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) : base(world)
        {
            _assetStore = assetStore;
            _textBatch = new TextBatch(graphicsDevice);
            _fontMaterial = new FontMaterial(window, graphicsDevice);
        }
        
        public void Render(double alpha, Window window, CommandBuffer cmdBuf, Texture swapchainTexture)
        {
            var renderPass = cmdBuf.BeginRenderPass(
                new ColorTargetInfo()
                {
                    Texture = swapchainTexture.Handle,
                    LoadOp = LoadOp.Load,  // This preserves the previous pass
                    StoreOp = StoreOp.Store
                }
            );
            
            const int fontSize = 48;
            renderPass.BindGraphicsPipeline(_fontMaterial.GraphicsPipeline);
            var textModel = Matrix4x4.CreateTranslation(window.Width / 2f, fontSize, 0);
            var textProj = Matrix4x4.CreateOrthographicOffCenter(
                0,
                window.Width,
                window.Height,
                0,
                0,
                -1
            );
            _textBatch.Start(_assetStore.GetFont("SofiaSans".GetHashCode()));
            _textBatch.Add(
                "Hanamura",
                fontSize,
                Color.White,
                HorizontalAlignment.Center,
                VerticalAlignment.Middle
            );
            _textBatch.UploadBufferData(cmdBuf);
            _textBatch.Render(cmdBuf, renderPass, textModel * textProj);
            cmdBuf.EndRenderPass(renderPass);
        }
    }
}