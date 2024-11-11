using MoonTools.ECS;

namespace Hanamura
{
    public struct GridMarkerEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
        }

        public static void Init(World world, Entity entity)
        {
            world.Set(entity, new GridMarkerTag());
            world.Set(entity, new Transform());
        }
    }
}