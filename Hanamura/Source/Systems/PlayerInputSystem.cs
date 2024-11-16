using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class PlayerInputSystem : MoonTools.ECS.System
    {
        private readonly Inputs _inputs;
        
        public PlayerInputSystem(World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
        }

        public override void Update(TimeSpan delta)
        {           
            const float moveSpeed = 2.5f;
            const float pitchSpeed = 0.025f;
            const float yawSpeed = 0.05f;
            
            var playerInput = World.GetSingletonEntity<PlayerInput>();
            var movementTarget = World.OutRelationSingleton<ControlsMovement>(playerInput);
            var orbitTarget = World.OutRelationSingleton<ControlsOrbit>(playerInput);
            
            ref var movementTransform = ref World.Get<LocalTransform>(movementTarget);
            var orbit = World.GetRelationData<OrbitsTarget>(orbitTarget, movementTarget);
            
            ref var transform = ref World.Get<LocalTransform>(movementTarget);
            ref var relativeTransform = ref World.Get<LocalTransform>(orbitTarget);
            var forward = relativeTransform.Forward;
            forward.Y = 0;
            forward = Vector3.Normalize(forward);
            var right = -Vector3.Cross(Vector3.UnitY, forward);

            
            if (_inputs.Keyboard.IsDown(KeyCode.W))
            {
                transform.Position += forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(KeyCode.S))
            {
                transform.Position -= forward * moveSpeed * (float) delta.TotalSeconds;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.A))
            {
                transform.Position -= right * moveSpeed * (float) delta.TotalSeconds;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.D))
            {
                transform.Position += right * moveSpeed * (float) delta.TotalSeconds;
            }
            
            orbit.Yaw -= _inputs.Mouse.DeltaX * 0.001f;
            orbit.Pitch += _inputs.Mouse.DeltaY * 0.001f;
            
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftX)) > 0.1f)
            {
                transform.Position += right * moveSpeed * _inputs.GetGamepad(0).AxisValue(AxisCode.LeftX) * (float) delta.TotalSeconds;
            }
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY)) > 0.1f)
            {
                transform.Position += forward * moveSpeed * -_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY) * (float) delta.TotalSeconds;
            }
            
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightX)) > 0.1f)
            {
                orbit.Yaw -= _inputs.GetGamepad(0).AxisValue(AxisCode.RightX) * yawSpeed;
            }

            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightY)) > 0.1f)
            {
                orbit.Pitch += _inputs.GetGamepad(0).AxisValue(AxisCode.RightY) * pitchSpeed;
            }
            
            World.Relate(orbitTarget, movementTarget, orbit);
        }
    }
}