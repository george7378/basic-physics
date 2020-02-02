using Microsoft.Xna.Framework;
using PhysicsCore.Utility;

namespace PhysicsCore.Simulation
{
    public class RigidBodySimplified
    {
        #region Properties

        public Vector3[] BoundingVertices { get; set; }

        public RigidBodySimplifiedState State { get; set; }

        #endregion

        #region Methods

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
