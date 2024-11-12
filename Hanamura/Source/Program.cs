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
            base(windowCreateInfo, frameLimiterSettings, availableShaderFormats)
        {
            MainWindow.SetPosition(1720, 236);

            ShaderCross.Initialize();
            _assetStore = new AssetStore(GraphicsDevice, MainWindow, contentPath);
            _assetStore.RegisterMaterial<StandardMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<GridMarkerMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<BlobShadowMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            
            _renderSystem = new RenderSystem(_world);

            _systems.Add(new HotReloadSystem(_world));
            _systems.Add(new UpdatePreviousTransformSystem(_world));
            _systems.Add(new PlayerMovementSystem(_world, Inputs));
            _systems.Add(new CameraFollowSystem(_world));
            _systems.Add(new GridMarkerPositionSystem(_world, Inputs));
            
            _world.Spawn<DirectionalLightEntity>();
            _world.Spawn<PrototypeGroundEntity>();
            _world.Spawn<GridMarkerEntity>();
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(2, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(-2, 0, 0)));
            _world.Spawn<TreeEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, -2)));
            _world.Spawn<MainWindow>()
                .Set(new Rect(MainWindow.Position.Item1, MainWindow.Position.Item2, MainWindow.Width, MainWindow.Height));

            var character = _world.Spawn<PlayerCharacterEntity>()
                .Set(Transform.FromPosition(new Vector3(0, 0, 2)));
            _world.Spawn<PlayerControllerEntity>()
                .Relate(character, new PlayerControllerTargetTag());
            _world.Spawn<MainCameraEntity>()
                .Relate(character, new CameraTargetTag());
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
            _renderSystem.Draw(alpha, MainWindow, GraphicsDevice, _assetStore);
        }
    }
}