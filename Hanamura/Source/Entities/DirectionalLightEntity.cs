using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct DirectionalLightEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new DirectionalLight());
            world.Set(entity, Transform.FromRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, float.DegreesToRadians(-65))));
        }
    }
}