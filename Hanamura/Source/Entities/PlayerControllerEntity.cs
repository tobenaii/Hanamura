using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public struct PlayerControllerEntity : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
            world.Set(entity, new PlayerControllerConfig(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
        }

        public static void SetState(World world, Entity entity)
        {
        }
    }
}