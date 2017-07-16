using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
    enum TileContent
    { Grass, Road, Villge, Water, Lake, Forest, Farm, Paddock, Mine, Shop };

    class AI
    {
        // intrinsic
        Vector2 position;

        // specific
        float hunger = 0f;
        float thirst = 0f;
        float tiredness = 0f;

    }

    class WorldTile
    {
        public Color colour = Color.White;
        public bool isWalkable = false;
    }

    class World
    {
        Dictionary<Color, TileContent> mMapDefinition; // maps colours to the meaning of that colour
        Dictionary<String, Texture2D> mTextureAtlas;
        Texture2D mPixel;
        //Texture2D mInputImage;
        RenderTarget2D mMap;
        GraphicsDevice mGraphicsDevice;

        WorldTile[,] mWorld; // = new WorldTile[64, 64];

        int mWorldSizeX = 0;
        int mWorldSizeY = 0;
        int mGridSize = 8;
        int mWinWidth = 0;
        int mWinHeight = 0;

        public void LoadContent(GraphicsDevice graphicsDevice, Dictionary<String, Texture2D> textureAtlas)
        {
            // set private members
            mGraphicsDevice = graphicsDevice;
            mTextureAtlas = textureAtlas;

            // create a 'pixel' texture
            mPixel = new Texture2D(mGraphicsDevice, mGridSize, mGridSize);

            // convert input texture into our useful 2d array
            Texture2D inputImage = mTextureAtlas["test_village"];

            mWorldSizeX = inputImage.Width;
            mWorldSizeY = inputImage.Height;
            mWorld = new WorldTile[mWorldSizeX, mWorldSizeY];

            Color[] colors1D = new Color[mWorldSizeX * mWorldSizeY];
            inputImage.GetData<Color>(colors1D);
            for (int y = 0; y < mWorldSizeY; y++)
            {
                for (int x = 0; x < mWorldSizeX; x++)
                {
                    mWorld[x, y] = new WorldTile();
                    mWorld[x, y].colour = colors1D[x + y * mWorldSizeX];
                }
            }

            // populate the texture with white
            List<Color> colorList = new List<Color>();
            for (int i = 0; i < mGridSize * mGridSize; ++i)
                colorList.Add(Color.White);
            mPixel.SetData(colorList.ToArray());

            mWinWidth = mGraphicsDevice.PresentationParameters.BackBufferWidth;
            mWinHeight = mGraphicsDevice.PresentationParameters.BackBufferHeight;

            // map
            mMap = new RenderTarget2D(
                mGraphicsDevice,
                mWinWidth,
                mWinHeight);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < mWorldSizeY; y++)
            {
                for (int x = 0; x < mWorldSizeX; x++)
                {
                    spriteBatch.Draw(
                        mPixel,
                        new Vector2(x * mGridSize, y * mGridSize),
                        null,
                        null,
                        null,
                        0f,
                        Vector2.One,
                        mWorld[x, y].colour,
                        SpriteEffects.None,
                        0f);
                }
            }

            // scale 8 times
            //spriteBatch.Draw(mInputImage, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 0);
        }
    }
}
