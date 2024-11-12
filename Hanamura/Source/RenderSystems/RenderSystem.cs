using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using Filter = MoonTools.ECS.Filter;

namespace Hanamura
{
    public class RenderSystem : Renderer
    {
        private readonly WorldRenderSystem _worldRenderSystem;
        private readonly UIRenderSystem _uiRenderSystem;
        private readonly Filter _transformFilter;
        private readonly Filter _withoutRenderTransformFilter;

        public RenderSystem(World world) :
            base(world)
        {
            _transformFilter = FilterBuilder
                .Include<Transform>()
                .Include<PreviousTransform>()
                .Include<RenderTransform>()
                .Build();
            
            _withoutRenderTransformFilter = FilterBuilder
                .Include<Transform>()
                .Exclude<RenderTransform>()
                .Build();
            _worldRenderSystem = new WorldRenderSystem(world);
            _uiRenderSystem = new UIRenderSystem(world);
        }

        public void Draw(double alpha, Window window, GraphicsDevice graphicsDevice, AssetStore assetStore)
        {
            foreach (var entity in _withoutRenderTransformFilter.Entities)
            {
                World.Set(entity, new RenderTransform());
            }
            
            foreach (var entity in _transformFilter.Entities)
            {
                var previousTransform = World.Get<PreviousTransform>(entity);
                var currentTransform = World.Get<Transform>(entity);
                ref var renderTransform = ref World.Get<RenderTransform>(entity);
                if (!World.Has<FixedRenderTag>(entity))
                {
                    renderTransform.Transform = new Transform(
                        Position: Vector3.Lerp(previousTransform.Transform.Position, currentTransform.Position,
                            (float)alpha),
                        Rotation: Quaternion.Slerp(previousTransform.Transform.Rotation, currentTransform.Rotation,
                            (float)alpha),
                        Scale: Vector3.Lerp(previousTransform.Transform.Scale, currentTransform.Scale, (float)alpha)
                    );
                }
                else
                {
                    renderTransform.Transform = currentTransform;
                }
            }
            
            var mainCameraEntity = World.GetSingletonEntity<MainCameraTag>();
            
            var cameraConfig = World.Get<CameraConfig>(mainCameraEntity);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                cameraConfig.Fov,
                (float)window.Width / window.Height,
                cameraConfig.Near,
                cameraConfig.Far
            );
            
            var cameraTransform = Get<RenderTransform>(mainCameraEntity).Transform;
            var cameraPosition = cameraTransform.Position;
            var cameraDirection = Vector3.Transform(new Vector3(0, 0, -1), cameraTransform.Rotation);
            var upDirection = Vector3.UnitY;
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            
            ref var viewProjection = ref World.Get<CameraViewProjection>(mainCameraEntity);
            viewProjection = new CameraViewProjection(view * projection);

            var cmdBuf = graphicsDevice.AcquireCommandBuffer();
            var swapchainTexture = cmdBuf.AcquireSwapchainTexture(window);
            if (swapchainTexture != null)
            {
                _worldRenderSystem.Render(cmdBuf, swapchainTexture, assetStore.RenderTarget, assetStore.DepthTexture,
                    assetStore);
                _uiRenderSystem.Render(cmdBuf, swapchainTexture, assetStore);
            }

            graphicsDevice.Submit(cmdBuf);
        }
    }
}