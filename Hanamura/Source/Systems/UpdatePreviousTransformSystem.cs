using MoonTools.ECS;

namespace Hanamura
{
    public class UpdatePreviousTransformSystem : MoonTools.ECS.System
    {
        private readonly Filter _withoutPreviousTransformFilter;
        private readonly Filter _transformFilter;
        
        public UpdatePreviousTransformSystem(World world) : base(world)
        {
            _transformFilter = FilterBuilder
                .Include<Transform>()
                .Build();
            _withoutPreviousTransformFilter = FilterBuilder
                .Include<Transform>()
                .Exclude<PreviousTransform>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _withoutPreviousTransformFilter.Entities)
            {
                World.Set(entity, new PreviousTransform());
            }
            
            foreach (var entity in _transformFilter.Entities)
            {
                var transform = Get<Transform>(entity);
                ref var previousTransform = ref Get<PreviousTransform>(entity);
                previousTransform.Transform = transform;
            }
        }
    }
}