using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using SDL3;

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
                ShaderFormat.SPIRV,
                Path.Join(args.Length > 0 ? args[0] : SDL.SDL_GetBasePath(), "Content/")
            );
            game.Run();
        }

        private readonly AssetStore _assetStore;
        private readonly RenderSystem _renderSystem;
        private readonly List<MoonTools.ECS.System> _systems = new();

        private Program(WindowCreateInfo windowCreateInfo, FrameLimiterSettings frameLimiterSettings,
            ShaderFormat availableShaderFormats, string contentPath) :
            base(windowCreateInfo, frameLimiterSettings, availableShaderFormats, 120)
        {
            ShaderCross.Initialize();
            _assetStore = new AssetStore(GraphicsDevice, contentPath);
            var world = new World();
            _systems.Add(new PlayerMovementSystem(world, Inputs));
            _systems.Add(new CameraFollowSystem(world));
            _systems.Add(new CameraUpdateSystem(world, MainWindow));
            _systems.Add(new GridMarkerPositionSystem(MainWindow, world, Inputs));
            
            _renderSystem = new RenderSystem(world, _assetStore, GraphicsDevice, MainWindow);
            
            world.Spawn<DirectionalLightEntity>();
            world.Spawn<PrototypeGroundEntity>();
            world.Spawn<TreeEntity>();
            var character = world.Spawn<PlayerCharacterEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, 2)));
            world.Spawn<PlayerControllerEntity>()
                .Relate(character, new PlayerControllerTarget());
            world.Spawn<MainCameraEntity>()
                .Relate(character, new CameraTarget());
            world.Spawn<GridMarkerEntity>();
        }

        protected override void Update(TimeSpan delta)
        {
            _assetStore.CheckForReload();
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