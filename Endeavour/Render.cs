using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Endeavour
{
	class Render
	{
		Dictionary<string, Texture2D> mTextureAtlas;
		GraphicsDevice mGraphicsDevice;
		GraphicsDeviceManager mGraphicsDeviceManager;

		// The assignment of effect.View and effect.Projection
		// are nearly identical to the code in the Model drawing code.
		Vector3 mCameraPosition = new Vector3(0, -40, 40);
		Vector3 mCameraLookAtVector = Vector3.Zero;
		Vector3 mCameraUpVector = Vector3.UnitZ;
		int mCameraSpeed = 1;
		Random mRandom = new Random();

		// 3d graphics shit
		// https://developer.xamarin.com/guides/cross-platform/game_development/monogame/3d/part2/
		VertexPositionColor[] mVerts;
		//int[] mIndices;

		VertexBuffer vb;
		IndexBuffer ib;

		// new code:
		BasicEffect mEffect;

		public Render(Game game) { }

		public void Update(GameTime gameTime)
		{ }

		public void ProcessInput(InputState input)
		{
			var lookAtVec = mCameraPosition - mCameraLookAtVector;
			lookAtVec.Normalize();
			if (float.IsNaN(lookAtVec.X) || float.IsNaN(lookAtVec.Y) || float.IsNaN(lookAtVec.Z))
			{
				lookAtVec = Vector3.Zero;
			}

			var planarLookAtVec = lookAtVec;
			planarLookAtVec.Z = 0;

			if (input.mCurrentKeyboardState.IsKeyDown(Keys.Right))
			{
				mCameraPosition += Vector3.Right * mCameraSpeed;
				mCameraLookAtVector += Vector3.Right * mCameraSpeed;
			}
			if (input.mCurrentKeyboardState.IsKeyDown(Keys.Left))
			{
				mCameraPosition += Vector3.Left * mCameraSpeed;
				mCameraLookAtVector += Vector3.Left * mCameraSpeed;
			}

			if (input.mCurrentKeyboardState.IsKeyDown(Keys.Up))
			{
				mCameraPosition += Vector3.Up * mCameraSpeed;
				mCameraLookAtVector += Vector3.Up * mCameraSpeed;
			}
			if (input.mCurrentKeyboardState.IsKeyDown(Keys.Down))
			{
				mCameraPosition += Vector3.Down * mCameraSpeed;
				mCameraLookAtVector += Vector3.Down * mCameraSpeed;
			}

			if (input.mCurrentKeyboardState.IsKeyDown(Keys.PageDown))
			{
				mCameraPosition -= lookAtVec * -1 * mCameraSpeed;
			}
			if (input.mCurrentKeyboardState.IsKeyDown(Keys.PageUp))
			{
				mCameraPosition += lookAtVec * -1 * mCameraSpeed;
			}

		}

		void DrawGround()
		{
			mEffect.View = Matrix.CreateLookAt(
				mCameraPosition, mCameraLookAtVector, mCameraUpVector);

			var aspectRatio =
				mGraphicsDeviceManager.PreferredBackBufferWidth / (float)mGraphicsDeviceManager.PreferredBackBufferHeight;
			const float fieldOfView = MathHelper.PiOver4;
			const float nearClipPlane = 1;
			const float farClipPlane = 2000;

			mEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
				fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

			mEffect.VertexColorEnabled = true;

			mGraphicsDevice.SetVertexBuffer(vb);
			mGraphicsDevice.Indices = ib;

			// solid render
			var rSolidState = new RasterizerState();
			rSolidState.FillMode = FillMode.Solid;
			rSolidState.CullMode = CullMode.CullCounterClockwiseFace;
			mGraphicsDevice.RasterizerState = rSolidState;
			foreach (var pass in mEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				mGraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, ib.IndexCount / 3);
			}

			// wireframe
			var wf = (BasicEffect)mEffect.Clone();
			wf.DiffuseColor = Vector3.Zero;
			wf.AmbientLightColor = Vector3.Zero;
			wf.EmissiveColor = Vector3.Zero;
			wf.SpecularColor = Vector3.Zero;
			var rWFState = new RasterizerState();
			rWFState.FillMode = FillMode.WireFrame;
			rWFState.CullMode = CullMode.CullCounterClockwiseFace;
			mGraphicsDevice.RasterizerState = rWFState;
			foreach (var pass in wf.CurrentTechnique.Passes)
			{
				pass.Apply();
				mGraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, ib.IndexCount / 3);
			}
		}

		public void Draw(GameTime gameTime)
		{
			mGraphicsDevice.Clear(Color.SteelBlue);
			DrawGround();
		}

		VertexPositionColor[] MakeIndexedQuad(int x1, int y1, int x2, int y2)
		{
			const int maxHeight = 10;
			var verts = new VertexPositionColor[4];
			verts[0].Position = new Vector3(x1, y1, mRandom.Next(maxHeight));
			verts[1].Position = new Vector3(x2, y1, mRandom.Next(maxHeight));
			verts[2].Position = new Vector3(x1, y2, mRandom.Next(maxHeight));
			verts[3].Position = new Vector3(x2, y2, mRandom.Next(maxHeight));

			return verts;
		}

		public void Initialize(GraphicsDeviceManager graphicsDeviceManager)
		{
			var vVerts = new List<VertexPositionColor>();
			var mIndices = new List<int>();

			mGraphicsDeviceManager = graphicsDeviceManager;
			mGraphicsDevice = mGraphicsDeviceManager.GraphicsDevice;
			const int scale = 10;
			const int yGridSize = 50;
			const int xGridSize = 50;

			for (var y = 0; y < yGridSize; y++)
			{
				for (var x = 0; x < xGridSize; x++)
				{
					// verts
					var verts = MakeIndexedQuad(
						x * scale,
						y * scale,
						(x + 1) * scale,
						(y + 1) * scale);

					// this section finds if the vert we just made exists already
					// if it does exist we get it's index (with FindIndex) and use it
					// if it doesn't exist we add the new vert and get its new index (which is the back of the list)
					// also we only check matches on X and Y and ignore Z axis since the grid should remain joined along X-Y
					var index0 = vVerts.FindIndex(a => (a.Position.X == verts[0].Position.X && a.Position.Y == verts[0].Position.Y));
					if (index0 == -1)
					{
						index0 = vVerts.Count;
						vVerts.Add(verts[0]);
					}

					var index1 = vVerts.FindIndex(a => (a.Position.X == verts[1].Position.X && a.Position.Y == verts[1].Position.Y));
					if (index1 == -1)
					{
						index1 = vVerts.Count;
						vVerts.Add(verts[1]);
					}

					var index2 = vVerts.FindIndex(a => (a.Position.X == verts[2].Position.X && a.Position.Y == verts[2].Position.Y));
					if (index2 == -1)
					{
						index2 = vVerts.Count;
						vVerts.Add(verts[2]);
					}

					var index3 = vVerts.FindIndex(a => (a.Position.X == verts[3].Position.X && a.Position.Y == verts[3].Position.Y));
					if (index3 == -1)
					{
						index3 = vVerts.Count;
						vVerts.Add(verts[3]);
					}

					// add two CCW triangles to form a quad
					mIndices.Add(index0);
					mIndices.Add(index2);
					mIndices.Add(index1);

					mIndices.Add(index1);
					mIndices.Add(index2);
					mIndices.Add(index3);
				}
			}
			mVerts = vVerts.ToArray();

			Console.WriteLine("VertexCount={0}", mVerts.Length);
			Console.WriteLine("IndexCount={0}", mIndices.Count);

			var palette = new Color[6]
			{
				new Color(0, 103, 51),
				new Color(0, 107, 51),
				new Color(0, 117, 51),
				new Color(0, 129, 51),
				new Color(0, 153, 51),
				new Color(0, 187, 51),
			};

			for (var i = 0; i < mVerts.Length; ++i)
			{
				var rand = mRandom.Next(6);
				mVerts[i].Color = palette[rand];
			}

			// setup vb
			vb = new VertexBuffer(mGraphicsDevice, typeof(VertexPositionColor), mVerts.Length, BufferUsage.WriteOnly);
			vb.SetData(mVerts);

			// setup ib
			ib = new IndexBuffer(mGraphicsDevice, typeof(int), mIndices.Count, BufferUsage.WriteOnly);
			ib.SetData(mIndices.ToArray());

			// We’ll be assigning texture values later

			mEffect = new BasicEffect(mGraphicsDevice);
		}

		public void LoadContent(Dictionary<string, Texture2D> textureAtlas)
		{
			mTextureAtlas = textureAtlas;
		}
	}
}
