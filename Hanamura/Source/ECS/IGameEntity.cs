using MoonTools.ECS;

namespace Hanamura
{
    public interface IGameEntity
    {
        public static abstract void Configure(World world, Entity entity);
        public static abstract void Init(World world, Entity entity);
    }
}