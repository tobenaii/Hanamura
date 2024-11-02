using System.Numerics;
using System.Runtime.InteropServices;

namespace Hanamura
{
    public struct TransformVertexUniform
    {
        public Matrix4x4 ViewProjection;
        public Matrix4x4 ModelProjection;

        public TransformVertexUniform(Matrix4x4 viewProjection, Matrix4x4 modelProjection)
        {
            ViewProjection = viewProjection;
            ModelProjection = modelProjection;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LightingFragmentUniform
    {
        public Vector3 LightPosition;

        public LightingFragmentUniform(Vector3 lightPosition)
        {
            LightPosition = lightPosition;
        }
    }
}