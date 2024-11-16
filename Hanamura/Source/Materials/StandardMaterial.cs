namespace Hanamura
{
    public record struct StandardMaterial(AssetRef Texture) : IMaterial
    {
        public static AssetRef VertexShader => "StandardShader.vert";
        public static AssetRef FragmentShader => "StandardShader.frag";
    }
}