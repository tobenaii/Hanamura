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
            var controller = World.GetSingletonEntity<PlayerController>();
            var target = World.OutRelationSingleton<PlayerControllerTarget>(controller);
            var input = World.Get<PlayerController>(controller);
            const float moveSpeed = 2.5f;
            
            var transform = World.Get<Transform>(target);

            if (_inputs.Keyboard.IsDown(input.MoveForward))
            {
                transform.Position += transform.Forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveBackward))
            {
                transform.Position -= transform.Forward * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveLeft))
            {
                transform.Position -= transform.Right * moveSpeed * (float) delta.TotalSeconds;
            }

            if (_inputs.Keyboard.IsDown(input.MoveRight))
            {
                transform.Position += transform.Right * moveSpeed * (float) delta.TotalSeconds;
            }

            World.Set(target, transform);
        }
    }
}