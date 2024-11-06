using System.Numerics;
using MoonTools.ECS;
using MoonWorks;

namespace Hanamura
{
    public class CameraUpdateSystem : MoonTools.ECS.System
    {
        private Matrix4x4 _projection;
        
        public CameraUpdateSystem(World world, Window window) : base(world)
        {
            _projection= Matrix4x4.CreatePerspectiveFieldOfView(
                float.DegreesToRadians(90),
                (float)window.Width / window.Height,
                0.01f,
                100f
            );
        }

        public override void Update(TimeSpan delta)
        {
            var mainCameraEntity = World.GetSingletonEntity<MainCamera>();
            var cameraTransform = Get<Transform>(mainCameraEntity);
            var cameraPosition = cameraTransform.Position;
            var cameraDirection = Vector3.Transform(new Vector3(0, 0, -1), cameraTransform.Rotation);
            var upDirection = Vector3.UnitY;
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            
            ref var mainCamera = ref World.Get<MainCamera>(mainCameraEntity);
            mainCamera = new MainCamera(view * _projection);
        }
    }
}