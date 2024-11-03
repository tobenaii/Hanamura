using MoonTools.ECS;

namespace Hanamura
{
    public struct GridMarkerEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new GridMarkerData());
            world.Set(entity, new Transform());
        }
    }
}