using MoonWorks.Graphics;

namespace Hanamura
{
    public struct GridMarkerMaterial : IMaterial
    {
        public static AssetRef VertexShader => "GridMarker.vert";
        public static AssetRef FragmentShader => "GridMarker.frag";

        public static void ConfigurePipeline(ref GraphicsPipelineCreateInfo pipelineCreateInfo)
        {
            pipelineCreateInfo.DepthStencilState.EnableDepthTest = false;
        }
    }
}