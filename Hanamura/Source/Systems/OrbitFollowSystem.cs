using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class OrbitFollowSystem : MoonTools.ECS.System
    {
        public OrbitFollowSystem(World world) : base(world)
        {
        }

        public override void Update(TimeSpan delta)
        {
            var orbitFollows = World.Relations<OrbitsTarget>();
            
            foreach (var (entity, target) in orbitFollows)
            {
                var orbit = World.GetRelationData<OrbitsTarget>(entity, target);
                
                var targetPosition = World.Get<LocalTransform>(target).Position;
                targetPosition += orbit.Offset;
                
                ref var transform = ref World.Get<LocalTransform>(entity);
            
                transform.Rotation = Quaternion.CreateFromYawPitchRoll(orbit.Yaw, -orbit.Pitch, 0);
                transform.Position = targetPosition;
            
                transform.Position -= transform.Forward * orbit.Distance;
                
                World.Relate(entity, target, orbit);
            }
        }
    }
}