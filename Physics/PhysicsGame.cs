using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics.Simulation;
using System;

namespace Physics
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class PhysicsGame : Game
    {
        #region Fields

        private readonly GraphicsDeviceManager _graphics;

        private Vector3 _cameraPosition, _lightDirection;

        private Matrix _viewMatrix, _projectionMatrix;
        private Matrix _lightViewMatrix, _lightProjectionMatrix;

        private RigidBody _cubeRigidBody;

        private Effect _singleColourEffect;
        private Effect _shadowMapEffect;

        private Model _cubeModel, _planeModel;

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

        private Matrix SkewSymmetricMatrix(Vector3 vector)
        {
            Matrix result = Matrix.Identity;
            
            result[0, 0] = 0;
            result[0, 1] = -vector.Z;
            result[0, 2] = vector.Y;
            result[1, 0] = vector.Z;
            result[1, 1] = 0;
            result[1, 2] = -vector.X;
            result[2, 0] = -vector.Y;
            result[2, 1] = vector.X;
            result[2, 2] = 0;

            return result;
        }

        private Matrix OrthonormaliseMatrix(Matrix matrix)
        {
            Vector3 up = Vector3.Normalize(matrix.Up);
            Vector3 forward = Vector3.Normalize(Vector3.Cross(up, matrix.Right));
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, up));

            Matrix result = Matrix.Identity;
            result.Up = up;
            result.Forward = forward;
            result.Right = right;

            return result;
        }

        #region Physics

        private void UpdatePhysics(float timeDelta)
        {
            // Calculate forces
            Vector3 collisionForce = new Vector3(0, 0, 0);
            Vector3 collisionTorque = new Vector3(0, 0, 0);
            foreach (Vector3 vertexPosition in _cubeRigidBody.BoundingVertices)
            {
                Vector3 vertexLocalPosition = Vector3.Transform(vertexPosition, _cubeRigidBody.State.Orientation);
                Vector3 vertexWorldPosition = _cubeRigidBody.State.Position + vertexLocalPosition;
                float penetrationDepth = 0.004f - vertexWorldPosition.Y;
                if (penetrationDepth >= 0)
                {
                    Vector3 collisionNormal = new Vector3(0, 1, 0);
                    Vector3 vertexVelocity = _cubeRigidBody.State.Velocity + Vector3.Cross(_cubeRigidBody.State.AngularVelocity, vertexLocalPosition);
                    Vector3 vertexCollisionForce = 100*collisionNormal*penetrationDepth - 5*vertexVelocity;

                    collisionForce += vertexCollisionForce;
                    collisionTorque += Vector3.Cross(vertexLocalPosition, vertexCollisionForce);
                }
            }

            Vector3 acceleration = new Vector3(0, -1.62f, 0) + collisionForce;
            Vector3 angularAcceleration = collisionTorque;
            //Vector3 acceleration = new Vector3(0, 0, 0);
            //Vector3 angularAcceleration = new Vector3(0, 0, 0);

            // Calculate updated state
            _cubeRigidBody.State.Position += _cubeRigidBody.State.Velocity*timeDelta;
            _cubeRigidBody.State.Velocity += acceleration*timeDelta;

            _cubeRigidBody.State.Orientation += Matrix.Transpose(SkewSymmetricMatrix(_cubeRigidBody.State.AngularVelocity)*Matrix.Transpose(_cubeRigidBody.State.Orientation))*timeDelta;
            _cubeRigidBody.State.Orientation = OrthonormaliseMatrix(_cubeRigidBody.State.Orientation);

            _cubeRigidBody.State.AngularVelocity += angularAcceleration*timeDelta;
        }

        #endregion

        #region Content drawing

        private void DrawSceneShadowMap()
        {
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
                    effect.Parameters["LightPower"].SetValue(1.0f);
                    effect.Parameters["AmbientLightPower"].SetValue(0.2f);
                    effect.Parameters["SpecularExponent"].SetValue(32.0f);
                    effect.Parameters["ShadowMapSize"].SetValue(1024.0f);
                    effect.Parameters["CameraPosition"].SetValue(_cameraPosition);
                    effect.Parameters["LightDirection"].SetValue(_lightDirection);
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
                    effect.Parameters["LightPower"].SetValue(1.0f);
                    effect.Parameters["AmbientLightPower"].SetValue(0.2f);
                    effect.Parameters["SpecularExponent"].SetValue(32.0f);
                    effect.Parameters["ShadowMapSize"].SetValue(1024.0f);
                    effect.Parameters["CameraPosition"].SetValue(_cameraPosition);
                    effect.Parameters["LightDirection"].SetValue(_lightDirection);
                    effect.Parameters["BaseColour"].SetValue(new Vector3(0.5f, 0.5f, 1));
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }
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
            _cameraPosition = new Vector3(10, 10, 10);
            _lightDirection = new Vector3(1, -1, -1);

            _viewMatrix = Matrix.CreateLookAt(_cameraPosition, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), GraphicsDevice.Viewport.AspectRatio, 0.1f, 100);

            Vector3 normalisedLightDirection = Vector3.Normalize(_lightDirection);
            Vector3 virtualLightPosition = -20*normalisedLightDirection;
            _lightViewMatrix = Matrix.CreateLookAt(virtualLightPosition, virtualLightPosition + normalisedLightDirection, new Vector3(0, 1, 0));

            _lightProjectionMatrix = Matrix.CreateOrthographic(30, 30, 0.1f, 100);

            _cubeRigidBody = new RigidBody()
            {
                BoundingVertices = new Vector3[]
                {
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, -1,- 1),
                    new Vector3(-1, -1, -1)
                },
                State = new RigidBodyState()
                {
                    Position = new Vector3(0, 5, 0),
                    Velocity = new Vector3(0, 2, 0),
                    Orientation = Matrix.Identity,
                    AngularVelocity = new Vector3(1, 1, 2)
                }
            };

            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 1024, 1024, false, SurfaceFormat.Single, DepthFormat.Depth24);
            
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
