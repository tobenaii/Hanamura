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
                direction += forward;
            }

            if (_inputs.Keyboard.IsDown(KeyCode.S))
            {
                direction -= forward;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.A))
            {
                direction -= right;
            }
            
            if (_inputs.Keyboard.IsDown(KeyCode.D))
            {
                direction += right;
            }
            
            var leftStick = new Vector2(_inputs.GetGamepad(0).AxisValue(AxisCode.LeftX), _inputs.GetGamepad(0).AxisValue(AxisCode.LeftY));
            var length = leftStick.Length();
            if (length > 0.3f)
            {
                direction += right * _inputs.GetGamepad(0).AxisValue(AxisCode.LeftX);
            }
            if (length > 0.3f)
            {
                direction += forward * -_inputs.GetGamepad(0).AxisValue(AxisCode.LeftY);
            }
            
            movement.Movement = new Vector2(direction.X, direction.Z);
            
            if (direction != Vector3.Zero)
            {
                movement.Facing = new Vector2(direction.X, direction.Z);
            }
            
            orbit.Yaw -= _inputs.Mouse.DeltaX * 0.001f;
            
            if (Math.Abs(_inputs.GetGamepad(0).AxisValue(AxisCode.RightX)) > 0.1f)
            {
                orbit.Yaw -= _inputs.GetGamepad(0).AxisValue(AxisCode.RightX);
            }
            
            World.Relate(orbitTarget, movementTarget, orbit);
        }
    }
}