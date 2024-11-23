using System.Numerics;
using MoonTools.ECS;

namespace Hanamura
{
    public class CharacterMovementSystem : MoonTools.ECS.System
    {
        private readonly Filter _filter;
        
        public CharacterMovementSystem(World world) : base(world)
        {
            _filter = FilterBuilder
                .Include<CharacterControls>()
                .Include<LocalTransform>()
                .Build();
        }

        public override void Update(TimeSpan delta)
        {
            foreach (var entity in _filter.Entities)
            {
                var controls = World.Get<CharacterControls>(entity);
                ref var transform = ref World.Get<LocalTransform>(entity);
                ref var characterViewRotation = ref World.Get<CharacterViewRotation>(entity);
                //clamp pitch
                var maxPitch = float.DegreesToRadians(85f);
                characterViewRotation = characterViewRotation with
                {
                    Pitch = Math.Clamp(characterViewRotation.Pitch, -maxPitch, maxPitch)
                };
                
                var inputDirection = controls.Move == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(controls.Move);
                var translation = Vector2.Transform(inputDirection, Matrix3x2.CreateRotation(characterViewRotation.Yaw));
                transform.Position += new Vector3(translation.X, 0, translation.Y) * 2 * (float) delta.TotalSeconds;
                transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, -characterViewRotation.Yaw);
                characterViewRotation = new CharacterViewRotation(characterViewRotation.Yaw + controls.LookYawPitch.X, characterViewRotation.Pitch + controls.LookYawPitch.Y);
            }
        }
    }
}