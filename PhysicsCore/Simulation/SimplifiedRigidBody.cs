using Microsoft.Xna.Framework;
using PhysicsCore.Utility;

namespace PhysicsCore.Simulation
{
    public class SimplifiedRigidBody
    {
        #region Properties

        public Vector3[] BoundingVertices { get; set; }

        public SimplifiedRigidBodyState State { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// By assuming mass to be 1 and the inertia tensor to be an identity matrix, force becomes equivalent to
        /// acceleration and torque becomes equivalent to angular acceleration. Angular velocity and angular momentum
        /// are also equivalent.
        /// </summary>
        public void ApplyPhysics(Vector3 acceleration, Vector3 angularAcceleration, float timeDelta)
        {
            State.Position += State.Velocity*timeDelta;
            State.Velocity += acceleration*timeDelta;

            State.Orientation += Matrix.Transpose(Globals.SkewSymmetricMatrix(State.AngularVelocity)*Matrix.Transpose(State.Orientation))*timeDelta;
            State.Orientation = Globals.OrthonormaliseMatrix(State.Orientation);

            State.AngularVelocity += angularAcceleration*timeDelta;
        }

        #endregion
    }
}
