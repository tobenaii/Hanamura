using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class CameraFollowSystem : MoonTools.ECS.System
    {
        private readonly Inputs _inputs;
        public CameraFollowSystem(World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
        }

        public override void Update(TimeSpan delta)
        {
            var camera = World.GetSingletonEntity<MainCameraTag>();
            var target = World.OutRelationSingleton<CameraTargetTag>(camera);
            
            var targetPosition = World.Get<LocalTransform>(target).Position;
            targetPosition.Y += World.Get<CameraFollowConfig>(camera).Height;
            ref var cameraTransform = ref World.Get<LocalTransform>(camera);
            var followConfig = World.Get<CameraFollowConfig>(camera);
            ref var followState = ref World.Get<CameraFollowState>(camera);
            followState.Yaw -= _inputs.Mouse.DeltaX * 0.001f;
            followState.Pitch += _inputs.Mouse.DeltaY * 0.001f;
            
            //do gamepad input here
            //check deadzone
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightX)) > 0.1f)
            {
                followState.Yaw -= _inputs.GetGamepad(0).AxisValue(AxisCode.RightX) * 0.05f;
            }

            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightY)) > 0.1f)
            {
                followState.Pitch += _inputs.GetGamepad(0).AxisValue(AxisCode.RightY) * 0.025f;
            }

            const float minPitch = 0.2f;
            const float maxPitch = 0.75f;
            //clamp pitch to not go under ground and over head
            followState.Pitch = Math.Clamp(followState.Pitch, minPitch, maxPitch);
            cameraTransform.Rotation = Quaternion.CreateFromYawPitchRoll(followState.Yaw, -followState.Pitch, 0);
            cameraTransform.Position = targetPosition;
            
            //lerp between min and max distance based on pitch
            var distance = float.Lerp(followConfig.MinDistance, followConfig.MaxDistance, (followState.Pitch - minPitch) / (maxPitch - minPitch));
            cameraTransform.Position -= cameraTransform.Forward * distance;
        }
    }
}