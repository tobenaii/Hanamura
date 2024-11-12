using System.Runtime.InteropServices;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Graphics.Font;
using SDL3;
using Texture = MoonWorks.Graphics.Texture;

namespace Hanamura
{
    public class AssetStore
    {
        private const SampleCount DEFAULT_SAMPLE_COUNT = SampleCount.Eight;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<AssetRef, Texture> _textures = new();
        private readonly Dictionary<AssetRef, Mesh> _meshes = new();
        private readonly Dictionary<AssetRef, Font> _fonts = new();
        private readonly Dictionary<AssetRef, Shader> _shaders = new();
        private readonly Dictionary<AssetRef, GraphicsPipeline> _materials = new();
        private readonly Dictionary<SamplerType, Sampler> _samplers = new();
        private readonly FileSystemWatcher _watcher;
        private readonly List<string> _requiresReload;
        
        public FontMaterial FontMaterial { get; private set; }
        public TextBatch TextBatch { get; private set; }
        public Texture RenderTarget { get; private set; }
        public Texture DepthTexture { get; private set; }

        public AssetStore(GraphicsDevice graphicsDevice, Window window, string contentPath)
        {
            _graphicsDevice = graphicsDevice;
            _requiresReload = new List<string>();

            _samplers.Add(SamplerType.LinearWrap, Sampler.Create(graphicsDevice, SamplerCreateInfo.LinearWrap));
            
            FontMaterial = new FontMaterial(window, graphicsDevice);
            TextBatch = new TextBatch(graphicsDevice);
            
            RenderTarget = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                window.SwapchainFormat,
                TextureUsageFlags.ColorTarget,
                1,
                DEFAULT_SAMPLE_COUNT
            );
            DepthTexture = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                TextureFormat.D32Float,
                TextureUsageFlags.DepthStencilTarget | TextureUsageFlags.Sampler,
                1,
                DEFAULT_SAMPLE_COUNT
            );
            
            var texturePaths = Directory.GetFiles(contentPath + "Textures", "*.png", SearchOption.AllDirectories);
            foreach (var path in texturePaths)
            {
                LoadTexture(path, graphicsDevice);
            }
            
            var modelPaths = Directory.GetFiles(contentPath + "Models", "*.glb", SearchOption.AllDirectories);
            foreach (var path in modelPaths)
            {
                LoadMesh(path, graphicsDevice);
            }
            
            var fontPaths = Directory.GetFiles(contentPath + "Fonts", "*.ttf", SearchOption.AllDirectories);
            foreach (var path in fontPaths)
            {
                LoadFont(path, graphicsDevice);
            }
            
            var shaderPaths = Directory.GetFiles(contentPath + "Shaders", "*.hlsl", SearchOption.AllDirectories);
            foreach (var path in shaderPaths)
            {
                LoadShader(path, graphicsDevice);
            }
            
            _watcher = new FileSystemWatcher(Path.GetFullPath(contentPath));
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += (_, args) =>
            {
                if (File.Exists(args.FullPath) && !args.FullPath.EndsWith('~') && !_requiresReload.Contains(args.FullPath))
                {
                    _requiresReload.Add(args.FullPath);
                }
            };
            _watcher.EnableRaisingEvents = true;
        }
        
        public void RegisterMaterial<T>(TextureFormat swapchainFormat, GraphicsDevice graphicsDevice) where T : IMaterial
        {
            if (_materials.TryGetValue(typeof(T).Name, out var material))
            {
                material.Dispose();
            }
            
            var vertexShader = _shaders[T.VertexShader];
            var fragmentShader = _shaders[T.FragmentShader];
            var pipelineCreateInfo = GetStandardGraphicsPipelineCreateInfo(
                swapchainFormat,
                vertexShader,
                fragmentShader
            );
            T.ConfigurePipeline(ref pipelineCreateInfo);
            var newMaterial = GraphicsPipeline.Create(graphicsDevice, pipelineCreateInfo);
            _materials.Add(typeof(T).Name, newMaterial);
        }

        public void CheckForReload()
        {
            if (_requiresReload.Count == 0) return;
            foreach (var path in _requiresReload)
            {
                var id = Path.GetFileNameWithoutExtension(path);
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
                    _shaders[id].Dispose();
                    _shaders.Remove(id);
                    LoadShader(path, _graphicsDevice);
                }
                Console.WriteLine($"Reloaded {Path.GetFileName(path)}");
            }
            _requiresReload.Clear();
        }
        
        public Shader GetShader(AssetRef shaderId)
        {
            return _shaders[shaderId];
        }
        
        public Texture GetTexture(AssetRef textureId)
        {
            return _textures[textureId];
        }
        
        public Mesh GetMesh(AssetRef meshId)
        {
            return _meshes[meshId];
        }
        
        public Font GetFont(AssetRef fontId)
        {
            return _fonts[fontId];
        }
        
        public GraphicsPipeline GetMaterial<T>() where T : IMaterial
        {
            return _materials[typeof(T).Name];
        }
        
        public Sampler GetSampler(SamplerType type)
        {
            return _samplers[type];
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
            ImageUtils.ImageInfoFromBytes(data, out var width, out var height, out _);
                
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
        
        private void LoadShader(string path, GraphicsDevice graphicsDevice)
        {
            var shader = ShaderLoader.LoadShader(path, graphicsDevice);
            _shaders.Add(Path.GetFileNameWithoutExtension(path).GetHashCode(), shader);
        }

        private static GraphicsPipelineCreateInfo GetStandardGraphicsPipelineCreateInfo(
            TextureFormat swapchainFormat,
            Shader vertShader,
            Shader fragShader)
        {
            return new GraphicsPipelineCreateInfo
            {
                TargetInfo = new GraphicsPipelineTargetInfo
                {
                    ColorTargetDescriptions =
                    [
                        new ColorTargetDescription
                        {
                            Format = swapchainFormat,
                            BlendState = ColorTargetBlendState.PremultipliedAlphaBlend
                        }
                    ],
                    DepthStencilFormat = TextureFormat.D32Float,
                    HasDepthStencilTarget = true
                },
                DepthStencilState = new DepthStencilState
                {
                    EnableDepthTest = true,
                    EnableDepthWrite = true,
                    CompareOp = CompareOp.LessOrEqual,
                },
                MultisampleState = new MultisampleState
                {
                    SampleCount = DEFAULT_SAMPLE_COUNT,
                },
                PrimitiveType = PrimitiveType.TriangleList,
                RasterizerState = RasterizerState.CCW_CullBack,
                VertexInputState = VertexInputState.CreateSingleBinding<VertexPositionNormalTexture>(),
                VertexShader = vertShader,
                FragmentShader = fragShader,
            };
        }
    }
}