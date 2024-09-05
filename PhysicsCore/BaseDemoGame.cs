using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsCore.Simulation;
using PhysicsCore.Utility;

namespace PhysicsCore
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BaseDemoGame : Game
    {
        #region Constants

        private const int ShadowMapSize = 1024;

        /// <summary>
        /// A little offset to make sure objects don't sink into the floor.
        /// </summary>
        private const float CollisionBias = 0.004f;

        /// <summary>
        /// A dimensionless measure of how hard the floor pushes against the objects.
        /// </summary>
        private const float ReactionForceMultiplier = 100;

        /// <summary>
        /// A dimensionless measure of how much friction the floor applies to the objects.
        /// </summary>
        private const float FrictionForceMultiplier = 5;

        #endregion

        #region Fields

        private readonly GraphicsDeviceManager _graphics;

        private DirectionLight _light;
        private Vector3 _cameraPosition;

        private RigidBody _plankRigidBody;
        private SimplifiedRigidBody _cubeRigidBody;

        private Matrix _viewMatrix, _projectionMatrix;
        private Matrix _lightViewMatrix, _lightProjectionMatrix;

        private Effect _singleColourWithShadowEffect;
        private Effect _shadowMapEffect;

        private Model _unitCubeModel, _planeModel;

        private RenderTarget2D _shadowMapRenderTarget;

        private Vector3 _gravity;

        #endregion

        #region Constructors

        public BaseDemoGame()
        {
            _graphics = new GraphicsDeviceManager(this) { PreferMultiSampling = true };

            Content.RootDirectory = "Content";
        }

        #endregion

        #region Private methods

        #region Content drawing

        private void DrawSceneShadowMap()
        {
            #region Plank

            foreach (ModelMesh mesh in _plankRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            Matrix plankWorldMatrix = _plankRigidBody.ScaleMatrix*_plankRigidBody.State.Orientation*Matrix.CreateTranslation(_plankRigidBody.State.Position);

            foreach (ModelMesh mesh in _plankRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(plankWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            #endregion

            #region Cube

            foreach (ModelMesh mesh in _cubeRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            Matrix cubeWorldMatrix = _cubeRigidBody.ScaleMatrix*_cubeRigidBody.State.Orientation*Matrix.CreateTranslation(_cubeRigidBody.State.Position);

            foreach (ModelMesh mesh in _cubeRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(cubeWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            #endregion

            #region Plane

            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            #endregion
        }

        private void DrawScene()
        {
            #region Plank

            foreach (ModelMesh mesh in _plankRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourWithShadowEffect;
                }
            }
    
            Matrix plankWorldMatrix = _plankRigidBody.ScaleMatrix*_plankRigidBody.State.Orientation*Matrix.CreateTranslation(_plankRigidBody.State.Position);

            foreach (ModelMesh mesh in _plankRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourWithShadowTechnique"];

                    effect.Parameters["World"].SetValue(plankWorldMatrix);
                    effect.Parameters["WorldViewProjection"].SetValue(plankWorldMatrix*_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(plankWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(_plankRigidBody.Colour.ToVector3());
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

            #endregion

            #region Cube

            foreach (ModelMesh mesh in _cubeRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourWithShadowEffect;
                }
            }
    
            Matrix cubeWorldMatrix = _cubeRigidBody.ScaleMatrix*_cubeRigidBody.State.Orientation*Matrix.CreateTranslation(_cubeRigidBody.State.Position);

            foreach (ModelMesh mesh in _cubeRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourWithShadowTechnique"];

                    effect.Parameters["World"].SetValue(cubeWorldMatrix);
                    effect.Parameters["WorldViewProjection"].SetValue(cubeWorldMatrix*_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(cubeWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(_cubeRigidBody.Colour.ToVector3());
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

            #endregion

            #region Plane

            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourWithShadowEffect;
                }
            }

            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourWithShadowTechnique"];

                    effect.Parameters["World"].SetValue(Matrix.Identity);
                    effect.Parameters["WorldViewProjection"].SetValue(_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(new Vector3(0.5f, 0.5f, 1));
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

            #endregion
        }

        #endregion

        #region Misc.

        /// <summary>
        /// Currently each object falls under gravity and interacts with the flat floor using a basic collision algorithm.
        /// The collisions consist of a reaction force which pushes away from the floor, and a friction force which dampens
        /// motion. The collision forces are summed over each vertex which intersects with the floor.
        /// You could add other interactions here - elastic springs, inter-object collision, explosions, push/pull forces...
        /// </summary>
        private void UpdatePhysics(float timeDelta)
        {
            #region Plank

            Vector3 plankForce = Vector3.Zero;
            Vector3 plankTorque = Vector3.Zero;

            // Floor contact interactions
            foreach (Vector3 vertexPosition in _plankRigidBody.BoundingVertices)
            {
                Vector3 vertexLocalPosition = Vector3.Transform(vertexPosition, _plankRigidBody.State.Orientation);
                Vector3 vertexWorldPosition = _plankRigidBody.State.Position + vertexLocalPosition;

                float penetrationDepth = CollisionBias - vertexWorldPosition.Y;
                if (penetrationDepth >= 0)
                {
                    Vector3 contactNormal = Vector3.UnitY;
                    Vector3 vertexVelocity = _plankRigidBody.State.Velocity + Vector3.Cross(_plankRigidBody.State.AngularVelocity, vertexLocalPosition);
                    Vector3 vertexContactForce = _plankRigidBody.Mass*(ReactionForceMultiplier*contactNormal*penetrationDepth - FrictionForceMultiplier*vertexVelocity);

                    plankForce += vertexContactForce;
                    plankTorque += Vector3.Cross(vertexLocalPosition, vertexContactForce);
                }
            }

            // Gravity
            plankForce += _plankRigidBody.Mass*_gravity;

            _plankRigidBody.ApplyPhysics(plankForce, plankTorque, timeDelta);

            #endregion

            #region Cube

            Vector3 cubeForce = Vector3.Zero;
            Vector3 cubeTorque = Vector3.Zero;

            // Floor contact interactions
            foreach (Vector3 vertexPosition in _cubeRigidBody.BoundingVertices)
            {
                Vector3 vertexLocalPosition = Vector3.Transform(vertexPosition, _cubeRigidBody.State.Orientation);
                Vector3 vertexWorldPosition = _cubeRigidBody.State.Position + vertexLocalPosition;

                float penetrationDepth = CollisionBias - vertexWorldPosition.Y;
                if (penetrationDepth >= 0)
                {
                    Vector3 contactNormal = Vector3.UnitY;
                    Vector3 vertexVelocity = _cubeRigidBody.State.Velocity + Vector3.Cross(_cubeRigidBody.State.AngularVelocity, vertexLocalPosition);
                    Vector3 vertexContactForce = ReactionForceMultiplier*contactNormal*penetrationDepth - FrictionForceMultiplier*vertexVelocity;

                    cubeForce += vertexContactForce;
                    cubeTorque += Vector3.Cross(vertexLocalPosition, vertexContactForce);
                }
            }

            cubeForce += _gravity;

            _cubeRigidBody.ApplyPhysics(cubeForce, cubeTorque, timeDelta);

            #endregion
        }

        #endregion

        #endregion

        #region Game overrides

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _light = new DirectionLight(Vector3.Normalize(new Vector3(1, -1, -1)), 1, 0.2f);
            _cameraPosition = new Vector3(10, 10, 10);

            _plankRigidBody = new RigidBody()
            {
                Colour = new Color(new Vector3(0.7f, 0.2f, 0.2f)),
                ScaleMatrix = Matrix.CreateScale(2, 0.4f, 6),
                Mass = 5,
                BoundingVertices = new Vector3[]
                {
                    new Vector3(-1, 0.2f, 3),
                    new Vector3(1, 0.2f, 3),
                    new Vector3(1, 0.2f, -3),
                    new Vector3(-1, 0.2f, -3),
                    new Vector3(-1, -0.2f, 3),
                    new Vector3(1, -0.2f, 3),
                    new Vector3(1, -0.2f, -3),
                    new Vector3(-1, -0.2f, -3)
                },
                State = new RigidBodyState()
                {
                    Position = new Vector3(-5, 5, 0),
                    Velocity = new Vector3(0, 5, -2),
                    Orientation = Matrix.Identity,
                    AngularMomentum = new Vector3(5, 18, 18)
                }
            };

            // The plank requires an inertia tensor to be defined. This depends on mass and dimensions, and
            // the formulation differs across object shapes. A good list for common shapes can be found here:
            // https://en.wikipedia.org/wiki/List_of_moments_of_inertia#List_of_3D_inertia_tensors
            // The plank uses the solid cuboid definition.
            Matrix plankBodyInertiaTensor = Matrix.Identity;
            plankBodyInertiaTensor[0, 0] = _plankRigidBody.Mass/12*(6*6 + 0.4f*0.4f);
            plankBodyInertiaTensor[1, 1] = _plankRigidBody.Mass/12*(6*6 + 2*2);
            plankBodyInertiaTensor[2, 2] = _plankRigidBody.Mass/12*(2*2 + 0.4f*0.4f);
            _plankRigidBody.InverseBodyInertiaTensor = Matrix.Invert(plankBodyInertiaTensor);

            _cubeRigidBody = new SimplifiedRigidBody()
            {
                Colour = new Color(new Vector3(0.7f, 0.7f, 0.7f)),
                ScaleMatrix = Matrix.CreateScale(2),
                BoundingVertices = new Vector3[]
                {
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, -1, -1),
                    new Vector3(-1, -1, -1)
                },
                State = new SimplifiedRigidBodyState()
                {
                    Position = new Vector3(0, 5, 0),
                    Velocity = new Vector3(0, 2, 1),
                    Orientation = Matrix.Identity,
                    AngularVelocity = new Vector3(1, 1, 2)
                }
            };

            _viewMatrix = Matrix.CreateLookAt(_cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);

            Vector3 virtualLightPosition = -20*_light.Direction;
            _lightViewMatrix = Matrix.CreateLookAt(virtualLightPosition, virtualLightPosition + _light.Direction, new Vector3(0, 1, 0));

            _lightProjectionMatrix = Matrix.CreateOrthographic(30, 30, 0.1f, 100);

            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowMapSize, ShadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            
            _gravity = new Vector3(0, -9.81f, 0);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _singleColourWithShadowEffect = Content.Load<Effect>("Effects/SingleColourWithShadowEffect");
            _shadowMapEffect = Content.Load<Effect>("Effects/ShadowMapEffect");

            _unitCubeModel = Content.Load<Model>("Models/UnitCube");
            _planeModel = Content.Load<Model>("Models/Plane");

            _plankRigidBody.Model = _unitCubeModel;
            _cubeRigidBody.Model = _unitCubeModel;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdatePhysics(gameTime.ElapsedGameTime.Milliseconds/1000.0f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // PASS 1: Draw the shadow map
            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.White);

                DrawSceneShadowMap();

            // PASS 2: Draw the scene
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

                DrawScene();

            base.Draw(gameTime);
        }

        #endregion
    }
}
