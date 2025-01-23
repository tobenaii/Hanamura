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

        public RenderSystem(World world, Window window, GraphicsDevice graphicsDevice) :
            base(world)
        {
            _transformFilter = FilterBuilder
                .Include<Transform>()
                .Include<TransformState>()
                .Build();
            
            _worldRenderSystem = new WorldRenderSystem(world, window, graphicsDevice);
            _uiRenderSystem = new UIRenderSystem(world, window, graphicsDevice);
        }

        public void Draw(double alpha, Window window, GraphicsDevice graphicsDevice)
        {
            /*foreach (var entity in _transformFilter.Entities)
            {
                var transformState = World.Get<TransformState>(entity);
                ref var transform = ref World.Get<Transform>(entity);
                if (!World.Has<HasFixedTransform>(entity))
                {
                    var prevScale = transformState.Previous.Scale;
                    var prevRotation = transformState.Previous.Rotation;
                    var prevTranslation = transformState.Previous.Position;
                    
                    var currentScale = transformState.Current.Scale;
                    var currentRotation = transformState.Current.Rotation;
                    var currentTranslation = transformState.Current.Position;
                    
                    var interpolatedScale = Vector3.Lerp(prevScale, currentScale, (float)alpha);
                    var interpolatedPosition = Vector3.Lerp(prevTranslation, currentTranslation, (float)alpha);
                    var interpolatedRotation = Quaternion.Slerp(prevRotation, currentRotation, (float)alpha);


                    transform = new Transform(
                        Matrix4x4.CreateScale(interpolatedScale) *
                        Matrix4x4.CreateFromQuaternion(interpolatedRotation) *
                        Matrix4x4.CreateTranslation(interpolatedPosition));
                }
                else
                {
                    transform = transformState.Current;
                }
            }*/

            var mainCameraEntity = World.GetSingletonEntity<MainRenderCamera>();
            
            var cameraConfig = World.Get<CameraSettings>(mainCameraEntity);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                cameraConfig.Fov,
                (float)window.Width / window.Height,
                cameraConfig.Near,
                cameraConfig.Far
            );
            
            var cameraTransform = Get<Transform>(mainCameraEntity);
            var cameraPosition = cameraTransform.Position;
            var cameraDirection = cameraTransform.Forward;
            var upDirection = Vector3.UnitY;
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            
            ref var viewProjection = ref World.Get<CameraViewProjection>(mainCameraEntity);
            viewProjection = new CameraViewProjection(view * projection);

            var cmdBuf = graphicsDevice.AcquireCommandBuffer();
            var swapchainTexture = cmdBuf.AcquireSwapchainTexture(window);
            if (swapchainTexture != null)
            {
                _worldRenderSystem.Render(cmdBuf, swapchainTexture);
                _uiRenderSystem.Render(cmdBuf, swapchainTexture);
            }

            graphicsDevice.Submit(cmdBuf);
        }
    }
}