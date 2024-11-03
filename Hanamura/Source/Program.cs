using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;

namespace Hanamura
{
    internal class Program : Game
    {
        private static void Main(string[] args)
        {
            var windowCreateInfo = new WindowCreateInfo(
                "Hanamura",
                1920,
                1080,
                ScreenMode.Windowed
            );

            var frameLimiterSettings = new FrameLimiterSettings(
                FrameLimiterMode.Uncapped,
                0);
            
            var game = new Program(
                windowCreateInfo,
                frameLimiterSettings,
                ShaderFormat.SPIRV
            );
            game.Run();
        }

        private readonly RenderSystem _renderSystem;
        private readonly List<MoonTools.ECS.System> _systems = new();
        
        private Program(WindowCreateInfo windowCreateInfo, FrameLimiterSettings frameLimiterSettings,
            ShaderFormat availableShaderFormats) :
            base(windowCreateInfo, frameLimiterSettings, availableShaderFormats, 120)
        {
            ShaderCross.Initialize();
            var world = new World();
            _systems.Add(new PlayerMovementSystem(world, Inputs));
            _systems.Add(new CameraFollowSystem(world));
            _systems.Add(new CameraUpdateSystem(world, MainWindow));
            _systems.Add(new GridMarkerPositionSystem(MainWindow, world, Inputs));
            
            _renderSystem = new RenderSystem(world, new AssetStore(GraphicsDevice), GraphicsDevice, MainWindow);
            
            world.Spawn<DirectionalLightEntity>();
            world.Spawn<PrototypeGroundEntity>();
            var character = world.Spawn<PlayerCharacterEntity>();
            world.Spawn<PlayerControllerEntity>()
                .Relate(character, new PlayerControllerTarget());
            world.Spawn<MainCameraEntity>()
                .Relate(character, new CameraTarget());
            world.Spawn<GridMarkerEntity>();
        }

        protected override void Update(TimeSpan delta)
        {
            foreach (var system in _systems)
            {
                system.Update(delta);
            }
        }

        protected override void Draw(double alpha)
        {
            _renderSystem.Draw(alpha);
        }
    }
}