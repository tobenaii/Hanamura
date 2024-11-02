using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct PlayerCharacterEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialData("Capsule".GetHashCode(), "PixPal_BaseColor".GetHashCode()));
            world.Set(entity, new Transform(Vector3.Zero, Quaternion.Identity));
        }
    }
}