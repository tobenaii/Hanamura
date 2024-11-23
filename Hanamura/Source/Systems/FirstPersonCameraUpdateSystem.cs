using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public class FirstPersonCameraUpdateSystem : MoonTools.ECS.System
    {
        private readonly Filter _filter;
        
        public FirstPersonCameraUpdateSystem(World world) : base(world)
        {
            _filter = FilterBuilder
                .Include<LocalTransform>()
                .Include<FirstPersonCamera>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _filter.Entities)
            {
                ref var transform = ref World.Get<LocalTransform>(entity);
                var character = World.OutRelationSingleton<AttachedToCharacter>(entity);
                var characterViewRotation = World.Get<CharacterViewRotation>(character);
                transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -characterViewRotation.Pitch);
            }
        }
    }
}