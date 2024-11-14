using System.Numerics;

namespace Hanamura
{
    public record struct LocalTransform(Vector3 Position, Quaternion Rotation, Vector3 Scale)
    {
        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation);
        
        public LocalTransform() : this(Vector3.Zero, Quaternion.Identity, Vector3.One)
        {
        }
        
        public static LocalTransform FromPosition(Vector3 position)
        {
            return new LocalTransform
            {
                Position = position,
            };
        }
        
        public static LocalTransform FromRotation(Quaternion rotation)
        {
            return new LocalTransform
            {
                Rotation = rotation
            };
        }
        
        public static LocalTransform FromScale(Vector3 scale)
        {
            return new LocalTransform
            {
                Scale = scale
            };
        }
        
        public static LocalTransform FromPositionRotation(Vector3 position, Quaternion rotation)
        {
            return new LocalTransform
            {
                Position = position,
                Rotation = rotation
            };
        }
        
        public static LocalTransform FromPositionScale(Vector3 position, Vector3 scale)
        {
            return new LocalTransform
            {
                Position = position,
                Scale = scale
            };
        }
        
        public static LocalTransform FromRotationScale(Quaternion rotation, Vector3 scale)
        {
            return new LocalTransform
            {
                Rotation = rotation,
                Scale = scale
            };
        }

        public static LocalTransform FromPositionRotationScale(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return new LocalTransform
            {
                Position = position,
                Rotation = rotation,
                Scale = scale
            };
        }
    }
}