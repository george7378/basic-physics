using Microsoft.Xna.Framework;

namespace Physics.Simulation
{
    public class RigidBodyState
    {
        #region Properties

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }

        public Matrix Orientation { get; set; }

        public Vector3 AngularVelocity { get; set; }

        #endregion
    }
}
