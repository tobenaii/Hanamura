using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class OrbitFollowSystem : MoonTools.ECS.System
    {
        private readonly Inputs _inputs;
        
        public OrbitFollowSystem(World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
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
                
                var minPitch = orbit.MinPitch;
                var maxPitch = orbit.MaxPitch;
            
                orbit.Pitch = Math.Clamp(orbit.Pitch, minPitch, maxPitch);
                transform.Rotation = Quaternion.CreateFromYawPitchRoll(orbit.Yaw, -orbit.Pitch, 0);
                transform.Position = targetPosition;
            
                var distance = float.Lerp(orbit.MinDistance, orbit.MaxDistance, (orbit.Pitch - minPitch) / (maxPitch - minPitch));
                transform.Position -= transform.Forward * distance;
                
                World.Relate(entity, target, orbit);
            }
        }
    }
}