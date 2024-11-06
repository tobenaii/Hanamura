using MoonTools.ECS;

namespace Hanamura
{
    public struct TreeEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new Transform());
            world.Set(entity, new StandardMaterialData(
                "Tree".GetHashCode(),
                "PixPal_BaseColor".GetHashCode()
                ));
            world.Set(entity, new HasblobShadow(4));
        }
    }
}