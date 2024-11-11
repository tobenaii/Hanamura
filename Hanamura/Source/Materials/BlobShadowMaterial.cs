using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class BlobShadowMaterial : Material
    {
        public BlobShadowMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) : 
            base("BlobShadow.vert", "BlobShadow.frag", assetStore, window, graphicsDevice)
        {
        }
    }
}