using Microsoft.Xna.Framework;

namespace PhysicsCore.Simulation
{
    public class RigidBodySimplifiedState
    {
        #region Properties

        public Vector3 Position { get; set; }

        public Vector3 Velocity { get; set; }

        public Matrix Orientation { get; set; }

        public Vector3 AngularVelocity { get; set; }

        #endregion
    }
}
