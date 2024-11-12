using MoonTools.ECS;

namespace Hanamura
{
    public struct MainWindow : IGameEntity
    {
        public static void Configure(World world, Entity entity)
        {
        }

        public static void SetState(World world, Entity entity)
        {
            world.Set(entity, new MainWindowTag());
        }
    }
}