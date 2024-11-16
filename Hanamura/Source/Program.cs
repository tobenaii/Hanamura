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
        private readonly ModelManipulator _modelManipulator;

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
            _modelManipulator = new ModelManipulator(_world);
            _assetStore = new AssetStore(GraphicsDevice, MainWindow, contentPath);
            
            _assetStore.RegisterMaterial<StandardMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<GridMarkerMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            _assetStore.RegisterMaterial<BlobShadowMaterial>(MainWindow.SwapchainFormat, GraphicsDevice);
            
            _renderSystem = new RenderSystem(_world);
            
            _systems.Add(new UpdateTransformStateSystem(_world));
            _systems.Add(new PlayerInputSystem(_world, Inputs));
            _systems.Add(new CharacterMovementSystem(_world));
            _systems.Add(new OrbitFollowSystem(_world));
            _systems.Add(new GridMarkerPositionSystem(_world, Inputs));
            _systems.Add(new TransformSystem(_world));

            var mainWindow = _world.CreateEntity();
            _world.Set(mainWindow, new RenderSurface(MainWindow.Width, MainWindow.Height));
            
            SpawnDirectionalLight();
            SpawnGround();
            SpawnTree(new Vector3(0, 0, 0));
            SpawnTree(new Vector3(2, 0, 0));
            SpawnTree(new Vector3(-2, 0, 0));
            SpawnTree(new Vector3(0, 0, -2));
            var playerCharacter = SpawnPlayerCharacter();
            var camera = SpawnMainCamera(playerCharacter);
            SpawnPlayerController(playerCharacter, camera);
        }
        
        private void SpawnDirectionalLight()
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, LocalTransform.FromRotation(Quaternion.CreateFromAxisAngle(Vector3.UnitX, float.DegreesToRadians(-65))));
            _world.Set(entity, new DirectionalLight());
        }

        private void SpawnGround()
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, new LocalTransform());
            _modelManipulator.AddModel(entity, "Ground", new StandardMaterial("PixPal_BaseColor"));
        }
        
        private void SpawnTree(Vector3 position)
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, LocalTransform.FromPosition(position));
            _modelManipulator.AddModel(entity, "Tree", new StandardMaterial("PixPal_BaseColor"));
        }

        private Entity SpawnPlayerCharacter()
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, new HasBlobShadow(0.75f));
            _world.Set(entity, new LocalTransform());
            _world.Set(entity, new CharacterMovement());
            _modelManipulator.AddModel(entity, "PlayerCharacter", new StandardMaterial("PixPal_BaseColor"));
            
            return entity;
        }

        private Entity SpawnMainCamera(Entity playerCharacter)
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, new MainRenderCamera());
            _world.Set(entity, new CameraViewProjection());
            _world.Set(entity, new LocalTransform());
            _world.Set(entity, new CameraSettings(float.DegreesToRadians(50), 0.01f, 100f));
            _world.Relate(entity, playerCharacter, new OrbitsTarget(Vector3.UnitY * 1f, 0, 0.75f, 7.5f));
            return entity;
        }
        
        private void SpawnPlayerController(Entity playerCharacter, Entity camera)
        {
            var entity = _world.CreateEntity();
            _world.Set(entity, new PlayerInput());
            _world.Relate(entity, playerCharacter, new ControlsMovement());
            _world.Relate(entity, camera, new ControlsOrbit());
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