﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Endeavour
{
	public class FrameRateCounter : DrawableGameComponent
	{
		ContentManager content;
		SpriteBatch spriteBatch;
		SpriteFont spriteFont;

		int frameRate = 0;
		int frameCounter = 0;
		TimeSpan elapsedTime = TimeSpan.Zero;


		public FrameRateCounter(Game game)
			: base(game)
		{
			content = new ContentManager(game.Services);
			content.RootDirectory = "Content";
		}


		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			spriteFont = content.Load<SpriteFont>("Fonts\\arial");
		}


		protected override void UnloadContent()
		{
			content.Unload();
		}


		public override void Update(GameTime gameTime)
		{
			elapsedTime += gameTime.ElapsedGameTime;

			if (elapsedTime > TimeSpan.FromSeconds(1))
			{
				elapsedTime -= TimeSpan.FromSeconds(1);
				frameRate = frameCounter;
				frameCounter = 0;
			}
		}


		public override void Draw(GameTime gameTime)
		{
			frameCounter++;

			var fps = string.Format("fps: {0}", frameRate);

			spriteBatch.Begin();

			spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
			spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);

			spriteBatch.End();
		}
	}
}
