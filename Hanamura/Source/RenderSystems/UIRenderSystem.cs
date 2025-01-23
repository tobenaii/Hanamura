using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;
using Waddle;

namespace Hanamura
{
    public class UIRenderSystem : Renderer
    {
        private readonly TextBatch _textBatch;
        private readonly FontMaterial _fontMaterial;
        
        public UIRenderSystem(World world, Window window, GraphicsDevice graphicsDevice) : base(world)
        {
            _textBatch = new TextBatch(graphicsDevice);
            _fontMaterial = new FontMaterial(window, graphicsDevice);
        }
        
        public void Render(CommandBuffer cmdBuf, Texture swapchainTexture)
        {
            var renderSurface = GetSingleton<RenderSurface>();
            
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
            var textModel = Matrix4x4.CreateTranslation(renderSurface.Width / 2f, fontSize, 0);
            var textProj = Matrix4x4.CreateOrthographicOffCenter(
                0,
                renderSurface.Width,
                renderSurface.Height,
                0,
                0,
                -1
            );
            _textBatch.Start(AssetStore.GetFont("SofiaSans"));
            _textBatch.Add(
                "Hanamura",
                fontSize,
                Color.White,
                HorizontalAlignment.Center,
                VerticalAlignment.Middle
            );
            _textBatch.UploadBufferData(cmdBuf);
            _textBatch.Render(renderPass, textModel * textProj);
            cmdBuf.EndRenderPass(renderPass);
        }
    }
}