using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct PrototypeGroundEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialConfig("Ground", "prototype_512x512_green1"));
        }

        public static void Init(World world, Entity entity)
        {
            world.Set(entity, new Transform());
        }
    }
}