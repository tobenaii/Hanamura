using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public struct PlayerControllerEntity : IGameEntity
    {
        public static void Build(World world, Entity entity)
        {
            world.Set(entity, new PlayerController(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D));
        }
    }
}