using MoonTools.ECS;

namespace Hanamura
{
    public struct TreeEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialConfig(
                "Tree",
                "PixPal_BaseColor"
            ));
            world.Set(entity, new BlobShadowConfig(4));
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new Transform());
        }
    }
}