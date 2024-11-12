using MoonTools.ECS;

namespace Hanamura
{
    public interface IGameEntity
    {
        public static abstract void Configure(World world, Entity entity);
        public static abstract void SetState(World world, Entity entity);
    }
}