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
                var charMovement = World.Get<CharacterMovement>(entity);
                ref var transform = ref World.Get<LocalTransform>(entity);

                var movement = charMovement.Movement == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(charMovement.Movement);
                var facing = charMovement.Facing == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(charMovement.Facing);
                
                transform.Position += new Vector3(movement.X, 0, movement.Y) * 2 * (float) delta.TotalSeconds;
                var angle = (float)Math.Atan2(-charMovement.Facing.X, -charMovement.Facing.Y);
                var rotation = Quaternion.CreateFromYawPitchRoll(angle, 0, 0);
                transform.Rotation = Quaternion.Slerp(transform.Rotation, rotation, 0.25f);
            }
        }
    }
}