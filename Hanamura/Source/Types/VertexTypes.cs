using System.Numerics;
using System.Runtime.InteropServices;
using MoonWorks.Graphics;

namespace Hanamura
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
        }

        public static VertexElementFormat[] Formats { get; } =
        [
            VertexElementFormat.Float3,
            VertexElementFormat.Float3,
            VertexElementFormat.Float2,
        ];

        public static uint[] Offsets { get; } =
        [
            0,
            12,
            24,
        ];

        public override string ToString()
        {
            return Position + " | " + TexCoord + " | " + Normal;
        }
    }
}