using System.Numerics;

namespace Hanamura
{
    public static class GLTFLoader
    {
        public class MeshData
        {
            public VertexPositionNormalTexture[] Vertices { get; }
            public uint[] Indices { get; }

            public MeshData(VertexPositionNormalTexture[] vertices, uint[] indices)
            {
                Vertices = vertices;
                Indices = indices;
            }
        }
        
        public static MeshData Load(string gltfPath)
        {
            var gltf = SharpGLTF.Schema2.ModelRoot.Load(gltfPath);

            var mesh = gltf.LogicalMeshes.First();
            var primitive = mesh.Primitives.First();
            var vertices = primitive.GetVertexAccessor("POSITION")
                .AsVector3Array();
            var uvs = primitive.GetVertexAccessor("TEXCOORD_0")
                .AsVector2Array();
            
            var normals = primitive.GetVertexAccessor("NORMAL")
                .AsVector3Array();

            var vertexData = new VertexPositionNormalTexture[vertices.Count];
            for (var i = 0; i < vertices.Count; i++)
            {
                var position = new Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z);
                var uv = new Vector2(uvs[i].X, uvs[i].Y);
                var normal = new Vector3(normals[i].X, normals[i].Y, normals[i].Z);
                vertexData[i] = new VertexPositionNormalTexture(position, normal, uv);
            }

            var indexAccessor = primitive.IndexAccessor;
            var indices = indexAccessor.AsIndicesArray().ToArray();
            
            return new MeshData(vertexData, indices);
        }
    }
}