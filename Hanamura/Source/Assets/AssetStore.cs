using System.Runtime.InteropServices;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;
using SDL3;

namespace Hanamura
{
    public class AssetStore
    {
        private static string AssetPath => SDL.SDL_GetBasePath() + "Content/";
        
        private readonly Dictionary<int, string> _shaders = new();
        private readonly Dictionary<int, Texture> _textures = new();
        private readonly Dictionary<int, Mesh> _meshes = new();
        private readonly Dictionary<int, Font> _fonts = new();

        public AssetStore(GraphicsDevice graphicsDevice)
        {
            LoadShaders();
            LoadTextures(graphicsDevice);
            LoadMeshes(graphicsDevice);
            LoadFonts(graphicsDevice);
        }
        
        public string GetShader(int shaderId)
        {
            return _shaders[shaderId];
        }
        
        public Texture GetTexture(int textureId)
        {
            return _textures[textureId];
        }
        
        public Mesh GetMesh(int meshId)
        {
            return _meshes[meshId];
        }
        
        public Font GetFont(int fontId)
        {
            return _fonts[fontId];
        }
        
        private void LoadShaders()
        {
            var shaderPaths = Directory.GetFiles(AssetPath + "Shaders", "*.hlsl", SearchOption.AllDirectories);
            foreach (var path in shaderPaths)
            {
                _shaders.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), path);
            }
        }
        
        private unsafe void LoadTextures(GraphicsDevice graphicsDevice)
        {
            var texturePaths = Directory.GetFiles(AssetPath + "Textures", "*.png", SearchOption.AllDirectories);
            var resourceUploader = new ResourceUploader(graphicsDevice);
            var cmdBuf = graphicsDevice.AcquireCommandBuffer();
            foreach (var path in texturePaths)
            {
                using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var length = fileStream.Length;
                var buffer = NativeMemory.Alloc((nuint) length);
                var data = new Span<byte>(buffer, (int) length);
                fileStream.ReadExactly(data);
                ImageUtils.ImageInfoFromBytes(data, out var width, out var height, out var _);
                
                var mipLevels = (uint)Math.Floor(Math.Log2(Math.Max(width, height))) + 1;
                var texture = Texture.Create2D(
                    graphicsDevice,
                    width,
                    height,
                    TextureFormat.R8G8B8A8Unorm,
                    TextureUsageFlags.Sampler,
                    mipLevels
                );
                
                var region = new TextureRegion
                {
                    Texture = texture.Handle,
                    W = width,
                    H = height,
                    D = 1
                };
                resourceUploader.SetTextureDataFromCompressed(region, data);
                NativeMemory.Free(buffer);
                _textures.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), texture);
                SDL.SDL_GenerateMipmapsForGPUTexture(cmdBuf.Handle, texture.Handle);
            }
            resourceUploader.Upload();
            resourceUploader.Dispose();
            graphicsDevice.Submit(cmdBuf);
        }
        
        private void LoadMeshes(GraphicsDevice graphicsDevice)
        {
            var modelPaths = Directory.GetFiles(AssetPath + "Models", "*.glb", SearchOption.AllDirectories);
            foreach (var path in modelPaths)
            {
                var meshData = GLTFLoader.Load(path);
                var resourceUploader = new ResourceUploader(graphicsDevice, 1024 * 1024);
                var vertexBuffer = resourceUploader.CreateBuffer(meshData.Vertices.AsSpan(), BufferUsageFlags.Vertex);
                var indexBuffer = resourceUploader.CreateBuffer(meshData.Indices.AsSpan(), BufferUsageFlags.Index);
                vertexBuffer.Name = Path.GetFileNameWithoutExtension(path) + " Vertices";
                indexBuffer.Name = Path.GetFileNameWithoutExtension(path) + " Indices";
                resourceUploader.Upload();
                resourceUploader.Dispose();
                _meshes.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), new Mesh(vertexBuffer, indexBuffer));
            }
        }
        
        private void LoadFonts(GraphicsDevice graphicsDevice)
        {
            var fontPaths = Directory.GetFiles(AssetPath + "Fonts", "*.ttf", SearchOption.AllDirectories);
            foreach (var path in fontPaths)
            {
                var font = Font.Load(graphicsDevice, path);
                _fonts.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), font);
            }
        }
    }
}