using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class MainRenderSystem : Renderer
    {
        private readonly StandardMaterial _standardMaterial;
        private readonly GridMarkerMaterial _gridMarkerMaterial;
        private readonly BlobShadowMaterial _blobShadowMaterial;
        private readonly Filter _meshFilter;
        private readonly Filter _markerFilter;
        private readonly Filter _blobShadowFilter;
        private readonly AssetStore _assetStore;
        
        public MainRenderSystem(World world, Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) : base(world)
        {
            _assetStore = assetStore;
            _standardMaterial = new StandardMaterial(window, graphicsDevice, assetStore);
            _gridMarkerMaterial = new GridMarkerMaterial(window, graphicsDevice, assetStore);
            _blobShadowMaterial = new BlobShadowMaterial(window, graphicsDevice, assetStore);
            _meshFilter = FilterBuilder
                .Include<StandardMaterialData>()
                .Include<Transform>()
                .Build();
            _markerFilter = FilterBuilder
                .Include<GridMarkerData>()
                .Include<Transform>()
                .Build();
            _blobShadowFilter = FilterBuilder
                .Include<HasblobShadow>()
                .Include<Transform>()
                .Build();
        }

        public void Render(double alpha, CommandBuffer cmdBuf, Texture swapchainTexture, Texture renderTarget, Texture depthTexture)
        {
            if (_meshFilter.Empty) return;
            var viewProjection = GetSingleton<MainCamera>().ViewProjection;
            var lightTransform = Get<Transform>(GetSingletonEntity<DirectionalLight>());
            
            var renderPass = cmdBuf.BeginRenderPass(
                new ColorTargetInfo()
                {
                    Texture = renderTarget.Handle,
                    LoadOp = LoadOp.Clear,
                    ClearColor = Color.Black,
                    Cycle = true,
                    CycleResolveTexture = true,
                    ResolveTexture = swapchainTexture.Handle,
                    StoreOp = StoreOp.Resolve
                },
                new DepthStencilTargetInfo(depthTexture, 1f, true)
            );
            var fragmentUniforms = new LightingFragmentUniform(lightTransform.Forward);
            cmdBuf.PushFragmentUniformData(fragmentUniforms);
            renderPass.BindGraphicsPipeline(_standardMaterial.GraphicsPipeline);
            foreach (var entity in _meshFilter.Entities)
            {
                var renderData = Get<StandardMaterialData>(entity);
                var transform = Get<Transform>(entity);
                var model = Matrix4x4.CreateScale(transform.Scale) *
                            Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                            Matrix4x4.CreateTranslation(transform.Position);
                var vertexUniforms = new TransformVertexUniform(model * viewProjection, model);
                var mesh = _assetStore.GetMesh(renderData.MeshId);
                var texture = _assetStore.GetTexture(renderData.TextureId);
                renderPass.BindVertexBuffer(mesh.VertexBuffer);
                renderPass.BindIndexBuffer(mesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                renderPass.BindFragmentSampler(new TextureSamplerBinding(texture, _standardMaterial.Sampler));
                cmdBuf.PushVertexUniformData(vertexUniforms);
                renderPass.DrawIndexedPrimitives(mesh.IndexCount, 1, 0, 0, 0);
            }

            renderPass.BindGraphicsPipeline(_gridMarkerMaterial.GraphicsPipeline);
            foreach (var entity in _markerFilter.Entities)
            {
                var markerTransform = World.Get<Transform>(entity);
                var markerModel = Matrix4x4.CreateScale(markerTransform.Scale) *
                                  Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                  Matrix4x4.CreateTranslation(markerTransform.Position);
                var markerVertexUniforms = new TransformVertexUniform(markerModel * viewProjection, markerModel);
                var markerMesh = _assetStore.GetMesh("Quad".GetHashCode());
                renderPass.BindVertexBuffer(markerMesh.VertexBuffer);
                renderPass.BindIndexBuffer(markerMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(markerVertexUniforms);
                renderPass.DrawIndexedPrimitives(markerMesh.IndexCount, 1, 0, 0, 0);
            }
            
            renderPass.BindGraphicsPipeline(_blobShadowMaterial.GraphicsPipeline);
            foreach (var entity in _blobShadowFilter.Entities)
            {
                var blobShadowTransform = World.Get<Transform>(entity);
                var blowShadow = World.Get<HasblobShadow>(entity);
                var blobShadowModel = Matrix4x4.CreateScale(blowShadow.Radius) *
                                      Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                      Matrix4x4.CreateTranslation(blobShadowTransform.Position with { Y = 0.01f });
                var blobShadowVertexUniforms =
                    new TransformVertexUniform(blobShadowModel * viewProjection, blobShadowModel);
                var blobShadowMesh = _assetStore.GetMesh("BlobShadow".GetHashCode());
                renderPass.BindVertexBuffer(blobShadowMesh.VertexBuffer);
                renderPass.BindIndexBuffer(blobShadowMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(blobShadowVertexUniforms);
                renderPass.DrawIndexedPrimitives(blobShadowMesh.IndexCount, 1, 0, 0, 0);
            }

            cmdBuf.EndRenderPass(renderPass);
        }
    }
}