using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using Waddle;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class WorldRenderSystem : Renderer
    {
        private readonly Filter _meshFilter;
        private readonly Filter _markerFilter;
        private readonly Filter _blobShadowFilter;
        private readonly Sampler _linearWrapSampler;
        private readonly Texture _renderTexture;
        private readonly Texture _depthTexture;
        private readonly GraphicsPipeline _standardPipeline;
        private readonly GraphicsPipeline _gridMarkerPipeline;
        private readonly GraphicsPipeline _blobShadowPipeline;
        
        private const SampleCount DEFAULT_SAMPLE_COUNT = SampleCount.Eight;
        
        public WorldRenderSystem(World world, Window window, GraphicsDevice graphicsDevice) : base(world)
        {
            _linearWrapSampler = Sampler.Create(graphicsDevice, SamplerCreateInfo.LinearWrap);
            
            _renderTexture = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                window.SwapchainFormat,
                TextureUsageFlags.ColorTarget,
                1,
                DEFAULT_SAMPLE_COUNT
            );
            
            _depthTexture = Texture.Create2D(
                graphicsDevice,
                window.Width,
                window.Height,
                TextureFormat.D32Float,
                TextureUsageFlags.DepthStencilTarget | TextureUsageFlags.Sampler,
                1,
                DEFAULT_SAMPLE_COUNT
            );
            
            _standardPipeline = GraphicsPipeline.Create(graphicsDevice, 
                GetStandardGraphicsPipelineCreateInfo(window.SwapchainFormat, 
                    AssetStore.GetShader("StandardShader.vert"), 
                    AssetStore.GetShader("StandardShader.frag")));
            
            var gridMarkerInfo = GetStandardGraphicsPipelineCreateInfo(window.SwapchainFormat, AssetStore.GetShader("GridMarker.vert"), AssetStore.GetShader("GridMarker.frag"));
            gridMarkerInfo.DepthStencilState.EnableDepthTest = false;
            _gridMarkerPipeline = GraphicsPipeline.Create(graphicsDevice, gridMarkerInfo);
            
            var blobShadowInfo = GetStandardGraphicsPipelineCreateInfo(window.SwapchainFormat, AssetStore.GetShader("BlobShadow.vert"), AssetStore.GetShader("BlobShadow.frag"));
            blobShadowInfo.DepthStencilState.EnableDepthWrite = false;
            _blobShadowPipeline = GraphicsPipeline.Create(graphicsDevice, blobShadowInfo);
            
            _meshFilter = FilterBuilder
                .Include<StandardMaterial>()
                .Include<HasMesh>()
                .Include<Transform>()
                .Build();
            _markerFilter = FilterBuilder
                .Include<HasGridMarker>()
                .Include<Transform>()
                .Build();
            _blobShadowFilter = FilterBuilder
                .Include<HasBlobShadow>()
                .Include<Transform>()
                .Build();
        }

        public void Render(CommandBuffer cmdBuf, Texture swapchainTexture)
        {
            if (_meshFilter.Empty) return;
            var mainCamera = GetSingletonEntity<MainRenderCamera>();
            var viewProjection = Get<CameraViewProjection>(mainCamera).ViewProjection;
            var lightTransform = Get<Transform>(GetSingletonEntity<DirectionalLight>());
            
            var renderPass = cmdBuf.BeginRenderPass(
                new DepthStencilTargetInfo(_depthTexture, 1f, true),
                new ColorTargetInfo()
                {
                    Texture = _renderTexture.Handle,
                    LoadOp = LoadOp.Clear,
                    ClearColor = Color.Black,
                    Cycle = true,
                    CycleResolveTexture = true,
                    ResolveTexture = swapchainTexture.Handle,
                    StoreOp = StoreOp.Resolve
                }
            );
            var fragmentUniforms = new LightingFragmentUniform(lightTransform.Forward);
            cmdBuf.PushFragmentUniformData(fragmentUniforms);
            renderPass.BindGraphicsPipeline(_standardPipeline);
            foreach (var entity in _meshFilter.Entities)
            {
                var material = Get<StandardMaterial>(entity);
                var meshConfig = Get<HasMesh>(entity);
                var transform = Get<Transform>(entity);
                var model = transform.Value;
                var vertexUniforms = new TransformVertexUniform(model * viewProjection, model);
                var mesh = AssetStore.GetMesh(meshConfig.Mesh);
                var texture = AssetStore.GetTexture(material.Texture);
                renderPass.BindVertexBuffers(mesh.VertexBuffer);
                renderPass.BindIndexBuffer(mesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                renderPass.BindFragmentSamplers(new TextureSamplerBinding(texture, _linearWrapSampler));
                cmdBuf.PushVertexUniformData(vertexUniforms);
                renderPass.DrawIndexedPrimitives(mesh.IndexCount, 1, 0, 0, 0);
            }

            renderPass.BindGraphicsPipeline(_gridMarkerPipeline);
            foreach (var entity in _markerFilter.Entities)
            {
                var markerTransform = World.Get<Transform>(entity);
                var markerModel = markerTransform.Value;
                var markerVertexUniforms = new TransformVertexUniform(markerModel * viewProjection, markerModel);
                var markerMesh = AssetStore.GetMesh("Quad");
                renderPass.BindVertexBuffers(markerMesh.VertexBuffer);
                renderPass.BindIndexBuffer(markerMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(markerVertexUniforms);
                renderPass.DrawIndexedPrimitives(markerMesh.IndexCount, 1, 0, 0, 0);
            }
            
            renderPass.BindGraphicsPipeline(_blobShadowPipeline);
            foreach (var entity in _blobShadowFilter.Entities)
            {
                var blobShadowTransform = World.Get<Transform>(entity);
                var blowShadow = World.Get<HasBlobShadow>(entity);
                var blobShadowModel = Matrix4x4.CreateScale(blowShadow.Radius) *
                                      Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                      Matrix4x4.CreateTranslation(blobShadowTransform.Position with { Y = 0.01f });
                var blobShadowVertexUniforms =
                    new TransformVertexUniform(blobShadowModel * viewProjection, blobShadowModel);
                var blobShadowMesh = AssetStore.GetMesh("BlobShadow");
                renderPass.BindVertexBuffers(blobShadowMesh.VertexBuffer);
                renderPass.BindIndexBuffer(blobShadowMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(blobShadowVertexUniforms);
                renderPass.DrawIndexedPrimitives(blobShadowMesh.IndexCount, 1, 0, 0, 0);
            }

            cmdBuf.EndRenderPass(renderPass);
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