using System.Numerics;
using MoonTools.ECS;
using MoonWorks;

namespace Hanamura
{
    public class CameraProjectionSystem : MoonTools.ECS.System
    {

        public CameraProjectionSystem(World world) : base(world)
        {
        }

        public override void Update(TimeSpan delta)
        {
            var mainCameraEntity = World.GetSingletonEntity<MainCameraTag>();
            var mainWindowEntity = World.GetSingletonEntity<MainWindowTag>();
            var window = World.Get<Rect>(mainWindowEntity);
            
            var cameraConfig = World.Get<CameraConfig>(mainCameraEntity);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                cameraConfig.Fov,
                (float)window.Width / window.Height,
                cameraConfig.Near,
                cameraConfig.Far
            );
            
            var cameraTransform = Get<Transform>(mainCameraEntity);
            var cameraPosition = cameraTransform.Position;
            var cameraDirection = Vector3.Transform(new Vector3(0, 0, -1), cameraTransform.Rotation);
            var upDirection = Vector3.UnitY;
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            
            ref var viewProjection = ref World.Get<CameraViewProjection>(mainCameraEntity);
            viewProjection = new CameraViewProjection(view * projection);
        }
    }
}