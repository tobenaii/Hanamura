using System.Numerics;
using System.Runtime.InteropServices;
using glTFLoader;
using glTFLoader.Schema;

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
            var gltf = Interface.LoadModel(gltfPath);
            var positionsBuffer = GetBuffer(gltf, gltfPath, "POSITION");
            var normalsBuffer = GetBuffer(gltf, gltfPath, "NORMAL");
            var uvsBuffer = GetBuffer(gltf, gltfPath, "TEXCOORD_0");
            var indicesBuffer = GetBuffer(gltf, gltfPath, "INDICES");
            
            var positions = MemoryMarshal.Cast<byte, Vector3>(positionsBuffer);
            var normals = MemoryMarshal.Cast<byte, Vector3>(normalsBuffer);
            var uvs = MemoryMarshal.Cast<byte, Vector2>(uvsBuffer);
            var indices = MemoryMarshal.Cast<byte, ushort>(indicesBuffer);
            var indicesUint = new uint[indices.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                indicesUint[i] = indices[i];
            }
            
            var vertexData = new VertexPositionNormalTexture[positions.Length];

            for (var i = 0; i < vertexData.Length; i++)
            {
                var position = positions[i];
                var normal = normals[i];
                var uv = uvs[i];
                vertexData[i] = new VertexPositionNormalTexture(position, normal, uv);
            }
            return new MeshData(vertexData, indicesUint);
        }

        private static Span<byte> GetBuffer(Gltf gltf, string gltfFilePath, string attribute)
        {
            var mesh = gltf.Meshes[0];
            var primitive = mesh.Primitives[0];
    
            var positionAccessorIndex = attribute == "INDICES" ? primitive.Indices!.Value : primitive.Attributes[attribute];
            var accessor = gltf.Accessors[positionAccessorIndex];
    
            var bufferView = gltf.BufferViews[accessor.BufferView!.Value];
            var bufferData = gltf.LoadBinaryBuffer(bufferView.Buffer, gltfFilePath);
            var startPosition = bufferView.ByteOffset + accessor.ByteOffset;
            var componentSize = accessor.ComponentType switch
            {
                Accessor.ComponentTypeEnum.UNSIGNED_BYTE => 1,
                Accessor.ComponentTypeEnum.BYTE => 1,
                Accessor.ComponentTypeEnum.UNSIGNED_SHORT => 2,
                Accessor.ComponentTypeEnum.SHORT => 2,
                Accessor.ComponentTypeEnum.UNSIGNED_INT => 4,
                Accessor.ComponentTypeEnum.FLOAT => 4,
                _ => throw new FileLoadException("Component type not supported")
            }; 
            var numComponents = accessor.Type switch
            {
                Accessor.TypeEnum.SCALAR => 1,
                Accessor.TypeEnum.VEC2 => 2,
                Accessor.TypeEnum.VEC3 => 3,
                Accessor.TypeEnum.VEC4 => 4,
                Accessor.TypeEnum.MAT2 => 4,
                Accessor.TypeEnum.MAT3 => 9,
                Accessor.TypeEnum.MAT4 => 16,
                _ => throw new FileLoadException("Component type not supported")
            };
            
            var totalBytes = accessor.Count * componentSize * numComponents;
            return bufferData.AsSpan(startPosition, totalBytes);
        }
    }
}