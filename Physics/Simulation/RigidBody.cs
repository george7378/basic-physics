using Microsoft.Xna.Framework;

namespace Physics.Simulation
{
    public class RigidBody
    {
        #region Properties

        public Vector3[] BoundingVertices { get; set; }

        public RigidBodyState State { get; set; }

        #endregion
    }
}
