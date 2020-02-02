using Microsoft.Xna.Framework;
using PhysicsCore.Utility;

namespace PhysicsCore.Simulation
{
    public class RigidBody
    {
        #region Properties

        public float Mass { get; set; }

        public Matrix InverseBodyInertiaTensor { get; set; }

        public Vector3[] BoundingVertices { get; set; }

        public RigidBodyState State { get; set; }

        #endregion

        #region Methods

        public void ApplyPhysics(Vector3 force, Vector3 torque, float timeDelta)
        {
            State.Position += State.Velocity*timeDelta;
            State.Velocity += force/Mass*timeDelta;

            State.Orientation += Matrix.Transpose(Globals.SkewSymmetricMatrix(State.AngularVelocity)*Matrix.Transpose(State.Orientation))*timeDelta;
            State.Orientation = Globals.OrthonormaliseMatrix(State.Orientation);

            State.AngularMomentum += torque*timeDelta;
            State.AngularVelocity = Vector3.Transform(State.AngularMomentum, Matrix.Transpose(State.Orientation)*InverseBodyInertiaTensor*State.Orientation);
        }

        #endregion
    }
}
