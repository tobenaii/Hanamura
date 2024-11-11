using MoonTools.ECS;

namespace Hanamura
{
    public struct MainCameraEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new CameraConfig
            {
                Fov = float.DegreesToRadians(75),
                Near = 0.01f,
                Far = 100f
            });
            world.Set(entity, new CameraFollowConfig
            {
                Angle = float.DegreesToRadians(75),
                Distance = 5
            });
        }

        public static void Init(World world, Entity entity)
        {
            world.Set(entity, new MainCameraTag());
            world.Set(entity, new CameraViewProjection());
            world.Set(entity, new Transform());
        }
    }
}