using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class PlayerMovementSystem : MoonTools.ECS.System
    {
        private Inputs _inputs;
        
        public PlayerMovementSystem(World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
        }

        public override void Update(TimeSpan delta)
        {
            var mainCamera = World.GetSingletonEntity<MainCameraTag>();
            var controller = World.GetSingletonEntity<PlayerControllerConfig>();
            var target = World.OutRelationSingleton<PlayerControllerTargetTag>(controller);
            var cameraTransform = World.Get<LocalTransform>(mainCamera);
            var input = World.Get<PlayerControllerConfig>(controller);
            const float moveSpeed = 2.5f;
            
            var transform = World.Get<LocalTransform>(target);
            var forward = cameraTransform.Forward;
            forward.Y = 0;
            forward = Vector3.Normalize(forward);
            var right = -Vector3.Cross(Vector3.UnitY, forward);

            if (_inputs.Keyboard.IsDown(input.MoveForward))
            {
                transform.Position += forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveBackward))
            {
                transform.Position -= forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveLeft))
            {
                transform.Position -= right * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveRight))
            {
                transform.Position += right * moveSpeed * (float) delta.TotalSeconds;
            }
            
            //gamepad input
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftX)) > 0.1f)
            {
                transform.Position += right * moveSpeed * _inputs.GetGamepad(0).AxisValue(AxisCode.LeftX) * (float) delta.TotalSeconds;
            }
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY)) > 0.1f)
            {
                transform.Position += forward * moveSpeed * -_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY) * (float) delta.TotalSeconds;
            }

            World.Set(target, transform);
        }
    }
}