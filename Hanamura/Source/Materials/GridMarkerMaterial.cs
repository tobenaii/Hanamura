using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class GridMarkerMaterial : Material
    {
        public GridMarkerMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) 
            : base("GridMarker.vert", "GridMarker.frag", assetStore, window, graphicsDevice)
        {
        }
    }
}