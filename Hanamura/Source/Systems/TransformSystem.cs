using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public class TransformSystem : MoonTools.ECS.System
    {
        private Filter _transformStateFilter;
        private Filter _noTransform;
        private Filter _depth0;
        private Filter _depth1;
        private Filter _depth2;
        private Filter _depth3;
        
        public TransformSystem(World world) : base(world)
        {
            _transformStateFilter = FilterBuilder
                .Include<Transform>()
                .Include<TransformState>()
                .Build();
            _noTransform = FilterBuilder
                .Include<LocalTransform>()
                .Exclude<Transform>()
                .Build();
            _depth0 = FilterBuilder
                .Include<Transform>()
                .Include<LocalTransform>()
                .Exclude<Parent>()
                .Build();
            _depth1 = FilterBuilder
                .Include<Transform>()
                .Include<LocalTransform>()
                .Include<ChildDepth1>()
                .Include<Parent>()
                .Build();
            
            _depth2 = FilterBuilder
                .Include<Transform>()
                .Include<LocalTransform>()
                .Include<ChildDepth2>()
                .Include<Parent>()
                .Build();
            
            _depth3 = FilterBuilder
                .Include<Transform>()
                .Include<LocalTransform>()
                .Include<ChildDepth3>()
                .Include<Parent>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _noTransform.Entities)
            {
                World.Set(entity, new Transform());
            }
            
            foreach (var entity in _depth0.Entities)
            {
                ref var transform = ref Get<Transform>(entity);
                var localTransform = Get<LocalTransform>(entity);
                var model = Matrix4x4.CreateScale(localTransform.Scale) *
                            Matrix4x4.CreateFromQuaternion(localTransform.Rotation) *
                            Matrix4x4.CreateTranslation(localTransform.Position);
                transform.Value = model;
            }
            UpdateTransforms(_depth1);
            UpdateTransforms(_depth2);
            UpdateTransforms(_depth3);
            
            foreach (var entity in _transformStateFilter.Entities)
            {
                var transform = Get<Transform>(entity);
                ref var transformState = ref Get<TransformState>(entity);
                transformState.Current = transform;
            }
        }

        private void UpdateTransforms(Filter filter)
        {
            foreach (var entity in filter.Entities)
            {
                ref var transform = ref Get<Transform>(entity);
                var localTransform = Get<LocalTransform>(entity);
                var parent = Get<Parent>(entity);
                var parentTransform = Get<Transform>(parent.Entity);
                var model = Matrix4x4.CreateScale(localTransform.Scale) *
                            Matrix4x4.CreateFromQuaternion(localTransform.Rotation) *
                            Matrix4x4.CreateTranslation(localTransform.Position);
                model *= parentTransform.Value;
                transform.Value = model;
            }
        }
    }
}