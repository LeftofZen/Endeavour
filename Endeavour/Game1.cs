using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace Endeavour
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;

        Dictionary<String, Texture2D> mTextureAtlas;

        //WorldGen mWorldGen;
        //World mWorld;

        Render mRender;

        // Keyboard states used to determine key presses
        InputState mInputState;
        
        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            mGraphics.IsFullScreen = false;
            mGraphics.PreferredBackBufferWidth = 1440;
            mGraphics.PreferredBackBufferHeight = 810;

            mTextureAtlas = new Dictionary<string, Texture2D>();
            mInputState = new InputState();

            this.IsMouseVisible = true;
            

            Content.RootDirectory = "Content";

            // enable this when we upgrade to monogame 3.6+
            //Components.Add(new FrameRateCounter(this));

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //mWorldGen = new WorldGen();
            //mWorld = new World();

            mRender = new Render(this);
            mRender.Initialize(mGraphics);

            base.Initialize();
        }

        void LoadImage(String name)
        {
            mTextureAtlas.Add(name, Content.Load<Texture2D>("Sprites\\" + name));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            LoadImage("test_village");
            LoadImage("arrow");


            //mWorld.LoadContent(GraphicsDevice, mTextureAtlas);
            //mWorldGen.LoadContent(GraphicsDevice, mTextureAtlas);
            mRender.LoadContent(mTextureAtlas);

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
            // Read the current state of the keyboard and gamepad and store it
            mInputState.Update(gameTime);



            if (   mInputState.mCurrentGamePadState.Buttons.Back == ButtonState.Pressed
                || mInputState.mCurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Update state here
            //
            //if (currentGamePadState.IsConnected)
            { }

            // Process keyboard
            { }

            // Process mouse
            { }

            //mWorldGen.ProcessInput();
            //mWorld.ProcessInput(mInputState);
            mRender.Update(gameTime);
            mRender.ProcessInput(mInputState);

            if (!mInputState.mCurrentKeyboardState.IsKeyDown(Keys.Space) && mInputState.mPreviousKeyboardState.IsKeyDown(Keys.Space))
            {
            }


            mInputState.UpdatePrevious();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // PointClamp means nearest-neighbour interpolation
            mSpriteBatch.Begin(
                SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            //mWorldGen.Draw(gameTime, mSpriteBatch);
            //mWorld.Draw(gameTime, mSpriteBatch);
            mRender.Draw(gameTime);

            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
