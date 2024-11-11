using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class BlobShadowMaterial : Material
    {
        protected override bool EnableDepthWrite => false;

        public BlobShadowMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) : 
            base("BlobShadow.vert", "BlobShadow.frag", assetStore, window, graphicsDevice)
        {
        }
    }
}