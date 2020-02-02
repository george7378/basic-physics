using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsCore.Simulation;
using PhysicsCore.Utility;

namespace PhysicsCore
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PhysicsGame : Game
    {
        #region Constants

        private const int ShadowMapSize = 1024;

        #endregion

        #region Fields

        private readonly GraphicsDeviceManager _graphics;

        private DirectionLight _light;

        private RigidBody _plankRigidBody;
        private RigidBodySimplified _cubeRigidBody;

        private Vector3 _cameraPosition;
        private Vector3 _gravity;

        private Matrix _viewMatrix, _projectionMatrix;
        private Matrix _lightViewMatrix, _lightProjectionMatrix;

        private Effect _singleColourEffect;
        private Effect _shadowMapEffect;

        private Model _plankModel, _cubeModel, _planeModel;

        private RenderTarget2D _shadowMapRenderTarget;

        #endregion

        #region Constructors

        public PhysicsGame()
        {
            _graphics = new GraphicsDeviceManager(this) { PreferMultiSampling = true };

            Content.RootDirectory = "Content";
        }

        #endregion

        #region Private methods

        #region Content drawing

        private void DrawSceneShadowMap()
        {
            // Plank
            foreach (ModelMesh mesh in _plankModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            Matrix plankWorldMatrix = _plankRigidBody.State.Orientation*Matrix.CreateTranslation(_plankRigidBody.State.Position);

            foreach (ModelMesh mesh in _plankModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(plankWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            // Cube
            foreach (ModelMesh mesh in _cubeModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            Matrix cubeWorldMatrix = _cubeRigidBody.State.Orientation*Matrix.CreateTranslation(_cubeRigidBody.State.Position);

            foreach (ModelMesh mesh in _cubeModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(cubeWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            // Plane
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
        }

        private void DrawScene()
        {
            // Plank
            foreach (ModelMesh mesh in _plankModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourEffect;
                }
            }
    
            Matrix plankWorldMatrix = _plankRigidBody.State.Orientation*Matrix.CreateTranslation(_plankRigidBody.State.Position);

            foreach (ModelMesh mesh in _plankModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourTechnique"];

                    effect.Parameters["World"].SetValue(plankWorldMatrix);
                    effect.Parameters["WorldViewProjection"].SetValue(plankWorldMatrix*_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(plankWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(new Vector3(0.7f, 0.2f, 0.2f));
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

            // Cube
            foreach (ModelMesh mesh in _cubeModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourEffect;
                }
            }
    
            Matrix cubeWorldMatrix = _cubeRigidBody.State.Orientation*Matrix.CreateTranslation(_cubeRigidBody.State.Position);

            foreach (ModelMesh mesh in _cubeModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourTechnique"];

                    effect.Parameters["World"].SetValue(cubeWorldMatrix);
                    effect.Parameters["WorldViewProjection"].SetValue(cubeWorldMatrix*_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(cubeWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(new Vector3(0.7f, 0.7f, 0.7f));
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

            // Plane
            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourEffect;
                }
            }

            foreach (ModelMesh mesh in _planeModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourTechnique"];

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
        }

        #endregion

        #region Misc.

        private void UpdatePhysics(float timeDelta)
        {
            // Plank
            Vector3 plankCollisionForce = Vector3.Zero;
            Vector3 plankCollisionTorque = Vector3.Zero;
            foreach (Vector3 vertexPosition in _plankRigidBody.BoundingVertices)
            {
                Vector3 vertexLocalPosition = Vector3.Transform(vertexPosition, _plankRigidBody.State.Orientation);
                Vector3 vertexWorldPosition = _plankRigidBody.State.Position + vertexLocalPosition;

                float penetrationDepth = 0.004f - vertexWorldPosition.Y;
                if (penetrationDepth >= 0)
                {
                    Vector3 collisionNormal = Vector3.UnitY;
                    Vector3 vertexVelocity = _plankRigidBody.State.Velocity + Vector3.Cross(_plankRigidBody.State.AngularVelocity, vertexLocalPosition);
                    Vector3 vertexCollisionForce = _plankRigidBody.Mass*(100*collisionNormal*penetrationDepth - 5*vertexVelocity);

                    plankCollisionForce += vertexCollisionForce;
                    plankCollisionTorque += Vector3.Cross(vertexLocalPosition, vertexCollisionForce);
                }
            }

            _plankRigidBody.ApplyPhysics(_plankRigidBody.Mass*_gravity + plankCollisionForce, plankCollisionTorque, timeDelta);

            // Cube
            Vector3 cubeCollisionForce = Vector3.Zero;
            Vector3 cubeCollisionTorque = Vector3.Zero;
            foreach (Vector3 vertexPosition in _cubeRigidBody.BoundingVertices)
            {
                Vector3 vertexLocalPosition = Vector3.Transform(vertexPosition, _cubeRigidBody.State.Orientation);
                Vector3 vertexWorldPosition = _cubeRigidBody.State.Position + vertexLocalPosition;

                float penetrationDepth = 0.004f - vertexWorldPosition.Y;
                if (penetrationDepth >= 0)
                {
                    Vector3 collisionNormal = Vector3.UnitY;
                    Vector3 vertexVelocity = _cubeRigidBody.State.Velocity + Vector3.Cross(_cubeRigidBody.State.AngularVelocity, vertexLocalPosition);
                    Vector3 vertexCollisionForce = 100*collisionNormal*penetrationDepth - 5*vertexVelocity;

                    cubeCollisionForce += vertexCollisionForce;
                    cubeCollisionTorque += Vector3.Cross(vertexLocalPosition, vertexCollisionForce);
                }
            }

            _cubeRigidBody.ApplyPhysics(_gravity + cubeCollisionForce, cubeCollisionTorque, timeDelta);
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

            _plankRigidBody = new RigidBody()
            {
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
            Matrix plankBodyInertiaTensor = Matrix.Identity;
            plankBodyInertiaTensor[0, 0] = _plankRigidBody.Mass/12*(6*6 + 0.4f*0.4f);
            plankBodyInertiaTensor[1, 1] = _plankRigidBody.Mass/12*(6*6 + 2*2);
            plankBodyInertiaTensor[2, 2] = _plankRigidBody.Mass/12*(2*2 + 0.4f*0.4f);
            _plankRigidBody.InverseBodyInertiaTensor = Matrix.Invert(plankBodyInertiaTensor);

            _cubeRigidBody = new RigidBodySimplified()
            {
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
                State = new RigidBodySimplifiedState()
                {
                    Position = new Vector3(0, 5, 0),
                    Velocity = new Vector3(0, 2, 1),
                    Orientation = Matrix.Identity,
                    AngularVelocity = new Vector3(1, 1, 2)
                }
            };

            _cameraPosition = new Vector3(10, 10, 10);
            _gravity = new Vector3(0, -9.81f, 0);

            _viewMatrix = Matrix.CreateLookAt(_cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);

            Vector3 virtualLightPosition = -20*_light.Direction;
            _lightViewMatrix = Matrix.CreateLookAt(virtualLightPosition, virtualLightPosition + _light.Direction, new Vector3(0, 1, 0));

            _lightProjectionMatrix = Matrix.CreateOrthographic(30, 30, 0.1f, 100);

            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowMapSize, ShadowMapSize, false, SurfaceFormat.Single, DepthFormat.Depth24);
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _singleColourEffect = Content.Load<Effect>("Effects/SingleColourEffect");
            _shadowMapEffect = Content.Load<Effect>("Effects/ShadowMapEffect");

            _plankModel = Content.Load<Model>("Models/Plank");
            _cubeModel = Content.Load<Model>("Models/Cube");
            _planeModel = Content.Load<Model>("Models/Plane");
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
