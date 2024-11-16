using System.Numerics;

public record struct Transform(Matrix4x4 Value)
{
    public Vector3 Position => Value.Translation;
    public Quaternion Rotation => Quaternion.CreateFromRotationMatrix(Value);
    public Vector3 Scale => new Vector3(
        new Vector3(Value.M11, Value.M12, Value.M13).Length(),
        new Vector3(Value.M21, Value.M22, Value.M23).Length(),
        new Vector3(Value.M31, Value.M32, Value.M33).Length()
    );

    // Extract the rotation part only to prevent scale influence
    private Matrix4x4 RotationMatrix
    {
        get
        {
            // Normalize the upper 3x3 part to remove scale
            var basisX = Vector3.Normalize(new Vector3(Value.M11, Value.M12, Value.M13));
            var basisY = Vector3.Normalize(new Vector3(Value.M21, Value.M22, Value.M23));
            var basisZ = Vector3.Normalize(new Vector3(Value.M31, Value.M32, Value.M33));

            return new Matrix4x4(
                basisX.X, basisX.Y, basisX.Z, 0,
                basisY.X, basisY.Y, basisY.Z, 0,
                basisZ.X, basisZ.Y, basisZ.Z, 0,
                0, 0, 0, 1
            );
        }
    }

    // Use the rotation matrix to get the directions
    public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, RotationMatrix);
    public Vector3 Right => Vector3.Transform(Vector3.UnitX, RotationMatrix);
}