using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsCore.Utility;

namespace PhysicsCore.Simulation
{
    public class RigidBody
    {
        #region Properties

        #region Visual only (these don't affect the physics)

        public Model Model { get; set; }

        public Color Colour { get; set; }

        public Matrix ScaleMatrix { get; set; }

        #endregion

        public float Mass { get; set; }

        public Matrix InverseBodyInertiaTensor { get; set; }

        public Vector3[] BoundingVertices { get; set; }

        public RigidBodyState State { get; set; }

        #endregion

        #region Methods

        public void ApplyPhysics(Vector3 force, Vector3 torque, float timeDelta)
        {
            float dt = timeDelta/Globals.PhysicsTimestepSubdivisionCount;
            for (int i = 0; i < Globals.PhysicsTimestepSubdivisionCount; i++)
            {
                State.Position += State.Velocity*dt;
                State.Velocity += force/Mass*dt;

                State.Orientation += Matrix.Transpose(Globals.SkewSymmetricMatrix(State.AngularVelocity)*Matrix.Transpose(State.Orientation))*dt;
                State.Orientation = Globals.OrthonormaliseMatrix(State.Orientation);

                State.AngularMomentum += torque*dt;
                State.AngularVelocity = Vector3.Transform(State.AngularMomentum, Matrix.Transpose(State.Orientation)*InverseBodyInertiaTensor*State.Orientation);
            }
        }

        #endregion
    }
}
