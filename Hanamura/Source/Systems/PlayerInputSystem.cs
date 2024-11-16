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
            
            ref var movement = ref World.Get<CharacterMovement>(movementTarget);
            var orbit = World.GetRelationData<OrbitsTarget>(orbitTarget, movementTarget);
            
            ref var relativeTransform = ref World.Get<LocalTransform>(orbitTarget);
            var forward = relativeTransform.Forward;
            forward.Y = 0;
            forward = Vector3.Normalize(forward);
            var right = -Vector3.Cross(Vector3.UnitY, forward);

            var direction = Vector3.Zero;
            if (_inputs.Keyboard.IsDown(KeyCode.W))
            {
                direction += forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(KeyCode.S))
            {
                direction -= forward * moveSpeed * (float) delta.TotalSeconds;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.A))
            {
                direction -= right * moveSpeed * (float) delta.TotalSeconds;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.D))
            {
                direction += right * moveSpeed * (float) delta.TotalSeconds;
            }
            
            var leftStick = new Vector2(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftX), _inputs.GetGamepad(0).AxisValue(AxisCode.LeftY));
            var length = leftStick.Length();
            if (length > 0.3f)
            {
                direction += right * moveSpeed * _inputs.GetGamepad(0).AxisValue(AxisCode.LeftX) * (float) delta.TotalSeconds;
            }
            if (length > 0.3f)
            {
                direction += forward * moveSpeed * -_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY) * (float) delta.TotalSeconds;
            }
            
            movement.Movement = new Vector2(direction.X, direction.Z);
            
            if (direction != Vector3.Zero)
            {
                movement.Facing = new Vector2(direction.X, direction.Z);
            }
            
            orbit.Yaw -= _inputs.Mouse.DeltaX * 0.001f;
            
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightX)) > 0.1f)
            {
                orbit.Yaw -= _inputs.GetGamepad(0).AxisValue(AxisCode.RightX) * yawSpeed;
            }
            
            World.Relate(orbitTarget, movementTarget, orbit);
        }
    }
}