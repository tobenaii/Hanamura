using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public struct MainCameraEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new MainCamera());
            world.Set(entity, new Transform());
            world.Set(entity, new CameraFollowConfig
            {
                Angle = float.DegreesToRadians(75),
                Distance = 5
            });
        }
    }
}