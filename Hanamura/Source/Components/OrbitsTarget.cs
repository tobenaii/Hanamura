using System.Numerics;

namespace Hanamura
{
    public record struct OrbitsTarget(
        Vector3 Offset,
        float Yaw,
        float Pitch,
        float MinPitch,
        float MaxPitch,
        float MinDistance,
        float MaxDistance)
    {
        public OrbitsTarget(Vector3 offset, float minPitch, float maxPitch, float minDistance, float maxDistance)
            : this(offset, 0, 0, minPitch, maxPitch, minDistance, maxDistance)
        {
        }
    }
}