using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Graphics;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class WorldRenderSystem : Renderer
    {
        private readonly Filter _meshFilter;
        private readonly Filter _markerFilter;
        private readonly Filter _blobShadowFilter;
        
        public WorldRenderSystem(World world) : base(world)
        {
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

        public void Render(CommandBuffer cmdBuf, Texture swapchainTexture, Texture renderTarget, Texture depthTexture, AssetStore assetStore)
        {
            if (_meshFilter.Empty) return;
            var mainCamera = GetSingletonEntity<MainRenderCamera>();
            var viewProjection = Get<CameraViewProjection>(mainCamera).ViewProjection;
            var lightTransform = Get<Transform>(GetSingletonEntity<DirectionalLight>());
            
            var renderPass = cmdBuf.BeginRenderPass(
                new DepthStencilTargetInfo(depthTexture, 1f, true),
                new ColorTargetInfo()
                {
                    Texture = renderTarget.Handle,
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
            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<StandardMaterial>());
            foreach (var entity in _meshFilter.Entities)
            {
                var material = Get<StandardMaterial>(entity);
                var meshConfig = Get<HasMesh>(entity);
                var transform = Get<Transform>(entity);
                var model = transform.Value;
                var vertexUniforms = new TransformVertexUniform(model * viewProjection, model);
                var mesh = assetStore.GetMesh(meshConfig.Mesh);
                var texture = assetStore.GetTexture(material.Texture);
                renderPass.BindVertexBuffers(mesh.VertexBuffer);
                renderPass.BindIndexBuffer(mesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                renderPass.BindFragmentSamplers(new TextureSamplerBinding(texture, assetStore.GetSampler(SamplerType.LinearWrap)));
                cmdBuf.PushVertexUniformData(vertexUniforms);
                renderPass.DrawIndexedPrimitives(mesh.IndexCount, 1, 0, 0, 0);
            }

            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<GridMarkerMaterial>());
            foreach (var entity in _markerFilter.Entities)
            {
                var markerTransform = World.Get<Transform>(entity);
                var markerModel = markerTransform.Value;
                var markerVertexUniforms = new TransformVertexUniform(markerModel * viewProjection, markerModel);
                var markerMesh = assetStore.GetMesh("Quad.Plane");
                renderPass.BindVertexBuffers(markerMesh.VertexBuffer);
                renderPass.BindIndexBuffer(markerMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(markerVertexUniforms);
                renderPass.DrawIndexedPrimitives(markerMesh.IndexCount, 1, 0, 0, 0);
            }
            
            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<BlobShadowMaterial>());
            foreach (var entity in _blobShadowFilter.Entities)
            {
                var blobShadowTransform = World.Get<Transform>(entity);
                var blowShadow = World.Get<HasBlobShadow>(entity);
                var blobShadowModel = Matrix4x4.CreateScale(blowShadow.Radius) *
                                      Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                      Matrix4x4.CreateTranslation(blobShadowTransform.Position with { Y = 0.01f });
                var blobShadowVertexUniforms =
                    new TransformVertexUniform(blobShadowModel * viewProjection, blobShadowModel);
                var blobShadowMesh = assetStore.GetMesh("BlobShadow.Plane");
                renderPass.BindVertexBuffers(blobShadowMesh.VertexBuffer);
                renderPass.BindIndexBuffer(blobShadowMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(blobShadowVertexUniforms);
                renderPass.DrawIndexedPrimitives(blobShadowMesh.IndexCount, 1, 0, 0, 0);
            }

            cmdBuf.EndRenderPass(renderPass);
        }
    }
}