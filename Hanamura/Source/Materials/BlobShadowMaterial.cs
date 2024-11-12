using MoonWorks.Graphics;

namespace Hanamura
{
    public struct BlobShadowMaterial : IMaterial
    {
        
        public static AssetRef VertexShader => "BlobShadow.vert";
        public static AssetRef FragmentShader => "BlobShadow.frag";

        public static void ConfigurePipeline(ref GraphicsPipelineCreateInfo pipelineCreateInfo)
        {
            pipelineCreateInfo.DepthStencilState.EnableDepthWrite = false;
        }
    }
}