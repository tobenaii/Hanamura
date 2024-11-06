using System.Numerics;

namespace Hanamura
{
    public record struct Transform(Vector3 Position, Quaternion Rotation, Vector3 Scale)
    {
        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);

        public Transform() : this(Vector3.Zero, Quaternion.Identity, Vector3.One)
        {
        }
        
        public static Transform FromPosition(Vector3 position)
        {
            return new Transform
            {
                Position = position,
            };
        }
        
        public static Transform FromRotation(Quaternion rotation)
        {
            return new Transform
            {
                Rotation = rotation
            };
        }
        
        public static Transform FromScale(Vector3 scale)
        {
            return new Transform
            {
                Scale = scale
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
        
        public static Transform FromPositionScale(Vector3 position, Vector3 scale)
        {
            return new Transform
            {
                Position = position,
                Scale = scale
            };
        }
        
        public static Transform FromRotationScale(Quaternion rotation, Vector3 scale)
        {
            return new Transform
            {
                Rotation = rotation,
                Scale = scale
            };
        }

        public static Transform FromPositionRotationScale(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return new Transform
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            };
        }
    }
}