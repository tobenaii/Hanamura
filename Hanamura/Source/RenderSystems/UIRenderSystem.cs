using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;

namespace Hanamura
{
    public class UIRenderSystem : Renderer
    {
        public UIRenderSystem(World world) : base(world)
        {
        }
        
        public void Render(CommandBuffer cmdBuf, Texture swapchainTexture, AssetStore assetStore)
        {
            var renderSurface = GetSingleton<RenderSurface>();
            var fontMaterial = assetStore.FontMaterial;
            var textBatch = assetStore.TextBatch;
            
            var renderPass = cmdBuf.BeginRenderPass(
                new ColorTargetInfo()
                {
                    Texture = swapchainTexture.Handle,
                    LoadOp = LoadOp.Load,  // This preserves the previous pass
                    StoreOp = StoreOp.Store
                }
            );
            
            const int fontSize = 48;
            renderPass.BindGraphicsPipeline(fontMaterial.GraphicsPipeline);
            var textModel = Matrix4x4.CreateTranslation(renderSurface.Width / 2f, fontSize, 0);
            var textProj = Matrix4x4.CreateOrthographicOffCenter(
                0,
                renderSurface.Width,
                renderSurface.Height,
                0,
                0,
                -1
            );
            textBatch.Start(assetStore.GetFont("SofiaSans"));
            textBatch.Add(
                "Hanamura",
                fontSize,
                Color.White,
                HorizontalAlignment.Center,
                VerticalAlignment.Middle
            );
            textBatch.UploadBufferData(cmdBuf);
            textBatch.Render(cmdBuf, renderPass, textModel * textProj);
            cmdBuf.EndRenderPass(renderPass);
        }
    }
}