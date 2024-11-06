using System.Numerics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Input;

namespace Hanamura
{
    public class GridMarkerPositionSystem : MoonTools.ECS.System
    {
        private Inputs _inputs;
        private Window _window;

        public GridMarkerPositionSystem(Window window, World world, Inputs inputs) : base(world)
        {
            _inputs = inputs;
            _window = window;
        }

        public override void Update(TimeSpan delta)
        {
            var camera = World.GetSingletonEntity<MainCamera>();
            var transform = World.Get<Transform>(camera);
            var cameraPosition = transform.Position;
            var cameraDirection = transform.Forward;
            var upDirection = Vector3.UnitY;
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                float.DegreesToRadians(90f),
                (float)_window.Width / _window.Height,
                0.01f,
                100f
            );
            var view = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, upDirection);
            var mousePos = new Vector2(_inputs.Mouse.X, _inputs.Mouse.Y);
            var groundPoint = GetMouseGroundPoint(mousePos, view, projection, _window.Width, _window.Height);
            var position = new Vector3(float.Floor(groundPoint.X * 2) / 2 + 0.25f, 0, float.Floor(groundPoint.Z * 2) / 2 + 0.25f);

            ref var cursorGroundPosition = ref World.Get<Transform>(World.GetSingletonEntity<GridMarkerData>());
            cursorGroundPosition.Position = position with { Y = 0.01f };
        }

        private static Vector3 GetMouseGroundPoint(Vector2 mousePos, Matrix4x4 view, Matrix4x4 projection, uint screenWidth, uint screenHeight)
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
        }
    }
}