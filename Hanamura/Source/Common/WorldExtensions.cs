using MoonTools.ECS;

namespace Hanamura
{
    public static class WorldExtensions
    {
        public static EntityBuilder Spawn<T>(this World world) where T : unmanaged, IGameEntity
        {
            var entity = world.CreateEntity();
            world.Set(entity, default(T));
            T.Configure(world, entity);
            T.Init(world, entity);
            return new EntityBuilder(world, entity);
        }
    }
}