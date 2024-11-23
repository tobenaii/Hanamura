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
            var movementTarget = World.OutRelationSingleton<ControlsCharacter>(playerInput);
            
            ref var characterControls = ref World.Get<CharacterControls>(movementTarget);
            var forward = new Vector3(0, 0, -1);
            var right = new Vector3(1, 0, 0);

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
            
            characterControls.Move = new Vector2(direction.X, direction.Z);
            
            var mouseX = _inputs.Mouse.DeltaX;
            var mouseY = _inputs.Mouse.DeltaY;
            var gamepadRightX = _inputs.GetGamepad(0).AxisValue(AxisCode.RightX);
            var gamepadRightY = _inputs.GetGamepad(0).AxisValue(AxisCode.RightY);
            if (mouseX == 0 && mouseY == 0)
            {
                //gamepad controls with deadzone
                if (Vector2.DistanceSquared(new Vector2(gamepadRightX, gamepadRightY), Vector2.Zero) > 0.025f)
                {
                    characterControls.LookYawPitch = new Vector2(gamepadRightX, gamepadRightY) * 0.025f;
                }
                else
                {
                    characterControls.LookYawPitch = Vector2.Zero;
                }
            }
            else
            {
                characterControls.LookYawPitch = new Vector2(mouseX, mouseY) * 0.001f;
            }
        }
    }
}