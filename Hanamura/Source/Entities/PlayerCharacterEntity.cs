using MoonTools.ECS;

namespace Hanamura
{
    public struct PlayerCharacterEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialData("Capsule".GetHashCode(), "PixPal_BaseColor".GetHashCode()));
            world.Set(entity, new Transform());
            world.Set(entity, new HasblobShadow(0.75f));
        }
    }
}