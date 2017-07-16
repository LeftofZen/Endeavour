using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace TestGame1
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
        World mWorld;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        //Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        Vector2 keyVec;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            mGraphics.IsFullScreen = false;
            mGraphics.PreferredBackBufferWidth = 1440;
            mGraphics.PreferredBackBufferHeight = 810;

            mTextureAtlas = new Dictionary<string, Texture2D>();

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            //mShip = new Ship();
            keyVec = new Vector2();

            currentMouseState = new MouseState();
            previousMouseState = currentMouseState;

            currentKeyboardState = new KeyboardState();
            previousKeyboardState = currentKeyboardState;

            currentGamePadState = new GamePadState();
            previousGamePadState = currentGamePadState;
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
            mWorld = new World();

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

            mWorld.LoadContent(GraphicsDevice, mTextureAtlas);
            //mWorldGen.LoadContent(GraphicsDevice, mTextureAtlas);

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
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            if (currentGamePadState.Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Update state here
            //
            if (currentGamePadState.IsConnected)
            {
            }

            // Process keyboard
            {

                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    keyVec.X -= 1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    keyVec.X += 1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Up))
                {
                    keyVec.Y -= 1;
                }
                if (currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    keyVec.Y += 1;
                }

                //mWorldGen.ProcessInput();
                if (!currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyDown(Keys.Space))
                {
                }

            }


            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

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
            mWorld.Draw(gameTime, mSpriteBatch);

            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
