using System.Numerics;
using MoonTools.ECS;
using MoonWorks.Input;

namespace Hanamura
{
    public class GridMarkerPositionSystem : MoonTools.ECS.System
    {
        private readonly Inputs _inputs;

        public GridMarkerPositionSystem(World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
        }

        public override void Update(TimeSpan delta)
        {
            //TODO: Fix this up to target in front of the player instead of using mouse
            /*var mainCamera = World.GetSingletonEntity<MainRenderCamera>();
            var renderSurface = World.Get<RenderSurface>(mainCamera);
            var cameraConfig = World.Get<CameraSettings>(mainCamera);
            var transform = World.Get<LocalTransform>(mainCamera);
            var cameraPosition = transform.Position;
            var cameraDirection = transform.Forward;
            var upDirection = Vector3.UnitY;
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                cameraConfig.Fov,
                renderSurface.Width / (float) renderSurface.Height,
                cameraConfig.Near,
                cameraConfig.Far
            );
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            var mousePos = new Vector2(_inputs.Mouse.X, _inputs.Mouse.Y);
            var groundPoint = GetMouseGroundPoint(mousePos, view, projection, renderSurface.Width, renderSurface.Height);
            var position = new Vector3(float.Floor(groundPoint.X * 2) / 2 + 0.25f, 0, float.Floor(groundPoint.Z * 2) / 2 + 0.25f);

            ref var cursorGroundPosition = ref World.Get<LocalTransform>(World.GetSingletonEntity<GridMarkerTag>());
            cursorGroundPosition.Position = position with { Y = 0.01f };*/
        }

        /*private static Vector3 GetMouseGroundPoint(Vector2 mousePos, Matrix4x4 view, Matrix4x4 projection, uint screenWidth, uint screenHeight)
        {
            Matrix4x4.Invert(view * projection, out var invVP);
    
            var ndcNear = new Vector4(
                2.0f * mousePos.X / screenWidth - 1,
                1 - 2.0f * mousePos.Y / screenHeight,
                0, 1);
    
            var ndcFar = ndcNear with { Z = 1 };
    
            var near = Vector4.Transform(ndcNear, invVP);
            var far = Vector4.Transform(ndcFar, invVP);
            near /= near.W;
            far /= far.W;
    
            var dir = Vector3.Normalize(new Vector3(far.X - near.X, far.Y - near.Y, far.Z - near.Z));
            var point = new Vector3(near.X, near.Y, near.Z) + dir * (-near.Y / dir.Y);
            return point;
        }*/
    }
}