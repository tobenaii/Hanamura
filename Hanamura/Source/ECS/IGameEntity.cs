using MoonTools.ECS;

namespace Hanamura
{
    public interface IGameEntity
    {
        public static abstract void Build(World world, Entity entity);
    }
}