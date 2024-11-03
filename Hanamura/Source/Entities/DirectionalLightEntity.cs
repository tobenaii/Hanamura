using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct DirectionalLightEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new DirectionalLight());
            world.Set(entity, Transform.FromPositionRotation(Vector3.UnitY, Quaternion.CreateFromAxisAngle(Vector3.UnitX, float.DegreesToRadians(-65))));
        }
    }
}