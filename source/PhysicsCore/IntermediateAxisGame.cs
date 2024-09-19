using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsCore.Simulation;
using PhysicsCore.Utility;

namespace PhysicsCore
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class IntermediateAxisGame : Game
    {
        #region Constants

        private const int ShadowMapSize = 1024;

        #endregion

        #region Fields

        private readonly GraphicsDeviceManager _graphics;

        private DirectionLight _light;
        private Vector3 _cameraPosition;

        private RigidBody _tHandleRigidBody;

        private Matrix _viewMatrix, _projectionMatrix;
        private Matrix _lightViewMatrix, _lightProjectionMatrix;

        private Effect _singleColourWithShadowEffect;
        private Effect _shadowMapEffect;

        private Model _tHandleModel;

        private RenderTarget2D _shadowMapRenderTarget;

        #endregion

        #region Constructors

        public IntermediateAxisGame()
        {
            _graphics = new GraphicsDeviceManager(this) { PreferMultiSampling = true };

            Content.RootDirectory = "Content";
        }

        #endregion

        #region Private methods

        #region Content drawing

        private void DrawSceneShadowMap()
        {
            #region T-Handle

            foreach (ModelMesh mesh in _tHandleRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _shadowMapEffect;
                }
            }

            Matrix tHandleWorldMatrix = _tHandleRigidBody.ScaleMatrix*_tHandleRigidBody.State.Orientation*Matrix.CreateTranslation(_tHandleRigidBody.State.Position);

            foreach (ModelMesh mesh in _tHandleRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["ShadowMapTechnique"];

                    effect.Parameters["LightWorldViewProjection"].SetValue(tHandleWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                }

                mesh.Draw();
            }

            #endregion
        }

        private void DrawScene()
        {
            #region T-Handle

            foreach (ModelMesh mesh in _tHandleRigidBody.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = _singleColourWithShadowEffect;
                }
            }
    
            Matrix tHandleWorldMatrix = _tHandleRigidBody.ScaleMatrix*_tHandleRigidBody.State.Orientation*Matrix.CreateTranslation(_tHandleRigidBody.State.Position);

            foreach (ModelMesh mesh in _tHandleRigidBody.Model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["SingleColourWithShadowTechnique"];

                    effect.Parameters["World"].SetValue(tHandleWorldMatrix);
                    effect.Parameters["WorldViewProjection"].SetValue(tHandleWorldMatrix*_viewMatrix*_projectionMatrix);
                    effect.Parameters["LightWorldViewProjection"].SetValue(tHandleWorldMatrix*_lightViewMatrix*_lightProjectionMatrix);
                    effect.Parameters["LightPower"].SetValue(_light.Power);
                    effect.Parameters["AmbientLightPower"].SetValue(_light.AmbientPower);
                    effect.Parameters["ShadowMapSize"].SetValue((float)ShadowMapSize);
                    effect.Parameters["LightDirection"].SetValue(_light.Direction);
                    effect.Parameters["BaseColour"].SetValue(_tHandleRigidBody.Colour.ToVector3());
                    effect.Parameters["ShadowMapTexture"].SetValue(_shadowMapRenderTarget);
                }

                mesh.Draw();
            }

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
            _cameraPosition = new Vector3(5, 5, 5);

            Random random = new Random();

            // An approximation of the body inertia tensor, assuming most of the mass is in the cross part of the handle.
            // Try changing the values along the diagonal - as long as they're all different and the Z value is between the X and Y, the effect should occur.
            Matrix tHandleInverseBodyInertiaTensor = Matrix.Invert(new Matrix(
                new Vector4(1.5f, 0, 0, 0),
                new Vector4(0, 3, 0, 0),
                new Vector4(0, 0, 2.5f, 0),
                new Vector4(0, 0, 0, 1)));

            _tHandleRigidBody = new RigidBody()
            {
                Colour = new Color(new Vector3(0.7f, 0.7f, 0.2f)),
                ScaleMatrix = Matrix.Identity,
                Mass = 1,
                InverseBodyInertiaTensor = tHandleInverseBodyInertiaTensor,
                BoundingVertices = new Vector3[] { },
                State = new RigidBodyState()
                {
                    Position = Vector3.Zero,
                    Velocity = Vector3.Zero,
                    Orientation = Matrix.Identity,
                    AngularMomentum = new Vector3(0.05f*(2*random.NextSingle() - 1), 0.05f*(2*random.NextSingle() - 1), 20)
                }
            };

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
            _singleColourWithShadowEffect = Content.Load<Effect>("Effects/SingleColourWithShadowEffect");
            _shadowMapEffect = Content.Load<Effect>("Effects/ShadowMapEffect");

            _tHandleModel = Content.Load<Model>("Models/THandle");

            _tHandleRigidBody.Model = _tHandleModel;
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
            _tHandleRigidBody.ApplyPhysics(Vector3.Zero, Vector3.Zero, gameTime.ElapsedGameTime.Milliseconds/1000.0f);

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
