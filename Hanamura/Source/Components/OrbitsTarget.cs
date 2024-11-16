using System.Numerics;

namespace Hanamura
{
    public record struct OrbitsTarget(
        Vector3 Offset,
        float Yaw,
        float Pitch,
        float Distance)
    {
        public OrbitsTarget(Vector3 offset, float pitch, float distance)
            : this(offset, 0, pitch, distance)
        {
        }
    }
}