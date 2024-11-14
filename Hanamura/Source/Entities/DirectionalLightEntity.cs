using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct DirectionalLightEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, LocalTransform.FromRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, float.DegreesToRadians(-65))));
            world.Set(entity, new DirectionalLightTag());
        }
    }
}