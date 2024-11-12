namespace Hanamura
{
    public struct StandardMaterial : IMaterial
    {
        public static AssetRef VertexShader => "StandardShader.vert";
        public static AssetRef FragmentShader => "StandardShader.frag";
    }
}