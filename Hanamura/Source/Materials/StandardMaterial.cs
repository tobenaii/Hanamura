using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    public class StandardMaterial : Material
    {
        public readonly Sampler Sampler;

        public StandardMaterial(Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) 
            : base("StandardShader.vert", "StandardShader.frag", assetStore, window, graphicsDevice)
        {
            Sampler = Sampler.Create(graphicsDevice, SamplerCreateInfo.LinearWrap);
        }
    }
}