using System.Numerics;

namespace Hanamura
{
    public record struct Transform(Vector3 Position, Quaternion Rotation)
    {
        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
        
        public static Transform FromPosition(Vector3 position)
        {
            return new Transform
            {
                Position = position,
                Rotation = Quaternion.Identity
            };
        }
        
        public static Transform FromPositionRotation(Vector3 position, Quaternion rotation)
        {
            return new Transform
            {
                Position = position,
                Rotation = rotation
            };
        }
    }
}