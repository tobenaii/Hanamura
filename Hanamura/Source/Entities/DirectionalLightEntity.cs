using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct DirectionalLightEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new DirectionalLight());
            world.Set(entity, new Transform()
            {
                Position = new Vector3(100, 100, 100)
            });
        }
    }
}