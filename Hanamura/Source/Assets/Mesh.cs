using Buffer = MoonWorks.Graphics.Buffer;

namespace Hanamura
{
    public class Mesh
    {
        public Buffer VertexBuffer { get; }
        public Buffer IndexBuffer { get; }
        
        public uint IndexCount => IndexBuffer.Size / sizeof(uint);

        public Mesh(Buffer vertexBuffer, Buffer indexBuffer)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
        }
    }
}