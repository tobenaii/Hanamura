using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Input;
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
        private readonly World _world;
        private readonly GameWorld _game;

        private Program(WindowCreateInfo windowCreateInfo, FrameLimiterSettings frameLimiterSettings,
            ShaderFormat availableShaderFormats, string contentPath) :
            base(windowCreateInfo, frameLimiterSettings, availableShaderFormats, 60)
        {
            /*SDL.SDL_SetGPUSwapchainParameters(GraphicsDevice.Handle,
                MainWindow.Handle,
                SDL.SDL_GPUSwapchainComposition.SDL_GPU_SWAPCHAINCOMPOSITION_SDR,
                SDL.SDL_GPUPresentMode.SDL_GPU_PRESENTMODE_IMMEDIATE
            );*/
            MainWindow.SetRelativeMouseMode(true);
            MainWindow.SetPosition(1720, 236);

            ShaderCross.Initialize();
            _world = new World();
            _assetStore = new AssetStore(GraphicsDevice, MainWindow, contentPath);
            _game = new GameWorld(_assetStore, _world);
            
            _assetStore.RegisterMaterial<StandardMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<GridMarkerMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<BlobShadowMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            
            _renderSystem = new RenderSystem(_world);
            
#if DEBUG
            _systems.Add(new HotReloadSystem(_world));
#endif
            _systems.Add(new UpdateTransformStateSystem(_world));
            _systems.Add(new PlayerMovementSystem(_world, Inputs));
            _systems.Add(new CameraFollowSystem(_world, Inputs));
            _systems.Add(new GridMarkerPositionSystem(_world, Inputs));
            _systems.Add(new TransformSystem(_world));
            
            _game.Spawn<DirectionalLightEntity>();
            _game.Spawn<PrototypeGroundEntity>();
            _game.Spawn<GridMarkerEntity>();
            _game.Spawn<TreeEntity>()
                .Set(LocalTransform.FromPosition(new Vector3(0, 0, 0)));
            _game.Spawn<TreeEntity>()
                .Set(LocalTransform.FromPosition(new Vector3(2, 0, 0)));
            _game.Spawn<TreeEntity>()
                .Set(LocalTransform.FromPosition(new Vector3(-2, 0, 0)));
            _game.Spawn<TreeEntity>()
                .Set(LocalTransform.FromPosition(new Vector3(0, 0, -2)));
            _game.Spawn<MainWindow>()
                .Set(new Rect(MainWindow.Position.Item1, MainWindow.Position.Item2, MainWindow.Width, MainWindow.Height));

            var character = _game.Spawn<PlayerCharacterEntity>()
                .Set(LocalTransform.FromPosition(new Vector3(0, 0, 2)));
            _game.Spawn<PlayerControllerEntity>()
                .Relate(character, new PlayerControllerTargetTag());
            _game.Spawn<MainCameraEntity>()
                .Relate(character, new CameraTargetTag());
        }
        
        protected override void Update(TimeSpan delta)
        {
            if (Inputs.Keyboard.IsPressed(KeyCode.Escape))
            {
                MainWindow.SetRelativeMouseMode(false);
            }
            else if (MainWindow.RelativeMouseMode == false && Inputs.Mouse.LeftButton.IsPressed)
            {
                MainWindow.SetRelativeMouseMode(true);
            }
            
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