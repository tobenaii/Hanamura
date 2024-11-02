using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public class CameraFollowSystem : MoonTools.ECS.System
    {
        public CameraFollowSystem(World world) : base(world)
        {
        }

        public override void Update(TimeSpan delta)
        {
            var camera = World.GetSingletonEntity<MainCamera>();
            var target = World.OutRelationSingleton<CameraTarget>(camera);
            
            var targetTransform = World.Get<Transform>(target);
            ref var cameraTransform = ref World.Get<Transform>(camera);
            var followConfig = World.Get<CameraFollowConfig>(camera);

            cameraTransform.Position = targetTransform.Position;
            cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -followConfig.Angle);
            cameraTransform.Position -= cameraTransform.Forward * followConfig.Distance;
        }
    }
}