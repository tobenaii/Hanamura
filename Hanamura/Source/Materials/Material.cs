using MoonWorks.Graphics;

namespace Hanamura
{
    public interface IMaterial
    {
        public static abstract AssetRef VertexShader { get; }
        public static abstract AssetRef FragmentShader { get; }
        
        public static virtual void ConfigurePipeline(ref GraphicsPipelineCreateInfo pipelineCreateInfo)
        {
        }
    }
}