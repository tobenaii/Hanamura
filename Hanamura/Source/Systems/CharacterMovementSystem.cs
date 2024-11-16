using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public class CharacterMovementSystem : MoonTools.ECS.System
    {
        private readonly Filter _filter;
        
        public CharacterMovementSystem(World world) : base(world)
        {
            _filter = FilterBuilder
                .Include<CharacterMovement>()
                .Include<LocalTransform>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _filter.Entities)
            {
                ref var movement = ref World.Get<CharacterMovement>(entity);
                ref var transform = ref World.Get<LocalTransform>(entity);

                transform.Position += new Vector3(movement.Movement.X, 0, movement.Movement.Y) * 50 * (float) delta.TotalSeconds;
                var angle = (float)Math.Atan2(-movement.Facing.X, -movement.Facing.Y);
                var rotation = Quaternion.CreateFromYawPitchRoll(angle, 0, 0);
                transform.Rotation = Quaternion.Slerp(transform.Rotation, rotation, 0.25f);
            }
        }
    }
}