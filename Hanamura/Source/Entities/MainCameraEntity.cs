using MoonTools.ECS;

namespace Hanamura
{
    public struct MainCameraEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new CameraConfig
            {
                Fov = float.DegreesToRadians(50),
                Near = 0.01f,
                Far = 100f
            });
            world.Set(entity, new CameraFollowConfig
            {
                Height = 1.0f,
                MinDistance = 5,
                MaxDistance = 7.5f
            });
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new MainCameraTag());
            world.Set(entity, new CameraViewProjection());
            world.Set(entity, new LocalTransform());
            world.Set(entity, new CameraFollowState());
        }
    }
}