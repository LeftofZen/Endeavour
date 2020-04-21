using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApp1
{
	class Iso
	{
		int tileSize;
		int halfTileSize;
		int quarterTileSize;
		int worldSize;
		int worldHeight;

		Bitmap tile;
		Bitmap bmp;
		Graphics g;

		int[,] world;

		public Iso()
		{
			tileSize = 32;
			halfTileSize = tileSize / 2;
			quarterTileSize = tileSize / 4;
			worldSize = 8;
			worldHeight = 2;

			tile = new Bitmap("isotile.png");
			tile.MakeTransparent(Color.White);
			world = new int[worldSize, worldSize];

			bmp = new Bitmap(512, 512, PixelFormat.Format32bppArgb);
			g = Graphics.FromImage(bmp);

			var r = new Random(123);

			for (var y = 0; y < worldSize; ++y)
			{
				for (var x = 0; x < worldSize; ++x)
				{
					world[x, y] = r.Next(1, worldHeight + 1);
				}
			}
		}

		public void Draw()
		{
			// diagonal drawing
			//for (int y = 1; y < worldSize-1; ++y)
			//{
			//	for (int x = 1; x < worldSize-1; ++x)
			//	{
			//		world[x, y] = (world[x, y] + world[x+1, y] + world[x-1, y] + world[x, y+1] + world[x, y-1]) / 5;

			//		for (int z = 0; z < world[x,y]; ++z)
			//		{
			//			g.DrawImage(tile, x * tileSize + (y * 8), (y * 4) - z * 7 + 128);
			//		}
			//	}
			//}

			// vertical drawing
			for (var y = 1; y < worldSize - 1; ++y)
			{
				for (var x = 2; x < worldSize - 2; x += 2)
				{
					DrawTileColumn(x, y);
				}

				for (var x = 1; x < worldSize - 1; x += 2)
				{
					DrawTileColumn(x, y);
				}
			}

			bmp.Save("iso_test.png", ImageFormat.Png);

		}
		private void DrawTileColumn(int x, int y)
		{
			//world[x, y] = (world[x, y] + world[x + 1, y] + world[x - 1, y] + world[x, y + 1] + world[x, y - 1]) / 5;

			for (var z = 0; z < world[x, y]; ++z)
			{
				g.DrawImage(tile, x * tileSize - (x * halfTileSize), (y * halfTileSize) - (z * (halfTileSize - 1)) + 128 + (x % 2 * quarterTileSize));
				//break;
			}
		}

		public static void IsoMain(string[] args)
		{
			var iso = new Iso();
			iso.Draw();
		}

	}
}
