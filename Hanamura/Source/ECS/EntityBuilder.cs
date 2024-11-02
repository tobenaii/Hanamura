using MoonTools.ECS;

namespace Hanamura
{
    public readonly record struct EntityBuilder(World World, Entity Entity)
    {
        public EntityBuilder Set<T>(T component) where T : unmanaged
        {
            World.Set(Entity, component);
            return this;
        }
        
        public EntityBuilder Relate<T>(in Entity other, in T relation) where T : unmanaged
        {
            World.Relate(Entity, other, relation);
            return this;
        }
        
        public static implicit operator Entity(EntityBuilder builder) => builder.Entity;
    }
}