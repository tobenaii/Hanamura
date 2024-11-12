using MoonTools.ECS;

namespace Hanamura
{
    public struct PlayerCharacterEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialConfig("Capsule", "PixPal_BaseColor"));
            world.Set(entity, new BlobShadowConfig(0.75f));
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new Transform());
        }
    }
}