using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class StandardMaterialRenderSystem : Renderer
    {
        private readonly StandardMaterial _standardMaterial;
        private readonly Filter _meshFilter;
        private readonly AssetStore _assetStore;
        
        public StandardMaterialRenderSystem(World world, Window window, GraphicsDevice graphicsDevice, AssetStore assetStore) : base(world)
        {
            _assetStore = assetStore;
            _standardMaterial = new StandardMaterial(window, graphicsDevice, assetStore);
            _meshFilter = FilterBuilder
                .Include<StandardMaterialData>()
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
            var fragmentUniforms = new LightingFragmentUniform(lightTransform.Position);
            cmdBuf.PushFragmentUniformData(fragmentUniforms);
            renderPass.BindGraphicsPipeline(_standardMaterial.RenderPipeline);
            foreach (var entity in _meshFilter.Entities)
            {
                var renderData = Get<StandardMaterialData>(entity);
                var transform = Get<Transform>(entity);
                var model = Matrix4x4.CreateFromQuaternion(transform.Rotation) *
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
            cmdBuf.EndRenderPass(renderPass);
        }
    }
}