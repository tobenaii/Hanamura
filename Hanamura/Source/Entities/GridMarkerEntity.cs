using MoonTools.ECS;

namespace Hanamura
{
    public struct GridMarkerEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new GridMarkerTag());
            world.Set(entity, new LocalTransform());
            world.Set(entity, new FixedRenderTag());
        }
    }
}