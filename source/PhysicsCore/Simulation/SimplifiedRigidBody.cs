using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsCore.Utility;

namespace PhysicsCore.Simulation
{
    public class SimplifiedRigidBody
    {
        #region Visual only (these don't affect the physics)

        public Model Model { get; set; }

        public Color Colour { get; set; }

        public Matrix ScaleMatrix { get; set; }

        #endregion

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
            float dt = timeDelta/Globals.PhysicsTimestepSubdivisionCount;
            for (int i = 0; i < Globals.PhysicsTimestepSubdivisionCount; i++)
            {
                State.Position += State.Velocity*dt;
                State.Velocity += acceleration*dt;

                State.Orientation += Matrix.Transpose(Globals.SkewSymmetricMatrix(State.AngularVelocity)*Matrix.Transpose(State.Orientation))*dt;
                State.Orientation = Globals.OrthonormaliseMatrix(State.Orientation);

                State.AngularVelocity += angularAcceleration*dt;
            }
        }

        #endregion
    }
}
