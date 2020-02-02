using Microsoft.Xna.Framework;

namespace PhysicsCore.Utility
{
    public static class Globals
    {
        #region Standalone methods

        public static Matrix SkewSymmetricMatrix(Vector3 vector)
        {
            Matrix result = Matrix.Identity;

            result[0, 0] = 0;
            result[0, 1] = -vector.Z;
            result[0, 2] = vector.Y;
            result[1, 0] = vector.Z;
            result[1, 1] = 0;
            result[1, 2] = -vector.X;
            result[2, 0] = -vector.Y;
            result[2, 1] = vector.X;
            result[2, 2] = 0;

            return result;
        }

        public static Matrix OrthonormaliseMatrix(Matrix matrix)
        {
            Vector3 up = Vector3.Normalize(matrix.Up);
            Vector3 forward = Vector3.Normalize(Vector3.Cross(up, matrix.Right));
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, up));

            Matrix result = Matrix.Identity;
            result.Up = up;
            result.Forward = forward;
            result.Right = right;

            return result;
        }

        #endregion
    }
}
