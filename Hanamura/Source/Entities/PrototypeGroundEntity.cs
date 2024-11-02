using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct PrototypeGroundEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new StandardMaterialData("Ground".GetHashCode(), "prototype_512x512_green1".GetHashCode()));
            world.Set(entity, Transform.FromPositionRotation(new Vector3(0, 0, 0), Quaternion.Identity));
        }
    }
}