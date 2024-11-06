using System.Runtime.InteropServices;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;
using SDL3;

namespace Hanamura
{
    public class AssetStore
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<int, string> _shaders = new();
        private readonly Dictionary<int, Texture> _textures = new();
        private readonly Dictionary<int, Mesh> _meshes = new();
        private readonly Dictionary<int, Font> _fonts = new();
        private readonly FileSystemWatcher _watcher;
        private readonly string _assetPath;

        private List<string> _requiresReload;

        public AssetStore(GraphicsDevice graphicsDevice, string contentPath)
        {
            _graphicsDevice = graphicsDevice;
            _assetPath = contentPath;
            _requiresReload = new List<string>();
            
            var texturePaths = Directory.GetFiles(_assetPath + "Textures", "*.png", SearchOption.AllDirectories);
            foreach (var path in texturePaths)
            {
                LoadTexture(path, graphicsDevice);
            }
            
            var modelPaths = Directory.GetFiles(_assetPath + "Models", "*.glb", SearchOption.AllDirectories);
            foreach (var path in modelPaths)
            {
                LoadMesh(path, graphicsDevice);
            }
            
            var fontPaths = Directory.GetFiles(_assetPath + "Fonts", "*.ttf", SearchOption.AllDirectories);
            foreach (var path in fontPaths)
            {
                LoadFont(path, graphicsDevice);
            }
            
            var shaderPaths = Directory.GetFiles(_assetPath + "Shaders", "*.hlsl", SearchOption.AllDirectories);
            foreach (var path in shaderPaths)
            {
                _shaders.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), path);
            }
            
            _watcher = new FileSystemWatcher(Path.GetFullPath(contentPath));
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += (sender, args) => { _requiresReload.Add(args.FullPath); };
            _watcher.EnableRaisingEvents = true;
        }

        public void CheckForReload()
        {
            if (_requiresReload.Count == 0) return;
            foreach (var path in _requiresReload)
            {
                var id = Path.GetFileNameWithoutExtension(path).GetHashCode();
                if (_textures.ContainsKey(id))
                {
                    _textures.Remove(id);
                    LoadTexture(path, _graphicsDevice);
                }
                else if (_meshes.ContainsKey(id))
                {
                    _meshes.Remove(id);
                    LoadMesh(path, _graphicsDevice);
                }
                else if (_fonts.ContainsKey(id))
                {
                    _fonts.Remove(id);
                    LoadFont(path, _graphicsDevice);
                }
                else if (_shaders.ContainsKey(id))
                {
                    _shaders[id] = path;
                }
            }
            Console.WriteLine("Reloaded assets");
            _requiresReload.Clear();
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

        private unsafe void LoadTexture(string path, GraphicsDevice graphicsDevice)
        {
            var resourceUploader = new ResourceUploader(graphicsDevice);
            var cmdBuf = graphicsDevice.AcquireCommandBuffer();
            
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
            resourceUploader.Upload();
            resourceUploader.Dispose();
            graphicsDevice.Submit(cmdBuf);
        }

        private void LoadFont(string path, GraphicsDevice graphicsDevice)
        {
            var font = Font.Load(graphicsDevice, path);
            _fonts.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), font);
        }

        private void LoadMesh(string path, GraphicsDevice graphicsDevice)
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
}