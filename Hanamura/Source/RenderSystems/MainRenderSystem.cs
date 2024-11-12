using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class MainRenderSystem : Renderer
    {
        private readonly Filter _meshFilter;
        private readonly Filter _markerFilter;
        private readonly Filter _blobShadowFilter;
        
        public MainRenderSystem(World world) : base(world)
        {
            _meshFilter = FilterBuilder
                .Include<StandardMaterialConfig>()
                .Include<Transform>()
                .Build();
            _markerFilter = FilterBuilder
                .Include<GridMarkerTag>()
                .Include<Transform>()
                .Build();
            _blobShadowFilter = FilterBuilder
                .Include<BlobShadowConfig>()
                .Include<Transform>()
                .Build();
        }

        public void Render(double alpha, CommandBuffer cmdBuf, Texture swapchainTexture, Texture renderTarget, Texture depthTexture, AssetStore assetStore)
        {
            if (_meshFilter.Empty) return;
            var mainCamera = GetSingletonEntity<MainCameraTag>();
            var viewProjection = Get<CameraViewProjection>(mainCamera).ViewProjection;
            var lightTransform = Get<Transform>(GetSingletonEntity<DirectionalLightTag>());
            
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
            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<StandardMaterial>());
            foreach (var entity in _meshFilter.Entities)
            {
                var renderData = Get<StandardMaterialConfig>(entity);
                var transform = Get<Transform>(entity);
                var model = Matrix4x4.CreateScale(transform.Scale) *
                            Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                            Matrix4x4.CreateTranslation(transform.Position);
                var vertexUniforms = new TransformVertexUniform(model * viewProjection, model);
                var mesh = assetStore.GetMesh(renderData.Mesh);
                var texture = assetStore.GetTexture(renderData.Texture);
                renderPass.BindVertexBuffer(mesh.VertexBuffer);
                renderPass.BindIndexBuffer(mesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                renderPass.BindFragmentSampler(new TextureSamplerBinding(texture, assetStore.GetSampler(SamplerType.LinearWrap)));
                cmdBuf.PushVertexUniformData(vertexUniforms);
                renderPass.DrawIndexedPrimitives(mesh.IndexCount, 1, 0, 0, 0);
            }

            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<GridMarkerMaterial>());
            foreach (var entity in _markerFilter.Entities)
            {
                var markerTransform = World.Get<Transform>(entity);
                var markerModel = Matrix4x4.CreateScale(markerTransform.Scale) *
                                  Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                  Matrix4x4.CreateTranslation(markerTransform.Position);
                var markerVertexUniforms = new TransformVertexUniform(markerModel * viewProjection, markerModel);
                var markerMesh = assetStore.GetMesh("Quad");
                renderPass.BindVertexBuffer(markerMesh.VertexBuffer);
                renderPass.BindIndexBuffer(markerMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(markerVertexUniforms);
                renderPass.DrawIndexedPrimitives(markerMesh.IndexCount, 1, 0, 0, 0);
            }
            
            renderPass.BindGraphicsPipeline(assetStore.GetMaterial<BlobShadowMaterial>());
            foreach (var entity in _blobShadowFilter.Entities)
            {
                var blobShadowTransform = World.Get<Transform>(entity);
                var blowShadow = World.Get<BlobShadowConfig>(entity);
                var blobShadowModel = Matrix4x4.CreateScale(blowShadow.Radius) *
                                      Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                      Matrix4x4.CreateTranslation(blobShadowTransform.Position with { Y = 0.01f });
                var blobShadowVertexUniforms =
                    new TransformVertexUniform(blobShadowModel * viewProjection, blobShadowModel);
                var blobShadowMesh = assetStore.GetMesh("BlobShadow");
                renderPass.BindVertexBuffer(blobShadowMesh.VertexBuffer);
                renderPass.BindIndexBuffer(blobShadowMesh.IndexBuffer, IndexElementSize.ThirtyTwo);
                cmdBuf.PushVertexUniformData(blobShadowVertexUniforms);
                renderPass.DrawIndexedPrimitives(blobShadowMesh.IndexCount, 1, 0, 0, 0);
            }

            cmdBuf.EndRenderPass(renderPass);
        }
    }
}