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
                1720,
                968,
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
        private readonly World _world = new();

        private Program(WindowCreateInfo windowCreateInfo, FrameLimiterSettings frameLimiterSettings,
            ShaderFormat availableShaderFormats, string contentPath) :
            base(windowCreateInfo, frameLimiterSettings, availableShaderFormats, 120)
        {
            MainWindow.SetPosition(1720, 236);
            //MainWindow.SetPosition(1720, 1440 - 968);

            ShaderCross.Initialize();
            _assetStore = new AssetStore(GraphicsDevice, contentPath);
            _systems.Add(new HotReloadSystem(_world));
            _systems.Add(new PlayerMovementSystem(_world, Inputs));
            _systems.Add(new CameraFollowSystem(_world));
            _systems.Add(new CameraProjectionSystem(_world, MainWindow));
            _systems.Add(new GridMarkerPositionSystem(MainWindow, _world, Inputs));

            _renderSystem = new RenderSystem(_world, _assetStore, GraphicsDevice, MainWindow);

            _world.Spawn<DirectionalLightEntity>();
            _world.Spawn<PrototypeGroundEntity>();
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(2, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(-2, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, -2)));
            var character = _world.Spawn<PlayerCharacterEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, 2)));
            _world.Spawn<PlayerControllerEntity>()
                .Relate(character, new PlayerControllerTargetTag());
            _world.Spawn<MainCameraEntity>()
                .Relate(character, new CameraTargetTag());
            _world.Spawn<GridMarkerEntity>();
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