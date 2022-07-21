using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Endeavour
{
	class WorldGenTile
	{
		public float height = 0;
		public float water = 0;

		public Vector2 mGradient = Vector2.Zero;
		//public int

	}

	class WorldGen
	{
		// is a reference to Game1::mTextureAtlas
		Dictionary<string, Texture2D> mTextureAtlas;

		Vector2 ball;
		Vector2 ballDirection;
		float ballVelocity;

		Texture2D mPixel;
		RenderTarget2D mMap;
		WorldGenTile[,] mWorld;
		WorldGenTile[,] mWorldBackBuffer;

		OpenSimplexNoise osn;
		GraphicsDevice mGraphicsDevice;

		int mGridWidth = 0;
		int mGridHeight = 0;
		int mWinWidth = 0;
		int mWinHeight = 0;
		int mQuantisationLevel = 40;
		float mCullLevel = 0.1f;
		int mGridSize = 8;

		Vector2 mWinCentre;
		Vector2 mGridCentre;

		public void LoadContent(GraphicsDevice graphicsDevice, Dictionary<string, Texture2D> textureAtlas)
		{
			// set private members
			mGraphicsDevice = graphicsDevice;
			mTextureAtlas = textureAtlas;

			osn = new OpenSimplexNoise(2);

			// create a 'pixel' texture
			mPixel = new Texture2D(mGraphicsDevice, mGridSize, mGridSize);


			// populate the texture with white
			var colorList = new List<Color>();
			for (var i = 0; i < mGridSize * mGridSize; ++i)
			{
				colorList.Add(Color.White);
			}

			mPixel.SetData(colorList.ToArray());

			mWinWidth = mGraphicsDevice.PresentationParameters.BackBufferWidth;
			mWinHeight = mGraphicsDevice.PresentationParameters.BackBufferHeight;

			mGridWidth = mWinWidth / mGridSize;
			mGridHeight = mWinHeight / mGridSize;

			mWinCentre = new Vector2(mWinWidth / 2, mWinHeight / 2);
			mGridCentre = new Vector2(mGridWidth / 2, mGridHeight / 2);

			ball = mWinCentre;

			// map
			mMap = new RenderTarget2D(
				mGraphicsDevice,
				mWinWidth,
				mWinHeight);

			// init world grid
			mWorld = new WorldGenTile[mWinWidth, mWinHeight];
			mWorldBackBuffer = new WorldGenTile[mWinWidth, mWinHeight];

			for (var y = 0; y < mWinHeight; y++)
			{
				for (var x = 0; x < mWinWidth; x++)
				{
					mWorld[x, y] = new WorldGenTile();
					mWorldBackBuffer[x, y] = new WorldGenTile();
				}
			}

			// run this async
			GenWorld();
			GenRivers();
			DoGradients();

			// draw game world
			DrawWorldToTexture();
		}

		public void ProcessInput(GameTime gameTime)
		{
			var ms = Mouse.GetState();
			if (ms.LeftButton == ButtonState.Pressed)
			{
				ball = ms.Position.ToVector2();
			}

			if ((int)gameTime.TotalGameTime.TotalMilliseconds % 10 == 0)
			{
				ApplyKernel();
				DrawWorldToTexture();
			}
		}

		void GenRivers()
		{
			//mWorld[(int)mGridCentre.X, (int)mGridCentre.Y].water = 1f;

			// don't look at outside boundary tiles
			for (var y = 1; y < mGridHeight - 1; y++)
			{
				for (var x = 1; x < mGridWidth - 1; x++)
				{
					// find lowest tile compared to this one
					//Tile thisTile = mWorld[x, y];
					//if (thisTile.height == 0)
					//    thisTile.water = 0f;
					//else
					//    thisTile.water = 0.9f;

				}
			}
		}

		void DoGradients()
		{
			// add a water source
			mWorld[(int)mWinCentre.X / mGridSize, (int)mWinCentre.Y / mGridSize].water = 2f;


			for (var y = 1; y < mGridHeight - 1; y++)
			{
				for (var x = 1; x < mGridWidth - 1; x++)
				{
					// compute gradients in each direction
					var dx = mWorld[x - 1, y].height - mWorld[x + 1, y].height;
					var dy = mWorld[x, y - 1].height - mWorld[x, y + 1].height;
					var dir = ToPolar(new Vector2(dx, dy));
					mWorld[x, y].mGradient = dir;
				}
			}
		}

		Vector2 ToPolar(Vector2 vec)
		{
			return new Vector2(vec.Length(), (float)Math.Atan2(vec.Y, vec.X));
		}

		Vector2 ToCartesian(Vector2 vec)
		{
			return new Vector2(vec.X * (float)Math.Cos(vec.Y), vec.X * (float)Math.Sin(vec.Y));
		}

		// Convolution, aka applying a kernel
		void ApplyKernel()
		{
			// move the ball by the gradient of it's current tile
			var d = ToCartesian(mWorld[(int)(ball.X / mGridSize), (int)(ball.Y / mGridSize)].mGradient);
			var ballD = d;
			if (ballD != Vector2.Zero)
			{
				ballD.Normalize();
				ballDirection = ballD;
			}
			var ballAcc = d.Length() * 4;
			ballVelocity += ballAcc;
			var friction = ballVelocity * 0.02f;
			ballVelocity -= friction;
			ball += ballVelocity * ballDirection;


			ballVelocity = MathHelper.Clamp(ballVelocity, 0f, 100f);

			//float damping = 0.1f;
			//ballVelocity -= damping;

			// do water stuff

			// add a water source
			mWorld[(int)mGridCentre.X, (int)mGridCentre.Y].water = 1f;

			for (var y = 1; y < mGridHeight - 1; y++)
			{
				for (var x = 1; x < mGridWidth - 1; x++)
				{
					// clear backbuffer
					mWorldBackBuffer[x, y].water = 0;
					mWorldBackBuffer[x, y].height = mWorld[x, y].height;
					mWorldBackBuffer[x, y].mGradient = mWorld[x, y].mGradient;
					var thisTile = mWorld[x, y];

					var water = thisTile.water / 4f;

					// up
					if (thisTile.mGradient.Y >= 1 * Math.PI / 4 && thisTile.mGradient.Y < 3 * Math.PI / 4)
					{
						mWorldBackBuffer[x - 1, y].water += water;
						thisTile.water -= water;
					}
					// left
					if (thisTile.mGradient.Y >= 3 * Math.PI / 4 && thisTile.mGradient.Y < 5 * Math.PI / 4)
					{
						mWorldBackBuffer[x, y - 1].water += water;
						thisTile.water -= water;
					}
					// down
					if (thisTile.mGradient.Y >= 5 * Math.PI / 4 && thisTile.mGradient.Y < 7 * Math.PI / 4)
					{
						mWorldBackBuffer[x + 1, y].water += water;
						thisTile.water -= water;
					}
					//right
					if (thisTile.mGradient.Y >= 7 * Math.PI / 4 || thisTile.mGradient.Y < 1 * Math.PI / 4)
					{
						mWorldBackBuffer[x, y + 1].water += water;
						thisTile.water -= water;
					}
				}
			}

			// switch buffers
			mWorld = mWorldBackBuffer;
		}

		// Convolution, aka applying a kernel
		void ApplyKernel2()
		{
			//mWorldBackBuffer = mWorld;

			//float[,] identityKernel = new float[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
			//float[,] edgeKernel1 = new float[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
			//float[,] blurKernel = new float[,] { { 1, 1, 1 }, { 1, 1, 1}, { 1, 1, 1} };
			//float[,] sharpenKernel = new float[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };

			//float[,] kernel = identityKernel;

			mWorld[(int)mWinCentre.X / mGridSize, (int)mWinCentre.Y / mGridSize].water = 2f;


			for (var y = 1; y < mGridHeight - 1; y++)
			{
				for (var x = 1; x < mGridWidth - 1; x++)
				{
					mWorldBackBuffer[x, y].water = 0;
					mWorldBackBuffer[x, y].height = mWorld[x, y].height;

					// if this is the ocean, make it a sink
					// and do not convolute it
					if (mWorld[x, y].height == 0)
					{
						mWorld[x, y].water = 0;
						//mWorld[x+1, y].water = 0;
						//mWorld[x-1, y].water = 0;
						//mWorld[x, y+1].water = 0;
						//mWorld[x, y-1].water = 0;
						continue;
					}

					//float accumulator = 0;

					// add rain
					//mWorld[x, y].water += mWorld[x, y].height / 8;
					var thisTile = mWorld[x, y];

					var water = (thisTile.water) / 4f;

					// spread water between the four tiles
					if (thisTile.height > mWorld[x - 1, y].height)
					{
						mWorldBackBuffer[x - 1, y].water += water;
						thisTile.water -= water;
					}

					if (thisTile.height > mWorld[x + 1, y].height)
					{
						mWorldBackBuffer[x + 1, y].water += water;
						thisTile.water -= water;
					}

					if (thisTile.height > mWorld[x, y - 1].height)
					{
						mWorldBackBuffer[x, y - 1].water += water;
						thisTile.water -= water;
					}

					if (thisTile.height > mWorld[x, y + 1].height)
					{
						mWorldBackBuffer[x, y + 1].water += water;
						thisTile.water -= water;
					}

					// overflow
					//if (thisTile.water > 1f)
					//{
					//    // counts the number of tiles we used for overflow
					//    int count = 0;

					//    // overflow amount
					//    float overflow = thisTile.water - 1f;

					//    if (mWorld[x - 1, y].height + mWorld[x - 1, y].water < thisTile.height + thisTile.water)
					//    {
					//        mWorldBackBuffer[x - 1, y].water += overflow;
					//        count++;
					//    }
					//    if (mWorld[x + 1, y].height + mWorld[x + 1, y].water < thisTile.height + thisTile.water)
					//    {
					//        mWorldBackBuffer[x + 1, y].water += overflow;
					//        count++;
					//    }
					//    if (mWorld[x, y - 1].height + mWorld[x, y - 1].water < thisTile.height + thisTile.water)
					//    {
					//        mWorldBackBuffer[x, y - 1].water += overflow;
					//        count++;
					//    }
					//    if (mWorld[x, y + 1].height + mWorld[x, y + 1].water < thisTile.height + thisTile.water)
					//    {
					//        mWorldBackBuffer[x, y + 1].water += overflow;
					//        count++;
					//    }

					//    // finally subtract any realised overflows
					//    thisTile.water -= count * overflow;
					//}

					// convolute
					//for (int ky = 0; ky < kernel.GetLength(0); ky++)
					//{
					//    for (int kx = 0; kx < kernel.GetLength(1); kx++)
					//    {
					//        Tile thisTile2 = mWorldBackBuffer[x - (kx - 1), y - (ky - 1)];
					//        accumulator +=
					//            kernel[kx, ky] *
					//            thisTile2.water *
					//            (1 - (float)Math.Pow(thisTile2.height, 2)); // we do 1 minus to get water to flow down instead of up
					//    }
					//}



					// normalise (if necessary)
					//accumulator /= 9;


					//if (accumulator < 0.05f)
					//    accumulator = 0;

					// update world
					//mWorldBackBuffer[x, y].height = mWorld[x, y].height;
					// mWorldBackBuffer[x, y].water = accumulator;
				}
			}

			// switch buffers
			mWorld = mWorldBackBuffer;
		}

		static T Clamp<T>(T lhs, T rhs) where T : System.IComparable<T>
		{
			return (lhs.CompareTo(rhs) > 0) ? rhs : lhs;
		}

		void DrawWorldToTexture()
		{
			mGraphicsDevice.SetRenderTarget(mMap);
			var sb = new SpriteBatch(mGraphicsDevice);
			sb.Begin();

			for (var y = 0; y < mGridHeight; y++)
			{
				for (var x = 0; x < mGridWidth; x++)
				{
					var currentPosition = new Vector2(x * mGridSize, y * mGridSize);
					var noise = mWorld[x, y].height;

					{
						var c = new Color(noise, noise, noise);
						//Color c = new Color(noise, 0, blue);
						sb.Draw(mPixel, currentPosition, c);

						// draw gradients
						//var tex = mTextureAtlas["arrow"];
						//if (mWorld[x, y].mGradient.X != 0)
						//{
						//	sb.Draw(
						//		tex,
						//		currentPosition + new Vector2(mGridSize / 2), // position
						//		null,
						//		null,
						//		new Vector2(tex.Width / 2, tex.Height / 2), //origin - set to centre of texture
						//		mWorld[x, y].mGradient.Y, // rotation
						//		Vector2.One, //new Vector2(mWorld[x,y].mGradient.X * 10), // scale
						//		Color.LightGoldenrodYellow,
						//		SpriteEffects.None,
						//		0);
						//}
					}

					// draw water
					//if (mWorld[x, y].water > 0.01f)
					//{
					//	//float blue = Clamp(mWorld[x, y].water, 1f);
					//	var blue = (mWorld[x, y].water > 1f) ? 1f : mWorld[x, y].water;

					//	//Color c = new Color(noise, noise, noise);
					//	var c = new Color(0, 0, blue, 1f);
					//	sb.Draw(mPixel, currentPosition, c);
					//}
				}
			}

			{
				// draw ball
				sb.Draw(mPixel, ball - new Vector2(mGridSize / 2), Color.Red);
			}

			sb.End();
			mGraphicsDevice.SetRenderTarget(null); // reset
		}

		void GenWorld()
		{
			for (var y = 0; y < mGridHeight; y++)
			{
				for (var x = 0; x < mGridWidth; x++)
				{
					const int octave = 8;
					float noise = 0;
					float divisor = 0;
					const float scale = 40;

					for (var i = 0; i < octave; i++)
					{
						var pow = (float)Math.Pow(2, i);
						divisor += 1 / pow;
						var ff = (float)osn.eval(x / scale * pow, y / scale * pow) / pow;
						noise += ff;
					}
					noise /= (divisor);
					noise += 1; // bring from -1,1 to 0,2
					noise /= 2; // bring from 0,2 to 0,1

					// now, compute how far it is from middle and if its close to the edge
					// then make it black
					var currentPosition = new Vector2(x * mGridSize, y * mGridSize);
					var xRatio = 1 - (float)Math.Pow((Math.Abs(mWinCentre.X - currentPosition.X) / mWinCentre.X), 1);
					var yRatio = 1 - (float)Math.Pow((Math.Abs(mWinCentre.Y - currentPosition.Y) / mWinCentre.Y), 1);
					noise *= (yRatio * xRatio);

					// quantise the noise into levels
					if (mQuantisationLevel != 0)
					{
						noise *= mQuantisationLevel;
						noise = (int)noise;
						noise /= mQuantisationLevel;
					}


					// cull anything below a certain level
					if (noise < mCullLevel)
					{
						noise = 0;
					}
					//noise *= noise;
					// draw the pixel
					mWorld[x, y].height = noise;
				}
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(mMap, Vector2.Zero, Color.White);
		}
	}
}
