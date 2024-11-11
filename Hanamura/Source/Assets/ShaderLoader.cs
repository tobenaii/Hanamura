using System.Text.RegularExpressions;
using MoonWorks.Graphics;

namespace Hanamura
{
    public partial class ShaderLoader
    {
        private readonly record struct ShaderResources(uint NonUniformBuffers, uint Samplers); 
        
        [GeneratedRegex(@"cbuffer\s+(\w+)", RegexOptions.Multiline)]
        private static partial Regex CbufferRegex();
        
        [GeneratedRegex(@"SamplerState\s+(\w+)", RegexOptions.Multiline)]
        private static partial Regex SamplerRegex();
        
        public static Shader LoadShader(string filePath, GraphicsDevice graphicsDevice)
        {
            ShaderStage stage;
            if (filePath.EndsWith(".vert.hlsl"))
            {
                stage = ShaderStage.Vertex;
            }
            else if (filePath.EndsWith(".frag.hlsl"))
            {
                stage = ShaderStage.Fragment;
            }
            else
            {
                throw new Exception("Unknown shader type");
            }
            var shaderResources = ParseShaderFile(filePath);
            var shader = ShaderCross.Create(
                graphicsDevice,
                filePath,
                "main",
                ShaderCross.ShaderFormat.HLSL,
                stage,
                new ShaderCross.ShaderResourceInfo()
                {
                    NumUniformBuffers = shaderResources.NonUniformBuffers,
                    NumSamplers = shaderResources.Samplers
                }
            );
            return shader;
        }
        
        private static ShaderResources ParseShaderFile(string filePath)
        {
            var shaderContent = File.ReadAllText(filePath);
            var uniformBuffers = 0u;
            var samplers = 0u;
            var cbufferMatches = CbufferRegex().Matches(shaderContent);

            foreach (Match match in cbufferMatches)
            {
                if (match.Groups.Count > 1)
                {
                    uniformBuffers++;
                }
            }
            
            var samplerMatches = SamplerRegex().Matches(shaderContent);

            foreach (Match match in samplerMatches)
            {
                if (match.Groups.Count > 1)
                {
                    samplers++;
                }
            }

            return new ShaderResources(uniformBuffers, samplers);
        }
    }
}