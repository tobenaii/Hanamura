using MoonTools.ECS;

namespace Hanamura
{
    public struct PrototypeGroundEntity : IGameEntity
    {
        public static string GameObject => "Ground";

        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialConfig("prototype_512x512_green1"));
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new LocalTransform());
        }
    }
}