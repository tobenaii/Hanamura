using MoonTools.ECS;

namespace Hanamura
{
    public class UpdateTransformStateSystem : MoonTools.ECS.System
    {
        private readonly Filter _noTransformStateFilter;
        private readonly Filter _transformStateFilter;
        
        public UpdateTransformStateSystem(World world) : base(world)
        {
            _noTransformStateFilter = FilterBuilder
                .Include<Transform>()
                .Exclude<TransformState>()
                .Build();
            
            _transformStateFilter = FilterBuilder
                .Include<Transform>()
                .Include<TransformState>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _noTransformStateFilter.Entities)
            {
                World.Set(entity, new TransformState());
            }
            
            foreach (var entity in _transformStateFilter.Entities)
            {
                ref var transform = ref Get<Transform>(entity);
                ref var transformState = ref Get<TransformState>(entity);
                transformState.Previous = transform;
                transform.Value = transformState.Current.Value;
            }
        }
    }
}