using MoonTools.ECS;

namespace Hanamura
{
    public static class WorldExtensions
    {
        public static EntityBuilder Spawn<T>(this World world) where T : IGameEntity
        {
            var entity = world.CreateEntity();
            T.Build(world, entity);
            return new EntityBuilder(world, entity);
        }
    }
}